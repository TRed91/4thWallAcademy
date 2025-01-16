using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.InstructorsModels;

public class InstructorTerminateModel : IValidatableObject
{
    public Instructor? Instructor { get; set; }
    
    [Required]
    [Display(Name = "Termination Date")]
    [DataType(DataType.Date)]
    public DateTime TerminationDate { get; set; } = DateTime.Today;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (TerminationDate < DateTime.Today)
        {
            errors.Add(new ValidationResult("Termination Date can't be in the past.", ["TerminationDate"]));
        }
        return errors;
    }
}