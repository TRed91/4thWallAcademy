using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

[Authorize(Roles = "Manager, Admin")]
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