using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HybridClient.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HybridClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var user = User.Identity;
            return View();
        }

        [Authorize(Roles = "管理员,普通用户")]
        public async Task<IActionResult> AccessApi1()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            client.SetBearerToken(accessToken);
            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RenewTokensAsync();
                    return RedirectToAction();
                }

                throw new Exception(response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync();

            return View("AccessApi1", content);
        }

        // [Authorize(Roles = "管理员")]
        [Authorize(Policy = "SmithInSomewhere")]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            // var code = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);

            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var revokeAccessTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = disco.RevocationEndpoint,
                    ClientId = "hybrid client",
                    ClientSecret = "hybrid secret",
                    Token = accessToken
                });

                if (revokeAccessTokenResponse.IsError)
                {
                    throw new Exception("Access Token Revocation Failed: " + revokeAccessTokenResponse.Error);
                }
            }

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeRefreshTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = disco.RevocationEndpoint,
                    ClientId = "hybrid client",
                    ClientSecret = "hybrid secret",
                    Token = refreshToken
                });

                if (revokeRefreshTokenResponse.IsError)
                {
                    throw new Exception("Refresh Token Revocation Failed: " + revokeRefreshTokenResponse.Error);
                }
            }

            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        private async Task<string> RenewTokensAsync()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // Refresh Access Token
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc client",
                ClientSecret = "mvc secret",
                Scope = "api1 openid profile email phone address",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResponse.IdentityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResponse.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };

            // 获取身份认证的结果，包含当前的pricipal和properties
            var currentAuthenticateResult =
                await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 把新的tokens存起来
            currentAuthenticateResult.Properties.StoreTokens(tokens);

            // 登录
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            return tokenResponse.AccessToken;
        }
    }
}
