using Microsoft.AspNetCore.Mvc;

namespace HybridClient.Controllers
{
    public class AuthorizationController: Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
