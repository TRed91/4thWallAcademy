using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.MVC.Models;
using FourthWallAcademy.MVC.Models.SectionModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

public class SectionsController : Controller
{
    private readonly ILogger _logger;
    private readonly ISectionService _sectionService;
    private readonly ICourseService _courseService;
    private readonly IInstructorService _instructorService;
    private readonly IStudentService _studentService;

    public SectionsController(ILogger<SectionsController> logger, 
        ISectionService sectionService, 
        ICourseService courseService,
        IInstructorService instructorService,
        IStudentService studentService)
    {
        _logger = logger;
        _sectionService = sectionService;
        _courseService = courseService;
        _instructorService = instructorService;
        _studentService = studentService;
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

    public IActionResult Details(int id)
    {
        var sectionsResult = _sectionService.GetSectionById(id);
        var studentSectionsResult = _sectionService.GetStudentsBySection(id);

        if (!sectionsResult.Ok || !studentSectionsResult.Ok)
        {
            var errMsg = "There was an error retrieving the section details.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + sectionsResult.Message + ", " + studentSectionsResult.Message);
            return RedirectToAction("Index");
        }

        sectionsResult.Data.StudentSections = studentSectionsResult.Data;

        var model = new SectionDetailsModel
        {
            Section = sectionsResult.Data,
        };
        
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
    [ValidateAntiForgeryToken]
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

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var sectionsResult = _sectionService.GetSectionById(id);
        var instructorsResult = _instructorService.GetInstructors();
        var coursesResult = _courseService.GetCourses();
        if (!sectionsResult.Ok)
        {
            var errMsg = "There was an error retrieving the section details.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + sectionsResult.Message);
            return RedirectToAction("Index");
        }
        
        var model = new SectionsFormModel();
        model.GenerateSelectLists(coursesResult.Data, instructorsResult.Data);
        model.Form = new SectionForm(sectionsResult.Data);
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, SectionsFormModel model)
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
            return View(model);
        }
        
        var section = model.Form.ToEntity();
        section.SectionID = id;
        var editResult = _sectionService.UpdateSection(section);
        if (!editResult.Ok)
        {
            var errMsg = $"There was an error editing section with id: {id}.";
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            _logger.LogError(errMsg + ": " + editResult.Message);
            return View(model);
        }
        
        var successMsg = $"Section with id '{id}' updated successfully.";
        _logger.LogInformation(successMsg);
        var sucTempData = new TempDataExtension(true, successMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    public IActionResult Enroll(int id, SectionEnrollModel model)
    {
        // fetch a list of all students and enrolled students then filter out students that aren't already enrolled
        Result<List<Student>> studentsResult;
        if (model.Form.StartLetter != '0')
        {
            studentsResult = _studentService.GetStudentsByStartingLetter(model.Form.StartLetter);
        }
        else
        {
            studentsResult = _studentService.GetStudents();
        }
        var studentSectionsResult = _sectionService.GetStudentsBySection(id);

        if (!studentsResult.Ok || !studentSectionsResult.Ok)
        {
            var errMsg = "There was an error retrieving student list.";
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            _logger.LogError(errMsg + ": " + studentsResult.Message + ", " + studentSectionsResult.Message);
            return RedirectToAction("Details", new { id });
        }
        
        var enrolledStudentIds = studentSectionsResult.Data
            .Select(s => s.StudentID)
            .ToList();
        
        var availableStudents = studentsResult.Data
            .Where(s => !enrolledStudentIds.Contains(s.StudentID))
            .ToList();

        // Order the list base on the Form
        if (model.Form.Order == OrderStudent.Name)
        {
            availableStudents = availableStudents.OrderBy(s => s.Alias).ToList();
        }
        else
        {
            availableStudents = availableStudents.OrderByDescending(s => s.DoB).ToList();
        }
        
        // Filter the list based on the searchstring
        if (!string.IsNullOrEmpty(model.Form.SearchString))
        {
            availableStudents = availableStudents
                .Where(s => s.Alias.Contains(model.Form.SearchString))
                .ToList();
        }
        
        model.Students = availableStudents;
        model.SectionId = id;
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EnrollStudent(int id, SectionEnrollModel model)
    {
        var studentSection = new StudentSection
        {
            StudentID = model.StudentId,
            SectionID = id,
            Absences = 0,
            Grade = 0
        };
        var enrollResult = _sectionService.AddStudentSection(studentSection);
        if (!enrollResult.Ok)
        {
            var errMsg = "There was an error adding student section.";
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            _logger.LogError(errMsg + ": " + enrollResult.Message);
            return RedirectToAction("Enroll", new { id });
        }
        
        var sucMsg = "Student enrolled successfully.";
        _logger.LogInformation($"{sucMsg}: StudentID={model.StudentId}, SectionID={id}");
        var sucTempData = new TempDataExtension(true, sucMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Enroll", new { id });
    }

    [HttpGet]
    public IActionResult Remove(int sectionId, int studentId)
    {
        var studentResult = _studentService.GetStudentById(studentId);
        if (!studentResult.Ok)
        {
            var errMsg = $"Error fetching student information";
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            _logger.LogError(errMsg + ": " + studentResult.Message);
            return RedirectToAction("Details", new { id = sectionId });
        }

        var model = new StudentSectionRemoveModel
        {
            SectionId = sectionId,
            StudentId = studentId,
            StudentAlias = studentResult.Data.Alias
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(StudentSectionRemoveModel model)
    {
        var removeResult = _sectionService.DeleteStudentSection(model.StudentId, model.SectionId);
        if (!removeResult.Ok)
        {
            var errMsg = $"There was an error removing student with Id:{model.StudentId} from section with Id:{model.SectionId}.";
            var errTempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(errTempData);
            _logger.LogError(errMsg + ": " + removeResult.Message);
            return RedirectToAction("Details", new { id = model.SectionId });
        }
        
        var sucMsg = $"Student with Id:{model.StudentId} removed successfully from section with Id:{model.SectionId}.";
        _logger.LogInformation(sucMsg);
        var sucTempData = new TempDataExtension(true, sucMsg);
        TempData["Message"] = TempDataSerializer.Serialize(sucTempData);
        return RedirectToAction("Details", new { id = model.SectionId });
    }
}