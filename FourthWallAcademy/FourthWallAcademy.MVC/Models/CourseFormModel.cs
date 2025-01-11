using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models;

public class CourseFormModel
{
    public CourseForm? Form { get; set; }
    public SelectList? SubjectList { get; set; }
}

public class CourseForm : IValidatableObject
{
    [Required]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; }
    
    [Required]
    [Display(Name = "Course Description")]
    public string CourseDescription { get; set; }
    
    [Required]
    [Range(1, 3)]
    public decimal Credits { get; set; }
    
    [Required]
    public int SubjectId { get; set; }

    public CourseForm() { }

    public CourseForm(Course entity)
    {
        CourseName = entity.CourseName;
        CourseDescription = entity.CourseDescription;
        Credits = entity.Credits;
        SubjectId = entity.Subject.SubjectID;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        
        if (CourseName.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            errors.Add(new ValidationResult("Course Name contains invalid characters.", ["CourseName"]));
        }
        
        return errors;
    }

    public Course ToEntity()
    {
        return new Course
        {
            CourseName = CourseName,
            CourseDescription = CourseDescription,
            Credits = Credits,
            SubjectID = SubjectId
        };
    }
}