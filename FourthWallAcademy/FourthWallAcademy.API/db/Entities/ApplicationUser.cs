using Microsoft.AspNetCore.Identity;

namespace FourthWallAcademy.API.db.Entities;

public class ApplicationUser : IdentityUser
{
    public int StudentID { get; set; }
}