namespace FourthWallAcademy.Core.Entities;

public class Power
{
    public int PowerID { get; set; }
    public int PowerTypeID { get; set; }
    public string PowerName { get; set; }
    public string PowerDescription { get; set; }
    
    public List<StudentPower> StudentPowers { get; set; } = new ();
    public PowerType PowerType { get; set; }
}