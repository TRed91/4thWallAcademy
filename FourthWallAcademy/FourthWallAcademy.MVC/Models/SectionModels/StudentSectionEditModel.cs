using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.SectionModels;

public class StudentSectionEditModel
{
    public string CourseName { get; set; }
    public string StudentAlias{ get; set; }
    public int SectionId { get; set; }
    public int StudentId { get; set; }
    public StudentSectionForm Form { get; set; }
}

public class StudentSectionForm
{
    [Required]
    [Range(0, byte.MaxValue)]
    public byte Absences { get; set; }
    [Required]
    [Range(0, byte.MaxValue)]
    public byte Grade { get; set; }
}