using Microsoft.AspNetCore.Identity;

namespace FourthWallAcademy.MVC.db.Entities;

public class IdentityStudent : IdentityUser
{
    public int StudentID { get; set; }
}