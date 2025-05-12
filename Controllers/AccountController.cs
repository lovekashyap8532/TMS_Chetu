using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUserNew> userManager { get; }

        private SignInManager<IdentityUserNew> signInManager { get; }
        public AccountController(
            UserManager<IdentityUserNew> _userManager,
            SignInManager<IdentityUserNew> _signInManager
            )
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register user)
        {
            if (user == null)
            {
                ModelState.AddModelError("", "User details can't Empty!!");
                return View();
            }
            else if (ModelState.IsValid)
            {
                var res = await userManager.FindByEmailAsync(user.Email);
                if (res == null)
                {
                    var userData = new IdentityUserNew()
                    {
                        UserName = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        //Status = "IsActive",
                        Email = user.Email,
                    };
                    var result = await userManager.CreateAsync(userData, user.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("HomeIndex", "Home");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Already Exists !!!");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Enter a Valid Details!!");
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login user)
        {
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    var res = await userManager.FindByEmailAsync(user.Email);
                    if (res != null )
                    {
                        Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(res, user.Password, user.RememberMe, true);
                        if (signInResult.Succeeded)
                        {
                            return RedirectToAction("HomeIndex", "Home");
                        }
                        else if (signInResult.IsLockedOut)
                        {
                            ModelState.AddModelError("", "Login Attempt Exceeded Max Limit !!!,\n Please contact to Administrator");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Enter Valid Email/Password");
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Please Enter Valid Email/Password");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Enter Email and Password");
                return View();
            }
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
