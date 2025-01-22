using System.Diagnostics;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.db.Entities;
using FourthWallAcademy.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStudentService _studentService;

    public HomeController(ILogger<HomeController> logger, 
        UserManager<ApplicationUser> userManager, 
        IStudentService studentService)
    {
        _logger = logger;
        _userManager = userManager;
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        HomeModel model = new HomeModel();
        if (user == null)
        {
            model.UserName = "Guest";
        }
        else
        {
            if (user.StudentID > 0)
            {
                var student = _studentService.GetStudentById(user.StudentID);
                model.UserName = student.Data.Alias;
            }
            else
            {
                model.UserName = user.UserName;
            }   
        }
        
        return View(model);
    }
}