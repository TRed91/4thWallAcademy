namespace FourthWallAcademy.Core.Entities;

public class PowerType
{
    public int PowerTypeID { get; set; }
    public string PowerTypeName { get; set; }
    public string PowerTypeDescription { get; set; }
    
    public List<Power> Powers { get; set; } = new ();
}