namespace FourthWallAcademy.Core.Entities;

public class WeaknessType
{
    public int WeaknessTypeID { get; set; }
    public string WeaknessTypeName { get; set; }
    public string WeaknessTypeDescription { get; set; }
    
    public List<Weakness> Weaknesses { get; set; }
}