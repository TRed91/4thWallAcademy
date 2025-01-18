namespace FourthWallAcademy.Core.Models;

public class EnrollmentReport
{
    public int CountStudents { get; set; }
    public int CountSections { get; set; }
    public int CountEnrollments { get; set; }
    public int SumAbsences { get; set; }
    
    public List<SectionEnrollment> SectionEnrollments { get; set; }
    public List<StudentEnrollment> StudentEnrollments { get; set; }
}

public class SectionEnrollment
{
    public string CourseName { get; set; }
    public string InstructorAlias { get; set; }
    public int StudentCount { get; set; }
    public int Absences { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class StudentEnrollment
{
    public string StudentAlias { get; set; }
    public int SectionsCount { get; set; }
    public int Absences { get; set; }
}