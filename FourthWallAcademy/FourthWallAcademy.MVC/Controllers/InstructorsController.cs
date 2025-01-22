using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models.InstructorsModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.MVC.Controllers;

[Authorize(Roles = "Manager, Admin")]
public class InstructorsController : Controller
{
    private readonly ILogger _logger;
    private readonly IInstructorService _instructorService;

    public InstructorsController(ILogger<InstructorsController> logger, IInstructorService instructorService)
    {
        _logger = logger;
        _instructorService = instructorService;
    }
    public IActionResult Index(InstructorIndexModel model)
    {
        var instructorsResult = _instructorService.GetInstructors();
        if (!instructorsResult.Ok)
        {
            var errMsg = "Error retrieving instructors";
            _logger.LogError(errMsg + ", " + instructorsResult.Message);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Index", "Management");
        }
        var instructors = instructorsResult.Data;

        if (!model.Form.ShowTerminated)
        {
            instructors = instructors
                .Where(i => i.TermDate == null || i.TermDate > DateTime.Today)
                .ToList();
        }

        if (!string.IsNullOrEmpty(model.Form.SearchString))
        {
            instructors = instructors
                .Where(i => i.Alias.ToLower().Contains(model.Form.SearchString.ToLower()))
                .ToList();
        }

        switch (model.Form.Order)
        {
            case InstructorIndexOrder.HireDate:
                instructors = instructors.OrderBy(i => i.HireDate).ToList();
                break;
            case InstructorIndexOrder.TermDate:
                instructors = instructors.OrderBy(i => i.TermDate).ToList();
                break;
            default:
                instructors = instructors.OrderBy(i => i.Alias).ToList();
                break;
        }
        
        model.Instructors = instructors;
        model.GenerateSelectList();
        return View(model);
    }
    
    public IActionResult Details(int id)
    {
        var instructor = _instructorService.GetInstructorById(id);
        if (!instructor.Ok)
        {
            var errMsg = $"Error retrieving instructor: {instructor.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Index");
        }
        return View(new InstructorDetailsModel(instructor.Data));
    }

    [HttpGet]
    public IActionResult Add()
    {
        var model = new InstructorFormModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(InstructorFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var instructor = model.ToEntity();
        var addResult = _instructorService.AddInstructor(instructor);
        if (!addResult.Ok)
        {
            var errMsg = $"Error adding instructor: {model.Alias}, {addResult.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Index");
        }
        
        var sucMsg = $"Instructor {instructor.Alias} added with id {instructor.InstructorID}.";
        _logger.LogInformation(sucMsg);
        var sucTempData = new TempDataExtension(true, sucMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var instructor = _instructorService.GetInstructorById(id);
        if (!instructor.Ok)
        {
            var errMsg = $"Error retrieving instructor with id {id}: {instructor.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Details", new { id });
        }
        var model = new InstructorFormModel();
        model.Alias = instructor.Data.Alias;
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, InstructorFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var instructor = new Instructor();
        instructor.InstructorID = id;
        instructor.Alias = model.Alias;
        
        var updateResult = _instructorService.UpdateInstructor(instructor);
        if (!updateResult.Ok)
        {
            var errMsg = $"Error updating instructor: {updateResult.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return View(model);
        }
        
        var sucMsg = $"Updated instructor with id {id}.";
        _logger.LogInformation(sucMsg);
        var sucTempData = new TempDataExtension(true, sucMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    public IActionResult Terminate(int id)
    {
        var instructorResult = _instructorService.GetInstructorById(id);
        if (!instructorResult.Ok)
        {
            var errMsg = $"Error retrieving instructor: {instructorResult.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Details", new { id });
        }

        var model = new InstructorTerminateModel();
        model.Instructor = instructorResult.Data;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Terminate(int id, InstructorTerminateModel model)
    {
        if (!ModelState.IsValid)
        {
            var instructorResult = _instructorService.GetInstructorById(id);
            model.Instructor = instructorResult.Data;
            return View(model);
        }
        var terminateResult = _instructorService.TerminateInstructor(id, model.TerminationDate);
        if (!terminateResult.Ok)
        {
            var errMsg = $"Error terminating instructor: {terminateResult.Message}";
            _logger.LogError(errMsg);
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            return RedirectToAction("Details", new { id });
        }
        return RedirectToAction("Details", new { id });
    }
}