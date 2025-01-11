using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models;

public class CoursesIndexModel
{
    public CoursesFilterForm FilterForm { get; set; } = new CoursesFilterForm();
    public SelectList OrderSelectList { get; set; }
    public SelectList SubjectSelectList { get; set; }
    public List<Subject> Subjects { get; set; } = new List<Subject>();
}

public class CoursesFilterForm
{
    public CoursesOrder Order { get; set; } = CoursesOrder.Course;
    [Display(Name = "Subject")]
    public int SubjectId { get; set; } = 0;
    public string SearchString { get; set; } = "";
}

public enum CoursesOrder
{
    Course = 1,
    Credits
}