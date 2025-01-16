namespace FourthWallAcademy.Core.Models;

public class EnrollmentsDashboard
{
    public int NumStudents { get; set; }
    public int NumSections { get; set; }
    public int NumEnrollments { get; set; }
    public int NumAbsences { get; set; }
}

public class SectionEnrollments
{
    public int TotalEnrollments { get; set; }
    public int TotalAbsences { get; set; }
}