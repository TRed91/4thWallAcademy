using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.StudentModels;

public class StudentPowerEditModel
{
    public string Alias  { get; set; }
    public string PowerName { get; set; }
    public int StudentId { get; set; }
    public int PowerId { get; set; }
    
    [Required]
    [Range(1, 100)]
    public byte Rating { get; set; }
}