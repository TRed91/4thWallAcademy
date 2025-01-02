namespace FourthWallAcademy.Core.Entities;

public class StudentSection
{
    public int StudentID { get; set; }
    public int SectionID { get; set; }
    public byte Grade { get; set; }
    public byte Absences { get; set; }
    
    public Student Student { get; set; }
    public Section Section { get; set; }
}