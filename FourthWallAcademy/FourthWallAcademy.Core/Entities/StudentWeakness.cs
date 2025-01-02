namespace FourthWallAcademy.Core.Entities;

public class StudentWeakness
{
    public int StudentID { get; set; }
    public int WeaknessID { get; set; }
    public byte RiskLevel { get; set; }
    
    public Student Student { get; set; }
    public Weakness Weakness { get; set; }
}