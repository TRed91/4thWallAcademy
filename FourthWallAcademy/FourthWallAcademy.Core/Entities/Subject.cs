namespace FourthWallAcademy.Core.Entities;

public class Subject
{
    public int SubjectID { get; set; }
    public string SubjectName { get; set; }
    
    public List<Course> Courses { get; set; } = new();
}