using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.UtilityClasses;
using FourthWallAcademy.MVC.Views.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

public class AdmissionsController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;
    private readonly IPowerService _powerService;

    public AdmissionsController(ILogger<AdmissionsController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        IPowerService powerService)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _powerService = powerService;
    }
    
    public IActionResult Students(AdmissionsStudentsModel? model)
    {
        if (model.Form == null)
        {
            model.Form = new AdmissionsStudentsForm();
        }
        // fetch students data
        var studentsResult = _studentService.GetStudentsByStartingLetter(model.Form.startsWith);

        if (!studentsResult.Ok)
        {
            _logger.LogWarning($"Error retrieving Students with letter {model.Form.startsWith}: {studentsResult.Message}");
            var msg = new TempDataExtension(false, $"Error retrieving Students with letter '{model.Form.startsWith}'.");
            TempData["message"] = TempDataSerializer.Serialize(msg);
            return RedirectToAction("Index", "Home");
        }

        // Order the list
        switch (model.Form.Order)
        {
            case AdmissionsStudentsOrder.BirthDate:
                model.Students = studentsResult.Data.OrderBy(s => s.DoB).Reverse().ToList();
                break;
            default:
                model.Students = studentsResult.Data.OrderBy(s => s.Alias).ToList();
                break;
        }

        // Reduce list to search string
        if (!string.IsNullOrEmpty(model.Form.searchString))
        {
            model.Students = model.Students
                .Where(s => s.Alias.Contains(model.Form.searchString))
                .ToList();
        }
        
        // Populate select lists
        model.OrderList = new SelectList(new List<SelectListItem>
        {
            new SelectListItem("Name", "1"),
            new SelectListItem("BirthDate", "2"),
        }, "Value", "Text");
        model.LetterList = SelectListFactory.CreateAlphabetSelectList();
        
        return View(model);
    }

    [HttpGet]
    public IActionResult NewStudent()
    {
        var model = new StudentFormModel();
        model.DoB = DateTime.Today;
        return View(model);
    }

    [HttpPost]
    public IActionResult NewStudent(StudentFormModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View(formModel);
        }

        var student = formModel.ToEntity();
        var addResult = _studentService.AddStudent(student);
        
        if (!addResult.Ok)
        {
            _logger.LogError($"Error adding new student: {addResult.Message}");
            var errMsg = new TempDataExtension(false, $"Error adding new student: {addResult.Message}");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return View(formModel);
        }

        var msg = $"New student added: Id: {student.StudentID}, Alias: {student.Alias}";
        _logger.LogInformation(msg);
        var tempMsg = new TempDataExtension(true, msg);
        TempData["message"] = TempDataSerializer.Serialize(tempMsg);
        return RedirectToAction("Students");
    }

    [HttpGet]
    public IActionResult EditStudent(int id)
    {
        var studentResult = _studentService.GetStudentById(id);
        if (!studentResult.Ok)
        {
            var msg = $"Error retrieving student with id {id}: {studentResult.Message}";
            _logger.LogError(msg);
            var errMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentDetails", new { id });
        }
        var model = new StudentFormModel(studentResult.Data);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStudent(int id, StudentFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Fetch the old previous student data
        var studentResult = _studentService.GetStudentById(id);
        if (!studentResult.Ok)
        {
            var msg = $"Error retrieving student with id {id}: {studentResult.Message}";
            _logger.LogError(msg);
            var errMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            
            return View(model);
        }
        // replace the student data with model
        var student = studentResult.Data;
        student.Alias = model.Alias;
        student.FirstName = model.FirstName;
        student.LastName = model.LastName;
        student.DoB = model.DoB;
        
        // update
        var updateResult = _studentService.UpdateStudent(student);
        if (!updateResult.Ok)
        {
            var msg = $"Error updating student with id {id}: {updateResult.Message}";
            _logger.LogError(msg);
            var errMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            
            return View(model);
        }

        var successMsg = $"Student with id {id} updated";
        _logger.LogInformation(successMsg);
        var tempMsg = new TempDataExtension(true, successMsg);
        TempData["message"] = TempDataSerializer.Serialize(tempMsg);
        
        return RedirectToAction("StudentDetails", new { id });
    }

    public IActionResult StudentDetails(int id)
    {
        var profileResult = _studentService.GetStudentById(id);

        if (!profileResult.Ok)
        {
            var msg = $"Error retrieving Student Details for Student with id {id}: {profileResult.Message}";
            _logger.LogError(msg);
            var errorMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errorMsg);
        }

        var sectionsResult = _sectionService.GetStudentSections(id);

        if (!sectionsResult.Ok)
        {
            var msg = $"Error retrieving Sections for Student with id {id}: {sectionsResult.Message}";
            _logger.LogError(msg);
            var errorMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errorMsg);
        }

        var model = new StudentProfile
        {
            Student = profileResult.Data,
            Sections = sectionsResult.Data
        };
        
        return View(model);
    }

    public IActionResult StudentPowers(int id)
    {
        var studentResult = _studentService.GetStudentById(id);
        if (!studentResult.Ok)
        {
            var msg = $"Error retrieving student with id {id}: {studentResult.Message}";
            _logger.LogError(msg);
            var errorMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errorMsg);
            return RedirectToAction("StudentDetails", new { id });
        }

        var powersResult = _powerService.GetPowers();

        var selectList = powersResult.Data.OrderBy(p => p.PowerName)
            .Select(p => new SelectListItem
            {
                Value = p.PowerID.ToString(),
                Text = p.PowerName
            }).ToList();
        var head = new SelectListItem { Value = "", Text = "- SELECT POWER" };
        selectList.Insert(0, head);
            
        var model = new StudentPowersModel
        {
            Form = new StudentPowersForm(),
            Powers = studentResult.Data.StudentPowers
                .OrderBy(p => p.Power.PowerName)
                .ToList(),
            PowersSelectList = new SelectList(selectList, "Value", "Text"),
            StudentID = studentResult.Data.StudentID,
            StudentAlias = studentResult.Data.Alias
        };
        
        return View(model);
    }
}