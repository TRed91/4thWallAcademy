using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class WeaknessesController : Controller
{
    private readonly ILogger _logger;
    private readonly IWeaknessService _weaknessService;

    public WeaknessesController(ILogger<WeaknessesController> logger, IWeaknessService weaknessService)
    {
        _logger = logger;
        _weaknessService = weaknessService;
    }

    public IActionResult Details(int id)
    {
        var result = _weaknessService.GetWeaknessById(id);
        if (!result.Ok)
        {
            var errMsg = $"Error getting weakness by id {id}: {result.Message}";
            _logger.LogError(errMsg);
            var errTempDataMsg = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempDataMsg);
            return RedirectToAction("Students", "Admissions");
        }
        
        return View(result.Data);
    }
}