using System.ComponentModel.DataAnnotations;

namespace FourthWallAcademy.API.Models;

public class LoginModel
{
    [Required]
    public string Alias { get; set; }
    [Required]
    public string Password { get; set; }
}