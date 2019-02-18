using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityModel.Client;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        private string _accessToken;
        private DiscoveryResponse _disco;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RequestAccessToken_ButtonClick(object sender, RoutedEventArgs e)
        {
            var userName = UserNameInput.Text;
            var password = PasswordInput.Password;

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
            _disco = disco;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request access token
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "wpf client",
                ClientSecret = "wpf secrect",
                Scope = "api1 openid profile address phone email",

                UserName = userName,
                Password = password
            });

            if (tokenResponse.IsError)
            {
                MessageBox.Show(tokenResponse.Error);
                return;
            }

            _accessToken = tokenResponse.AccessToken;
            AccessTokenTextBlock.Text = tokenResponse.Json.ToString();
        }

        private async void RequestApi1Resource_ButtonClick(object sender, RoutedEventArgs e)
        {
            // call API1 Resource
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(_accessToken);

            var response = await apiClient.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Api1ResponseTextBlock.Text = content;
            }

        }

        private async void RequestIdentityResource_ButtonClick(object sender, RoutedEventArgs e)
        {
            // call Identity Resource from Identity Server
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(_accessToken);

            var response = await apiClient.GetAsync(_disco.UserInfoEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                IdentityResponseTextBlock.Text = content;
            }
        }
    }
}
