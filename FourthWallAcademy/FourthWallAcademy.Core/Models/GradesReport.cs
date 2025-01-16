namespace FourthWallAcademy.Core.Models;

public class GradesReport
{
    public int MinGrade { get; set; }
    public int MaxGrade { get; set; }
    public int AvgGrade { get; set; }
    
    public List<StudentGrades> StudentGrades { get; set; }
}

public class StudentGrades
{
    public int StudentMinGrade { get; set; }
    public int StudentMaxGrade { get; set; }
    public int StudentAvgGrade { get; set; }
    public string Alias  { get; set; }
}