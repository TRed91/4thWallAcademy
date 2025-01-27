using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.db.Entities;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.Models.StudentModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

public class AdmissionsController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;
    private readonly IPowerService _powerService;
    private readonly IWeaknessService _weaknessService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdmissionsController(ILogger<AdmissionsController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        IPowerService powerService,
        IWeaknessService weaknessService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _powerService = powerService;
        _weaknessService = weaknessService;
        _userManager = userManager;
    }
    
    [Authorize(Roles = "Admission, Admin")]
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

    [Authorize(Roles = "Admission, Admin")]
    [HttpGet]
    public IActionResult NewStudent()
    {
        var model = new StudentFormModel();
        model.DoB = DateTime.Today;
        return View(model);
    }
    
    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewStudent(StudentFormModel formModel)
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
        
        // remove whitespace from alias
        string formattedAlias = string.Join("", student.Alias
            .Where(c => !char.IsWhiteSpace(c)));
        
        var user = new ApplicationUser{ StudentID = student.StudentID, UserName = formattedAlias };
        var result = await _userManager.CreateAsync(user, addResult.Data.Password);
        
        if (!result.Succeeded)
        {
            List<string> errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogError($"Error adding new student account: {string.Join(", ", errors)}");
            return View(formModel);
        }
        
        // Add the student role
        var registeredUser = await _userManager.FindByNameAsync(formattedAlias);
        if (registeredUser != null)
        {
            await _userManager.AddToRoleAsync(registeredUser, "Student");
        }

        var msg = $"New student added: Id: {student.StudentID}, Alias: {student.Alias}";
        _logger.LogInformation(msg);
        var tempMsg = new TempDataExtension(true, msg);
        TempData["message"] = TempDataSerializer.Serialize(tempMsg);
        return RedirectToAction("Students");
    }

    [Authorize(Roles = "Admission, Admin")]
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

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStudent(int id, StudentFormModel model)
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
        
        // format the old and new alias to update the identity record
        string oldFormattedAlias = string.Join("",studentResult.Data.Alias
            .Where(c => !char.IsWhiteSpace(c)));
        
        string newFormattedAlias = string.Join("", model.Alias
            .Where(c => !char.IsWhiteSpace(c)));
        
        // replace the student data with model
        var student = studentResult.Data;
        student.Alias = model.Alias;
        student.FirstName = model.FirstName;
        student.LastName = model.LastName;
        student.DoB = model.DoB;
        
        // update the student table
        var updateResult = _studentService.UpdateStudent(student);
        if (!updateResult.Ok)
        {
            var msg = $"Error updating student with id {id}: {updateResult.Message}";
            _logger.LogError(msg);
            var errMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return View(model);
        }
        
        // fetch and update the identity
        var identityUser = await _userManager.FindByNameAsync(oldFormattedAlias);
        if (identityUser == null)
        {
            var msg = $"Student Identity not found: {oldFormattedAlias}";
            _logger.LogError(msg);
            var errMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentDetails", new { id });
        }
        
        identityUser.UserName = newFormattedAlias;
        identityUser.NormalizedUserName = newFormattedAlias.ToUpper();
        var result = await _userManager.UpdateAsync(identityUser);
        if (!result.Succeeded)
        {
            var errMsg = $"Error updating student account: {string.Join(", ", result.Errors)}";
            _logger.LogError(errMsg);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        var successMsg = $"Student with id {id} updated";
        _logger.LogInformation(successMsg);
        var tempMsg = new TempDataExtension(true, successMsg);
        TempData["message"] = TempDataSerializer.Serialize(tempMsg);
        
        return RedirectToAction("StudentDetails", new { id });
    }

    [Authorize(Roles = "Manager, Admission, Admin")]
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

    [Authorize(Roles = "Admission, Admin")]
    [HttpGet]
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
        var head = new SelectListItem { Value = "", Text = "- SELECT POWER -" };
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

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StudentPowers(int id, StudentPowersModel model)
    {
        if (!ModelState.IsValid)
        {
            var errMsg = new TempDataExtension(false, "Invalid data input");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentPowers", new { id });
        }

        var studentPower = new StudentPower
        {
            StudentID = id,
            PowerID = model.Form.PowerID,
            Rating = (byte)model.Form.Rating,
        };
        
        var addResult = _studentService.AddStudentPower(studentPower);
        if (!addResult.Ok)
        {
            _logger.LogError($"Error adding Student Power: {addResult.Message}");
            var errMsg = new TempDataExtension(false, "Error adding Student Power");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentPowers", new { id });
        }
        
        _logger.LogInformation($"Student power added");
        var msg = new TempDataExtension(true, $"Student power added");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentPowers", new { id });
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpGet]
    public IActionResult EditStudentPower(int studentId, int powerId)
    {
        var studentResult = _studentService.GetStudentById(studentId);
        var studentPower = studentResult.Data.StudentPowers.FirstOrDefault(p => p.PowerID == powerId);
        studentPower.Power = studentResult.Data.StudentPowers
            .Where(p => p.PowerID == powerId)
            .Select(p => p.Power)
            .FirstOrDefault();

        var model = new StudentPowerEditModel
        {
            Alias = studentResult.Data.Alias,
            PowerName = studentPower.Power.PowerName,
            PowerId = powerId,
            StudentId = studentId,
            Rating = studentPower.Rating,
        };
        return View(model);
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStudentPower(StudentPowerEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var studentPower = new StudentPower
        {
            StudentID = model.StudentId,
            PowerID = model.PowerId,
            Rating = model.Rating,
        };
        var updateResult = _studentService.UpdateStudentPower(studentPower);
        if (!updateResult.Ok)
        {
            _logger.LogError($"Error updating Student Power: {updateResult.Message}");
            var errMsg = new TempDataExtension(false, "Error updating Student Power");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return View(model);
        }
        
        _logger.LogInformation($"Student power updated" + $"Student ID: {model.StudentId}, PowerID: {model.PowerId}");
        var msg = new TempDataExtension(true, $"Student power updated");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentPowers", new { id = model.StudentId });
    }
    
    [Authorize(Roles = "Admission, Admin")]
    [HttpGet]
    public IActionResult EditStudentWeakness(int studentId, int weaknessId)
    {
        var studentResult = _studentService.GetStudentById(studentId);
        var studentWeakness = studentResult.Data.StudentWeaknesses.FirstOrDefault(w => w.WeaknessID == weaknessId);
        studentWeakness.Weakness = studentResult.Data.StudentWeaknesses
            .Where(w => w.WeaknessID == weaknessId)
            .Select(p => p.Weakness)
            .FirstOrDefault();

        var model = new StudentWeaknessEditModel
        {
            Alias = studentResult.Data.Alias,
            WeaknessName = studentWeakness.Weakness.WeaknessName,
            WeaknessId = weaknessId,
            StudentId = studentId,
            RiskLevel = studentWeakness.RiskLevel,
        };
        return View(model);
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStudentWeakness(StudentWeaknessEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var studentWeakness = new StudentWeakness
        {
            StudentID = model.StudentId,
            WeaknessID = model.WeaknessId,
            RiskLevel = model.RiskLevel,
        };
        var updateResult = _studentService.UpdateStudentWeakness(studentWeakness);
        if (!updateResult.Ok)
        {
            _logger.LogError($"Error updating Student Weakness: {updateResult.Message}");
            var errMsg = new TempDataExtension(false, "Error updating Student Weakness");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return View(model);
        }
        
        _logger.LogInformation($"Student weakness updated" + $"Student ID: {model.StudentId}, Weakness ID: {model.WeaknessId}");
        var msg = new TempDataExtension(true, $"Student weakness updated");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentWeaknesses", new { id = model.StudentId });
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpGet]
    public IActionResult StudentWeaknesses (int id)
    {
        var studentResult = _studentService.GetStudentById(id);
        if (!studentResult.Ok)
        {
            var msg = $"Error retrieving student with id {id}: {studentResult.Message}";
            _logger.LogError(msg);
            var errorMsg = new TempDataExtension(false, msg);
            TempData["message"] = TempDataSerializer.Serialize(errorMsg);
        }

        var weaknessesResult = _weaknessService.GetWeaknesses();
        var selectList = weaknessesResult.Data
            .OrderBy(w => w.WeaknessName)
            .Select(w => new SelectListItem
            {
                Value = w.WeaknessID.ToString(),
                Text = w.WeaknessName
            }).ToList();
        var head = new SelectListItem { Value = "", Text = "- SELECT WEAKNESS -" };
        selectList.Insert(0, head);

        var model = new StudentWeaknessModel
        {
            Form = new StudentWeaknessForm(),
            Weaknesses = studentResult.Data.StudentWeaknesses
                .OrderBy(w => w.Weakness.WeaknessName)
                .ToList(),
            WeaknessesSelectList = new SelectList(selectList, "Value", "Text"),
            StudentID = studentResult.Data.StudentID,
            StudentAlias = studentResult.Data.Alias
        };
        return View(model);
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StudentWeaknesses(int id, StudentWeaknessModel model)
    {
        if (!ModelState.IsValid)
        {
            var errMsg = new TempDataExtension(false, "Invalid data input");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentWeaknesses", new { id });
        }

        var studentWeakness = new StudentWeakness
        {
            StudentID = id,
            WeaknessID = model.Form.WeaknessID,
            RiskLevel = (byte)model.Form.RiskLevel,
        };
        
        var addResult = _studentService.AddStudentWeakness(studentWeakness);
        if (!addResult.Ok)
        {
            _logger.LogError($"Error adding Student Weakness: {addResult.Message}");
            var errMsg = new TempDataExtension(false, "Error adding Student Weakness");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentWeaknesses", new { id });
        }
        _logger.LogInformation($"Student Weakness added");
        var msg = new TempDataExtension(true, $"Student Weakness added");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentWeaknesses", new { id });
    }

    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveStudentWeakness(int studentId, int weaknessId)
    {
        var weakness = new StudentWeakness
        {
            StudentID = studentId,
            WeaknessID = weaknessId
        };
        var removeResult = _studentService.DeleteStudentWeakness(weakness);
        if (!removeResult.Ok)
        {
            _logger.LogError($"Error deleting Student Weakness: {removeResult.Message}");
            var errMsg = new TempDataExtension(false, "Error deleting Student Weakness");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentWeaknesses", new { id = studentId });
        }
        _logger.LogInformation($"Student Weakness removed");
        var msg = new TempDataExtension(true, $"Student Weakness removed");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentWeaknesses", new { id = studentId });
    }
    
    [Authorize(Roles = "Admission, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveStudentPower(int studentId, int powerId)
    {
        var power = new StudentPower
        {
            StudentID = studentId,
            PowerID = powerId
        };
        var removeResult = _studentService.DeleteStudentPower(power);
        if (!removeResult.Ok)
        {
            _logger.LogError($"Error deleting Student Weakness: {removeResult.Message}");
            var errMsg = new TempDataExtension(false, "Error deleting Student Power");
            TempData["message"] = TempDataSerializer.Serialize(errMsg);
            return RedirectToAction("StudentPowers", new { id = studentId });
        }
        _logger.LogInformation($"Student Weakness removed");
        var msg = new TempDataExtension(true, $"Student Power removed");
        TempData["message"] = TempDataSerializer.Serialize(msg);
        return RedirectToAction("StudentPowers", new { id = studentId });
    }
}