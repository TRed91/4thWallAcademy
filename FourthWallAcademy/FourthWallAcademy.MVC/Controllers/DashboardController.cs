using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models.ReportModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;
    private readonly IPowerService _powerService;
    private readonly IWeaknessService _weaknessService;

    public DashboardController(ILogger<DashboardController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        IPowerService powerService,
        IWeaknessService weaknessService)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _powerService = powerService;
        _weaknessService = weaknessService;
    }
    
    public IActionResult Index()
    {
        var gradesResult = _studentService.GetGradesReport();
        var enrollmentResult = _sectionService.GetEnrollmentReport(new DateTime(1900, 1, 1), new DateTime(9999, 12, 31));
        var powersResult = _powerService.GetPowersReport();
        var weaknessResult = _weaknessService.GetWeaknessReport();

        if (!gradesResult.Ok || !enrollmentResult.Ok || !powersResult.Ok || !weaknessResult.Ok)
        {
            var errMsg = "There was an error retrieving reports data.";
            _logger.LogError(errMsg + ": " + string.Join(", ", new string[]
            {
                gradesResult.Message, 
                enrollmentResult.Message, 
                powersResult.Message, 
                weaknessResult.Message
            }));
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            
            return RedirectToAction("Index", "Home");
        }

        var model = new DashboardModel
        {
            GradesReport = gradesResult.Data,
            EnrollmentReport = enrollmentResult.Data,
            PowersReport = powersResult.Data,
            WeaknessesReport = weaknessResult.Data,
        };
        
        return View(model);
    }
}