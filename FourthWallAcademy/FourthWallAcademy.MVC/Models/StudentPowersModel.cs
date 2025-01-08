using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models;

public class StudentPowersModel
{
    public StudentPowersForm? Form { get; set; }
    public List<StudentPower>? Powers { get; set; }
    public SelectList? PowersSelectList { get; set; }
    public string? StudentAlias  { get; set; }
    public int? StudentID { get; set; }
}

public class StudentPowersForm
{
    [Required]
    public int PowerID { get; set; }
    [Required]
    [Range(1, byte.MaxValue)]
    public int Rating { get; set; }
}