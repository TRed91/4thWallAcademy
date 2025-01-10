using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.UtilityClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

public class CoursesController : Controller
{
    private readonly ILogger _logger;
    private readonly ICourseService _courseService;

    public CoursesController(ILogger<CoursesController> logger, ICourseService courseService)
    {
        _logger = logger;
        _courseService = courseService;
    }

    public IActionResult Index(CoursesModel model)
    {
        // fetch courses and subjects data
        var coursesResult = _courseService.GetCourses();
        var subjectsResult = _courseService.GetSubjects();
        string errMsg = "";
        if (!coursesResult.Ok)
        {
            errMsg = $"Error fetching courses: {coursesResult.Message}";
            _logger.LogError(errMsg);
        }
        if (!subjectsResult.Ok)
        {
            errMsg = $"Error fetching subjects: {subjectsResult.Message}";
            _logger.LogError(errMsg);
        }
        if (!coursesResult.Ok || !subjectsResult.Ok)
        {
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index","Management");
        }

        // Filter by SearchString
        var courses = coursesResult.Data;
        if (!string.IsNullOrEmpty(model.Form.SearchString))
        {
            courses = courses.Where(c => c.CourseName.Contains(model.Form.SearchString) || 
                                         c.CourseDescription.Contains(model.Form.SearchString))
                .ToList();
        }
        
        // Order by Order selection
        if (model.Form.Order == CoursesOrder.Course)
        {
            courses = courses
                .OrderBy(c => c.CourseName)
                .ToList();
        }
        else
        {
            courses = courses
                .OrderByDescending(c => c.Credits)
                .ToList();
        }

        // Add courses to subjects
        var subjects = subjectsResult.Data;
        foreach (var subject in subjects)
        {
            foreach (var course in courses)
            {
                if (subject.SubjectID == course.SubjectID)
                {
                    subject.Courses.Add(course);
                }
            }
        }

        // Filter out subjects without courses or without correct subjectId
        if (model.Form.SubjectId == 0)
        {
            subjects = subjects.Where(s => s.Courses.Count > 0).ToList();
        }
        else
        {
            subjects = subjects.Where(s => s.SubjectID == model.Form.SubjectId).ToList();
        }
        

        // Generate SelectLists
        var orderSL = new List<SelectListItem>
        {
            new SelectListItem("Course", "1"),
            new SelectListItem("Credits", "2"),
        };

        var subjectSL = subjectsResult.Data.OrderBy(s => s.SubjectName)
            .Select(s => new SelectListItem
            {
                Text = s.SubjectName,
                Value = s.SubjectID.ToString()
            });

        // update the model
        model = new CoursesModel
        {
            Form = new CoursesForm(),
            OrderSelectList = new SelectList(orderSL, "Value", "Text"),
            SubjectSelectList = new SelectList(subjectSL, "Value", "Text"),
            Subjects = subjects
        };
        
        return View(model);
    }
}