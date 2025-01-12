using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models;

public class SectionsFormModel
{
    public SelectList? Courses { get; set; }
    public SelectList? Instructors { get; set; }
    public SectionForm Form { get; set; } = new SectionForm();

    public void GenerateSelectLists(List<Course> courses, List<Instructor> instructors)
    {
        Courses = new SelectList(courses, "CourseID", "CourseName");
        Instructors = new SelectList(instructors, "InstructorID", "Alias");
    }
}

public class SectionForm : IValidatableObject
{
    [Required(ErrorMessage = "Course is Required")]
    [Display(Name = "Course")]
    public int CourseId { get; set; }
    [Required(ErrorMessage = "Instructor is Required")]
    [Display(Name = "Instructor")]
    public int InstructorId { get; set; }
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Today;
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Start Time")]
    public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0, 0);

    public Section ToEntity()
    {
        return new Section
        {
            CourseID = CourseId,
            InstructorID = InstructorId,
            StartDate = StartDate,
            EndDate = EndDate,
            StartTime = StartTime.ToTimeSpan(),
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (StartDate > EndDate)
        {
            errors.Add(new ValidationResult("Start date must be before end date.", ["StartDate"]));
            errors.Add(new ValidationResult("End date must be after start date.", ["EndDate"]));
        }

        if (StartDate < DateTime.Today)
        {
            errors.Add(new ValidationResult("Start date can't be in the past.", ["StartDate"]));
        }

        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(15, 0);
        if (StartTime < startTime || StartTime > endTime)
        {
            errors.Add(new ValidationResult("Start Time must be after 9:00 and before 15:00.", ["StartTime"]));
        }

        if (InstructorId == 0)
        {
            errors.Add(new ValidationResult("Instructor is required.", ["InstructorID"]));
        }

        if (CourseId == 0)
        {
            errors.Add(new ValidationResult("Course is required.", ["CourseID"]));
        }
        
        return errors;
    }
}