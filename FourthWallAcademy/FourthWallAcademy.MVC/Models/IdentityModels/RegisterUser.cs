using System.ComponentModel.DataAnnotations;

namespace FourthWallAcademy.MVC.Models.IdentityModels;

public class RegisterUser : IValidatableObject
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.";
        if (Username.Any(c => !allowedChars.Contains(c)))
        {
            errors.Add(new ValidationResult("Username contains invalid characters", ["Username"]));
        }
        
        return errors;
    }
}