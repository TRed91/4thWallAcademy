using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.SectionModels;

public class SectionDetailsModel
{
    public Section Section { get; set; }
    public int DeleteStudentId { get; set; }
}