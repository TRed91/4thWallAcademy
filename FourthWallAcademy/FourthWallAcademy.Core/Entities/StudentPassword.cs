namespace FourthWallAcademy.Core.Entities;

public class StudentPassword
{
    public int PasswordID { get; set; }
    public int StudentID { get; set; }
    public string Password { get; set; }
    
    public Student Student { get; set; }
}