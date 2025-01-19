using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class WeaknessReportModel
{
    public List<WeaknessTypeReport> Reports { get; set; } = new List<WeaknessTypeReport>();
    public WeaknessReportForm Form { get; set; } = new WeaknessReportForm();
    public SelectList? WeaknessTypes { get; set; }
    public SelectList OrderList { get; set; } = GenerateOrderList();

    private static SelectList GenerateOrderList()
    {
        var selectItems = new List<SelectListItem>
        {
            new SelectListItem("Weakness", "1"),
            new SelectListItem("Min Risk", "2"),
            new SelectListItem("Avg Risk", "3"),
            new SelectListItem("Max Risk", "4"),
        };
        return new SelectList(selectItems, "Value", "Text");
    }
}

public class WeaknessReportForm
{
    public WeaknessReportOrder Order { get; set; } = WeaknessReportOrder.Weakness;
    [Display(Name = "Weakness Type")]
    public int WeaknessTypeID { get; set; } = 0;
    public string SearchString { get; set; } = "";
}

public enum WeaknessReportOrder
{
    Weakness = 1,
    MinRisk,
    AvgRisk,
    MaxRisk,
}