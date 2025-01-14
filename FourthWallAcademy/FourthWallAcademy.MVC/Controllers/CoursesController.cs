using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.Models.CourseModels;
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

    public IActionResult Index(CoursesIndexModel indexModel)
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
        if (!string.IsNullOrEmpty(indexModel.FilterForm.SearchString))
        {
            courses = courses.Where(c => c.CourseName.Contains(indexModel.FilterForm.SearchString) || 
                                         c.CourseDescription.Contains(indexModel.FilterForm.SearchString))
                .ToList();
        }
        
        // Order by Order selection
        if (indexModel.FilterForm.Order == CoursesOrder.Course)
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
        if (indexModel.FilterForm.SubjectId == 0)
        {
            subjects = subjects.Where(s => s.Courses.Count > 0).ToList();
        }
        else
        {
            subjects = subjects.Where(s => s.SubjectID == indexModel.FilterForm.SubjectId).ToList();
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
        indexModel = new CoursesIndexModel
        {
            FilterForm = new CoursesFilterForm(),
            OrderSelectList = new SelectList(orderSL, "Value", "Text"),
            SubjectSelectList = new SelectList(subjectSL, "Value", "Text"),
            Subjects = subjects
        };
        
        return View(indexModel);
    }

    public IActionResult Details(int id)
    {
        var courseResult = _courseService.GetCourseById(id);

        if (!courseResult.Ok)
        {
            var msg = $"Error fetching course details: {courseResult.Message}";
            _logger.LogError(msg);
            var temp = new TempDataExtension(false, msg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }

        var model = new CourseDetailsModel(courseResult.Data);
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Add()
    {
        var subjectResult = _courseService.GetSubjects();

        if (!subjectResult.Ok)
        {
            var msg = $"Error fetching subjects: {subjectResult.Message}";
            _logger.LogError(msg);
            var temp = new TempDataExtension(false, msg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }

        var subjectsSelectItems = subjectResult.Data.Select(s => new SelectListItem
        {
            Text = s.SubjectName,
            Value = s.SubjectID.ToString()
        });

        var model = new CourseFormModel
        {
            Form = new CourseForm(),
            SubjectList = new SelectList(subjectsSelectItems, "Value", "Text")
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(CourseFormModel model)
    {
        var subjectResult = _courseService.GetSubjects();

        if (!subjectResult.Ok)
        {
            var errMsg = $"Error fetching subjects: {subjectResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }

        var subjectsSelectItems = subjectResult.Data.Select(s => new SelectListItem
        {
            Text = s.SubjectName,
            Value = s.SubjectID.ToString()
        });
        
        model.SubjectList = new SelectList(subjectsSelectItems, "Value", "Text");
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var course = model.Form.ToEntity();
        var addResult = _courseService.AddCourse(course);
        if (!addResult.Ok)
        {
            var errMsg = $"Error adding course: {addResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return View(model);
        }
        
        var msg = $"Course added with id: {course.CourseID}";
        _logger.LogInformation(msg);
        var successMsg = new TempDataExtension(true, msg);
        TempData["Message"] = TempDataSerializer.Serialize(successMsg);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var courseResult = _courseService.GetCourseById(id);
        var subjectResult = _courseService.GetSubjects();
        
        if (!courseResult.Ok)
        {
            var errMsg = $"Error fetching course: {courseResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }
        if (!subjectResult.Ok)
        {
            var errMsg = $"Error fetching subjects: {subjectResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }

        var subjectsSelectItems = subjectResult.Data.Select(s => new SelectListItem
        {
            Text = s.SubjectName,
            Value = s.SubjectID.ToString(),
        }).ToList();
        
        var model = new CourseFormModel
        {
            SubjectList = new SelectList(subjectsSelectItems, "Value", "Text"),
            Form = new CourseForm(courseResult.Data)
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CourseFormModel model)
    {
        var subjectResult = _courseService.GetSubjects();

        if (!subjectResult.Ok)
        {
            var errMsg = $"Error fetching subjects: {subjectResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Index");
        }

        var subjectsSelectItems = subjectResult.Data.Select(s => new SelectListItem
        {
            Text = s.SubjectName,
            Value = s.SubjectID.ToString()
        });
        
        model.SubjectList = new SelectList(subjectsSelectItems, "Value", "Text");
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var course = model.Form.ToEntity();
        course.CourseID = id;
        var updateResult = _courseService.UpdateCourse(course);
        if (!updateResult.Ok)
        {
            var errMsg = $"Error updating course: {updateResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return View(model);
        }
        
        _logger.LogInformation($"Course updated with id: {id}");
        var successMsg = new TempDataExtension(true, $"Course updated with id: {id}");
        TempData["Message"] = TempDataSerializer.Serialize(successMsg);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var courseResult = _courseService.GetCourseById(id);
        if (!courseResult.Ok)
        {
            var errMsg = $"Error fetching course: {courseResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Details", new { id });
        }
        
        return View(new CourseDetailsModel(courseResult.Data));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteCourse(int id)
    {
        var deleteResult = _courseService.DeleteCourse(id);

        if (!deleteResult.Ok)
        {
            var errMsg = $"Error deleting course with {id}: {deleteResult.Message}";
            _logger.LogError(errMsg);
            var temp = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(temp);
            return RedirectToAction("Delete", new { id });
        }

        var msg = $"Course deleted with id: {id}";
        _logger.LogInformation(msg);
        var successMsg = new TempDataExtension(true, msg);
        TempData["Message"] = TempDataSerializer.Serialize(successMsg);
        return RedirectToAction("Index");
    }
}