using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.UtilityClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

public class SectionsController : Controller
{
    private readonly ILogger _logger;
    private readonly ISectionService _sectionService;
    private readonly ICourseService _courseService;
    private readonly IInstructorService _instructorService;

    public SectionsController(ILogger<SectionsController> logger, 
        ISectionService sectionService, 
        ICourseService courseService,
        IInstructorService instructorService)
    {
        _logger = logger;
        _sectionService = sectionService;
        _courseService = courseService;
        _instructorService = instructorService;
    }
    
    public IActionResult Index(SectionsIndexModel model)
    {
        // fetch data
        var sectionsResult = _sectionService.GetSections();
        var coursesResult = _courseService.GetCourses();

        if (!sectionsResult.Ok || !coursesResult.Ok)
        {
            var errMsg = "There was an error retrieving the sections.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + sectionsResult.Message + ", " + coursesResult.Message);
            return RedirectToAction("Index", "Management");
        }

        // add sections to courses
        var courses = coursesResult.Data;
        foreach (var course in courses)
        {
            course.Sections.AddRange(sectionsResult.Data
                .Where(s => s.CourseID == course.CourseID)
                .ToList());

            // order by form order option
            switch (model.Form.Order)
            {
                case SectionsOrderOptions.Instructor:
                    course.Sections = course.Sections
                        .OrderBy(s => s.Instructor.Alias)
                        .ToList();
                    break;
                case SectionsOrderOptions.StartDate:
                    course.Sections = course.Sections
                        .OrderBy(s => s.StartDate)
                        .ToList();
                    break;
                case SectionsOrderOptions.EndDate:
                    course.Sections = course.Sections
                        .OrderBy(s => s.EndDate)
                        .ToList();
                    break;
                case SectionsOrderOptions.StartTime:
                    course.Sections = course.Sections
                        .OrderBy(s => s.StartTime)
                        .ToList();
                    break;
            }
        }

        // reduce to filtered course id or order by course name
        if (model.Form.CourseID != 0)
        {
            courses = courses
                .Where(c => c.CourseID == model.Form.CourseID)
                .ToList();
        }
        else
        {
            courses = courses.OrderBy(c => c.CourseName).ToList();
        }
        
        model.Courses = courses;
        model.GenerateSelectLists(coursesResult.Data, model.Form.Order);
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Add()
    {
        var coursesResult = _courseService.GetCourses();
        var instructorsResult = _instructorService.GetInstructors();
        if (!coursesResult.Ok || !instructorsResult.Ok)
        {
            var errMsg = "There was an error retrieving select list data.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + coursesResult.Message + ", " + instructorsResult.Message);
            return RedirectToAction("Index"); 
        }

        var model = new SectionsFormModel();
        model.GenerateSelectLists(coursesResult.Data, instructorsResult.Data);
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Add(SectionsFormModel model)
    {
        var coursesResult = _courseService.GetCourses();
        var instructorsResult = _instructorService.GetInstructors();
        if (!coursesResult.Ok || !instructorsResult.Ok)
        {
            var errMsg = "There was an error retrieving select list data.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + coursesResult.Message + ", " + instructorsResult.Message);
            return RedirectToAction("Index");
        }
        model.GenerateSelectLists(coursesResult.Data, instructorsResult.Data);
        
        if (!ModelState.IsValid)
        {
            foreach (var modelError in ModelState.Values)
            {
                foreach (var error in modelError.Errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }
            return View(model);
        }

        var section = model.Form.ToEntity();
        var addResult = _sectionService.AddSection(section);
        if (!addResult.Ok)
        {
            var errMsg = "There was an error adding new section.";
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            _logger.LogError(errMsg + ": " + addResult.Message);
            return View(model);
        }

        var successMsg = $"Section added successfully with id {section.SectionID}.";
        _logger.LogInformation(successMsg);
        var sucTempData = new TempDataExtension(true, successMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Index");
    }
}