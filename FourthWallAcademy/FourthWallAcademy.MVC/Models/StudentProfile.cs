using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models;

public class StudentProfile
{
    public Student Student { get; set; }
    public List<Section> Sections { get; set; }
}