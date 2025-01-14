using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.StudentModels;

public class AdmissionsStudentsModel
{
    public AdmissionsStudentsForm Form { get; set; }
    public List<Student> Students { get; set; }
    public SelectList OrderList { get; set; }
    public SelectList LetterList { get; set; }
}

public class AdmissionsStudentsForm
{
    [Display(Name = "Starts With")]
    public char startsWith { get; set; } = 'A';
    public string searchString { get; set; } = "";
    public AdmissionsStudentsOrder Order { get; set; } = AdmissionsStudentsOrder.Name;
}

public enum AdmissionsStudentsOrder
{
    Name = 1,
    BirthDate,
}