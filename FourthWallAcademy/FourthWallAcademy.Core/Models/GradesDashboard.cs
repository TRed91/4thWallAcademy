namespace FourthWallAcademy.Core.Models;

public class GradesDashboard
{
    public int MinGrade { get; set; }
    public int MaxGrade { get; set; }
    public int AvgGrade { get; set; }
}

public class StudentGrades
{
    public int MinGrade { get; set; }
    public int MaxGrade { get; set; }
    public int AvgGrade { get; set; }
    public string StudentAlias  { get; set; }
}