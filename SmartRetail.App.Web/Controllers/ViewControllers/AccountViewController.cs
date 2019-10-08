using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.ViewModel.Auth;

namespace SmartRetail.App.Web.Controllers.ViewControllers
{
    public class AccountViewController : Controller
    {
        private IUserRepository userRepo;
        public AccountViewController(IUserRepository _userRepo)
        {
            userRepo = _userRepo;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userRepo.GetByLogin(model.Username);
                if (user != null)
                {
                    var isAuth = await Authenticate(model); // аутентификация
                    if (isAuth)
                    {
                        return RedirectToAction("Index", "Main");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

 
        private async Task<bool> Authenticate(LoginViewModel user)
        {
            var claimsIdentity =
                await new SmartRetail.App.Web.Controllers.ApiControllers.AccountController(userRepo).GetIdentity(
                    user.Username, user.Password);
            if (claimsIdentity == null)
            {
                return false;
            }
            
            //ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return true;
        }
 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "AccountView");
        }
    }
}