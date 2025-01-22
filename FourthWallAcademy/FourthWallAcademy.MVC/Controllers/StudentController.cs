using System.Security.Claims;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.db.Entities;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.Models.SectionModels;
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
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentController(ILogger<StudentController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user.StudentID == 0)
        {
            var tempDataMsg = new TempDataExtension(false,
                "Admins and Admission Agents can view Student Profiles from the 'Admission' Section");
            var tempData = TempDataSerializer.Serialize(tempDataMsg);
            TempData["Message"] = tempData;
            return RedirectToAction("Index", "Home");
        }
        // Fetch Data
        var profileResult = _studentService.GetStudentById(user.StudentID);

        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving student with id {user.StudentID}: {profileResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving student with id {user.StudentID}");
            TempData["message"] = TempDataSerializer.Serialize(td);
            return RedirectToAction("Index", "Home");
        }

        var sectionsResult = _sectionService.GetStudentSections(user.StudentID);
        
        if (!profileResult.Ok)
        {
            _logger.LogWarning($"Error retrieving sections of student with id {user.StudentID}: {sectionsResult.Message}");
            var td = new TempDataExtension(false, 
                $"Error retrieving sections of student with id {user.StudentID}");
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
    
    public async Task<IActionResult> Schedule(ScheduleModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user.StudentID == 0)
        {
            var tempDataMsg = new TempDataExtension(false,
                "Only students can view their Schedule.");
            var tempData = TempDataSerializer.Serialize(tempDataMsg);
            TempData["Message"] = tempData;
            return RedirectToAction("Index", "Home");
        }
        
        var scheduleResult = _sectionService.GetStudentSchedule(user.StudentID, 
            model.Form.From, 
            model.Form.To);

        if (!scheduleResult.Ok)
        {
            var errMsg = $"Error retrieving schedule for student with id {user.StudentID}: {scheduleResult.Message}";
            _logger.LogError(errMsg);
            var tempData = new TempDataExtension(false,  errMsg);
            TempData["message"] = TempDataSerializer.Serialize(tempData);
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

    public IActionResult SectionDetails(int id)
    {
        var sectionsResult = _sectionService.GetSectionById(id);
        var studentSectionsResult = _sectionService.GetStudentsBySection(id);

        if (!sectionsResult.Ok || !studentSectionsResult.Ok)
        {
            var errMsg = "There was an error retrieving the section details.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + sectionsResult.Message + ", " + studentSectionsResult.Message);
            return RedirectToAction("Profile");
        }

        sectionsResult.Data.StudentSections = studentSectionsResult.Data;

        var model = new SectionDetailsModel
        {
            Section = sectionsResult.Data,
        };
        
        return View(model);
    }
}