using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models;

public class StudentFormModel : IValidatableObject
{
    public StudentFormModel() { }

    public StudentFormModel(Student entity)
    {
        Alias = entity.Alias;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
        DoB = entity.DoB;
    }
    
    [Required]
    public string Alias { get; set; }
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    [Required]
    [DataType(DataType.Date)]  
    public DateTime DoB { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (DateTime.Now.AddYears(-14) < DoB)
        {
            errors.Add(new ValidationResult("Student must be at least 14 years old.", ["DoB"]));
        }

        if (Alias.Any(c => !char.IsLetter(c)))
        {
            errors.Add(new ValidationResult("Alias must only contain letters.", ["Alias"]));
        }
        if (FirstName.Any(c => !char.IsLetter(c)))
        {
            errors.Add(new ValidationResult("First Name must only contain letters.", ["FirstName"]));
        }

        if (LastName.Any(c => !char.IsLetter(c)))
        {
            errors.Add(new ValidationResult("Last Name must only contain letters.", ["LastName"]));
        }
        
        return errors;
    }

    public Student ToEntity()
    {
        return new Student
        {
            Alias = Alias,
            FirstName = FirstName,
            LastName = LastName,
            DoB = DoB,
        };
    }
}