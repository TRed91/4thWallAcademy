using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.UtilityClasses;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

public class StudentController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;

    public StudentController(ILogger<StudentController> logger, IStudentService studentService, ISectionService sectionService)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
    }
    
    public IActionResult Profile(int id)
    {
        // Fetch Data
        var profileResult = _studentService.GetStudentById(id);

        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving student with id {id}: {profileResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving student with id {id}");
            TempData["message"] = TempDataSerializer.Serialize(td);
            return RedirectToAction("Index", "Home");
        }

        var sectionsResult = _sectionService.GetStudentSections(id);
        
        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving sections of student with id {id}: {sectionsResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving sections of student with id {id}");
            TempData["message"] = TempDataSerializer.Serialize(td);
            return RedirectToAction("Index", "Home");
        }

        var model = new StudentProfile
        {
            Student = profileResult.Data,
            Sections = sectionsResult.Data
        };
        
        return View(model);
    }
    
    public IActionResult Schedule(int id, ScheduleModel? model)
    {
        if (model.Form == null)
        {
            model.Form = new ScheduleForm();
        }
        
        var scheduleResult = _sectionService.GetStudentSchedule(id, 
            model.Form.From, 
            model.Form.To);

        if (!scheduleResult.Ok)
        {
            _logger.LogWarning($"Error retrieving schedule for student with id {id}: {scheduleResult.Message}");
            var msg = new TempDataExtension(false,  $"Error retrieving schedule for student with id {id}");
            TempData["message"] = TempDataSerializer.Serialize(msg);
            return RedirectToAction("Index", "Home");
        }
        
        model.Schedule = scheduleResult.Data;
        
        return View(model);
    }

    public IActionResult Section(int id)
    {
        var sectionResult = _sectionService.GetSectionById(id);
        if (!sectionResult.Ok)
        {
            _logger.LogWarning($"Error retrieving section details with id {id}: {sectionResult.Message}");
            var msg = new TempDataExtension(false,  $"Error retrieving section details with id {id}");
            TempData["message"] = TempDataSerializer.Serialize(msg);
            return RedirectToAction("Index", "Home");
                // !!! refactor to Redirect to profile with student Id instead !!!
        }
        return View(sectionResult.Data);
    }
}