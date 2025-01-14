using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.MVC.Views.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.SectionModels;

public class SectionEnrollModel
{
    public int SectionId { get; set; }
    public int StudentId { get; set; }
    public List<Student> Students { get; set; }
    public SectionEnrollForm Form { get; set; } = new SectionEnrollForm();
    
    public SelectList OrderSelectList { get; set; }
    public SelectList LetterSelectList { get; set; }

    public SectionEnrollModel()
    {
        OrderSelectList = new SelectList(new List<SelectListItem>
        {
            new SelectListItem("Name", "1"),
            new SelectListItem("Birth Date", "2"),
        }, "Value", "Text");
        LetterSelectList = SelectListFactory.CreateAlphabetSelectList();
    }
}

public class SectionEnrollForm
{
    public OrderStudent Order { get; set; } = OrderStudent.Name;
    public string SearchString { get; set; } = "";
    [Display(Name = "Starts With")]
    public char StartLetter { get; set; } = 'A';
}

public enum OrderStudent
{
    Name = 1,
    BirthDate,
}