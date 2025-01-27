using System.ComponentModel.DataAnnotations;

namespace FourthWallAcademy.MVC.Models.StudentModels;

public class StudentWeaknessEditModel
{
    public string Alias { get; set; }
    public string WeaknessName { get; set; }
    public int StudentId { get; set; }
    public int WeaknessId { get; set; }
    
    [Required]
    [Range(1, 10)]
    [Display(Name = "Risk Level")]
    public byte RiskLevel { get; set; }
}