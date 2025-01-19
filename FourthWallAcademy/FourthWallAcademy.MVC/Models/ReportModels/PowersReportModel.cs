using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class PowersReportModel
{
    public List<PowerTypeReport> Reports { get; set; } = new List<PowerTypeReport>();
    public PowerRatingsForm Form { get; set; } = new PowerRatingsForm();
    public SelectList? PowerTypes { get; set; }
    public SelectList OrderList { get; set; } = GenerateOrderList();

    private static SelectList GenerateOrderList()
    {
        var selectItems = new List<SelectListItem>
        {
            new SelectListItem("Power", "1"),
            new SelectListItem("Min Rating", "2"),
            new SelectListItem("Avg Rating", "3"),
            new SelectListItem("Max Rating", "4"),
        };
        return new SelectList(selectItems, "Value", "Text");
    }
}

public class PowerRatingsForm
{
    public PowerRatingsOrder Order { get; set; } = PowerRatingsOrder.Power;
    [Display(Name = "Power Type")]
    public int PowerTypeID { get; set; } = 0;
    public string SearchString { get; set; } = "";
}

public enum PowerRatingsOrder
{
    Power = 1,
    MinRating,
    AvgRating,
    MaxRating,
}