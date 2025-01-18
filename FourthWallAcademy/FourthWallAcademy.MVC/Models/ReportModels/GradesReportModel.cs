using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class GradesReportModel
{
    public GradesReport GradesReport { get; set; }
    public GradesReportForm Form { get; set; } = new GradesReportForm();
    public SelectList OrderList { get; set; } = GenerateSelectList();

    private static SelectList GenerateSelectList()
    {
        var listItems = new List<SelectListItem>
        {
            new SelectListItem("Student", "1"),
            new SelectListItem("Min Grade", "2"),
            new SelectListItem("Avg Grade", "3"),
            new SelectListItem("Max Grade", "4"),
        };
        return new SelectList(listItems, "Value", "Text");
    }
}

public class GradesReportForm
{
    public GradesReportOrder Order { get; set; } = GradesReportOrder.Alias;
    
    [Display(Name = "From")]
    [DataType(DataType.Date)]
    public DateTime FromDate { get; set; } = new DateTime(1990, 1, 1);
    
    [Display(Name = "To")]
    [DataType(DataType.Date)]
    public DateTime ToDate { get; set; } = DateTime.Today;

    public string SearchString { get; set; } = "";
}

public enum GradesReportOrder
{
    Alias = 1,
    MinGrade,
    AvgGrade,
    MaxGrade,
}