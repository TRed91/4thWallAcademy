namespace FourthWallAcademy.Core.Entities;

public class Weakness
{
    public int WeaknessID { get; set; }
    public int WeaknessTypeID { get; set; }
    public string WeaknessName { get; set; }
    public string WeaknessDescription { get; set; }
    
    public List<StudentWeakness> StudentWeaknesses { get; set; } = new ();
    public WeaknessType WeaknessType { get; set; }
}