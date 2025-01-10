using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class ManagementController : Controller
{
    private readonly ILogger _logger;

    public ManagementController(ILogger<ManagementController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}