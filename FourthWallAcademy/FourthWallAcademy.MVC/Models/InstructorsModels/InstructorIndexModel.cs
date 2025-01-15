using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.InstructorsModels;

public class InstructorIndexModel
{
    public List<Instructor> Instructors { get; set; }
    public InstructorIndexForm Form { get; set; } = new InstructorIndexForm();
    public SelectList OrderSelectList { get; set; }

    public void GenerateSelectList()
    {
        var slItems = new List<SelectListItem>
        {
            new SelectListItem("Alias", "1"),
            new SelectListItem("Hire Date", "2"),
            new SelectListItem("Termination Date", "3"),
        };
        OrderSelectList = new SelectList(slItems, "Value", "Text");
    }
}

public class InstructorIndexForm
{
    public InstructorIndexOrder Order { get; set; } = InstructorIndexOrder.Alias;
    public string SearchString { get; set; } = "";
    
    [Display(Name = "Show Terminated")]
    public bool ShowTerminated { get; set; } = false;
}

public enum InstructorIndexOrder
{
    Alias = 1,
    HireDate,
    TermDate
}