using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class SectionsReportModel
{
    public EnrollmentReport Report { get; set; }
    public SectionsReportForm Form { get; set; } = new SectionsReportForm();
    public SelectList OrderList { get; set; } = GenerateSelectList();

    private static SelectList GenerateSelectList()
    {
        var listItems = new List<SelectListItem>
        {
            new SelectListItem("Course", "1"),
            new SelectListItem("Instructor", "2"),
            new SelectListItem("Students Asc", "3"),
            new SelectListItem("Students Desc", "4"),
            new SelectListItem("Absences Asc", "5"),
            new SelectListItem("Absences Desc", "6"),
            new SelectListItem("Start Date Asc", "7"),
            new SelectListItem("Start Date Desc", "8"),
            new SelectListItem("End Date Asc", "9"),
            new SelectListItem("End Date Desc", "10"),
        };
        return new SelectList(listItems, "Value", "Text");
    }
}

public class SectionsReportForm
{
    public SectionsReportOrder Order { get; set; } = SectionsReportOrder.Course;
    [Display(Name = "From")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = new DateTime(1990, 1, 1);
    [Display(Name = "To")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;
}

public enum SectionsReportOrder
{
    Course = 1,
    Instructor,
    StudentCountAsc,
    StudentCountDesc,
    AbsencesAsc,
    AbsencesDesc,
    StartDateAsc,
    StartDateDesc,
    EndDateAsc,
    EndDateDesc,
}