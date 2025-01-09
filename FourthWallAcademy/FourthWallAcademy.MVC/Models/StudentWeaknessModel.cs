using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models;

public class StudentWeaknessModel
{
    public StudentWeaknessForm? Form { get; set; }
    public List<StudentWeakness>? Weaknesses { get; set; }
    public SelectList? WeaknessesSelectList { get; set; }
    public int? StudentID { get; set; }
    public string? StudentAlias  { get; set; }
}

public class StudentWeaknessForm
{
    [Required]
    public int WeaknessID { get; set; }
    [Required]
    [Range(1, 10)]
    [Display(Name = "Risk Lv.(1-10)")]
    public int RiskLevel { get; set; }
}