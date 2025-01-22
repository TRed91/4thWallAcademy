using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class PowersController : Controller
{
    private readonly ILogger _logger;
    private readonly IPowerService _powerService;

    public PowersController(ILogger<PowersController> logger, IPowerService powerService)
    {
        _logger = logger;
        _powerService = powerService;
    }

    public IActionResult Details(int id)
    {
        var result = _powerService.GetPowerById(id);
        if (!result.Ok)
        {
            var errMsg = $"Error getting power by id {id}: {result.Message}";
            _logger.LogError(errMsg);
            var errTempDataMsg = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempDataMsg);
            return RedirectToAction("Students", "Admissions");
        }
        
        return View(result.Data);
    }
}