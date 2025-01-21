using System.Security.Claims;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.db.Entities;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.Models.StudentModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

[Authorize(Roles = "Student, Admission, Manager, Admin")]
public class StudentController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;
    private readonly UserManager<IdentityStudent> _userManager;

    public StudentController(ILogger<StudentController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        UserManager<IdentityStudent> userManager)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Profile()
    {
        var student = await _userManager.GetUserAsync(User);
        
        // Fetch Data
        var profileResult = _studentService.GetStudentById(student.StudentID);

        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving student with id {student.StudentID}: {profileResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving student with id {student.StudentID}");
            TempData["message"] = TempDataSerializer.Serialize(td);
            return RedirectToAction("Index", "Home");
        }

        var sectionsResult = _sectionService.GetStudentSections(student.StudentID);
        
        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving sections of student with id {student.StudentID}: {sectionsResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving sections of student with id {student.StudentID}");
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