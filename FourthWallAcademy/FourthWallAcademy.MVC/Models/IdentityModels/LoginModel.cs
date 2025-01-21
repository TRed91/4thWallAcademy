using System.ComponentModel.DataAnnotations;

namespace FourthWallAcademy.MVC.Models.IdentityModels;

public class LoginModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}