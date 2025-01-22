using Microsoft.AspNetCore.Identity;

namespace FourthWallAcademy.MVC.db.Entities;

public class ApplicationUser : IdentityUser
{
    public int StudentID { get; set; }
}