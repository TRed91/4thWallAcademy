using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger _logger;

    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
}