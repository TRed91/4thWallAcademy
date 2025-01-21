using FourthWallAcademy.MVC.db.Entities;
using FourthWallAcademy.MVC.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityStudent> _userManager;
    private readonly SignInManager<IdentityStudent> _signInManager;

    public AccountController(UserManager<IdentityStudent> userManager, 
        SignInManager<IdentityStudent> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var model = new LoginModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(
            model.Username, model.Password, model.RememberMe, 
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
        
        var student = await _userManager.FindByNameAsync(model.Username);
        if (student == null)
        {
            ModelState.AddModelError(string.Empty, "Error fetching student data.");
            return View(model);
        }
        
        return RedirectToAction("Profile", "Student", new { id = student.StudentID });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}