using BlogMVC.Data;
using BlogMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public class AccountController : Controller {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, SeedingDB seedingDB) {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model) {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded) {
            return RedirectToAction("Index", "Main");
        }
        ModelState.AddModelError(string.Empty, "Invalid Login!");
    return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model) {
        if (ModelState.IsValid) {
            var user = new User { UserName = model.Email, Email = model.Email, NickName = model.NickName};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) {

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Main");
            }
            foreach (var error in result.Errors) {
                if (error.Code == "DuplicateEmail") {
                    ModelState.AddModelError(nameof(model.Email), error.Description);
                }
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout() {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Main");
    }
}