namespace FourthWallAcademy.Core.Entities;

public class Section
{
    public int SectionID { get; set; }
    public int CourseID { get; set; }
    public int InstructorID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public List<StudentSection> StudentSections { get; set; }
    public Instructor Instructor { get; set; }
    public Course Course { get; set; }
}