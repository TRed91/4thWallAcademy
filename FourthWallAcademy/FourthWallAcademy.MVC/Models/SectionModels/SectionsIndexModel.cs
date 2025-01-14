using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.SectionModels;

public class SectionsIndexModel
{
    public List<Course> Courses { get; set; }
    public SectionsFilterForm Form { get; set; } = new SectionsFilterForm();
    public SelectList OrderSelectList { get; set; }
    public SelectList CourseSelectList { get; set; }

    public void GenerateSelectLists(List<Course> courses, SectionsOrderOptions selected = SectionsOrderOptions.StartDate)
    {
        var listItems = new List<SelectListItem>
        {
            new SelectListItem("Start Date", "2", selected == SectionsOrderOptions.StartDate),
            new SelectListItem("End Date", "3", selected == SectionsOrderOptions.EndDate),
            new SelectListItem("Start Time", "4", selected == SectionsOrderOptions.StartTime),
            new SelectListItem("Instructor", "1", selected == SectionsOrderOptions.Instructor),
        };
        OrderSelectList = new SelectList(listItems, "Value", "Text");
        
        CourseSelectList = new SelectList(courses, "CourseID", "CourseName");
    }
}

public class SectionsFilterForm
{
    public SectionsOrderOptions Order { get; set; } = SectionsOrderOptions.StartDate;
    
    [Display(Name = "Course")]
    public int CourseID { get; set; } = 0;
}

public enum SectionsOrderOptions
{
    Instructor = 1,
    StartDate,
    EndDate,
    StartTime
}