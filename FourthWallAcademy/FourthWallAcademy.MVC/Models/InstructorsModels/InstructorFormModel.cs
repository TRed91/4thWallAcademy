using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.InstructorsModels;

public class InstructorFormModel : IValidatableObject
{
    [Required]
    public string Alias { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ. ";
        if (Alias.Any(c => !allowedChars.Contains(c)))
        {
            errors.Add(new ValidationResult("Alias contains invalid characters.", ["Alias"]));
        }
        return errors;
    }

    public Instructor ToEntity()
    {
        return new Instructor
        {
            Alias = Alias,
        };
    }
}