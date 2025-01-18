using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class StudentsReportModel
{
    public EnrollmentReport Report { get; set; }
    public StudentsReportForm Form { get; set; } = new StudentsReportForm();
    public SelectList OrderList { get; set; } = GenerateSelectList();

    private static SelectList GenerateSelectList()
    {
        var listItems = new List<SelectListItem>
        {
            new SelectListItem("Student", "1"),
            new SelectListItem("Enrollments Asc", "2"),
            new SelectListItem("Enrollments Desc", "3"),
            new SelectListItem("Absences Asc", "4"),
            new SelectListItem("Absences Desc", "5"),
        };
        return new SelectList(listItems, "Value", "Text");
    }
}

public class StudentsReportForm
{
    public StudentsReportOrder Order { get; set; } = StudentsReportOrder.Student;
    [Display(Name = "From")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = new DateTime(1990, 1, 1);
    [Display(Name = "To")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;
}

public enum StudentsReportOrder
{
    Student = 1,
    EnrollmentsAsc,
    EnrollmentsDesc,
    AbsencesAsc,
    AbsencesDesc,
}