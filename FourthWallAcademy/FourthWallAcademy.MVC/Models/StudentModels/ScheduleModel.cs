using System.ComponentModel.DataAnnotations;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.MVC.Models.StudentModels;

public class ScheduleModel
{
    public StudentSchedule? Schedule { get; set; }
    public ScheduleForm Form { get; set; }
}

public class ScheduleForm
{
    [DataType(DataType.Date)]
    public DateTime From { get; set; } = DateTime.Now;
    [DataType(DataType.Date)]
    public DateTime To { get; set; } = DateTime.Now.AddDays(7);
}