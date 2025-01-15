using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}