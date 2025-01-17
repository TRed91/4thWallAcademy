using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.MVC.Models.ReportModels;

public class DashboardModel
{
    public GradesReport GradesReport { get; set; }
    public EnrollmentReport EnrollmentReport { get; set; }
    public PowersReport PowersReport { get; set; }
    public WeaknessesReport WeaknessesReport { get; set; }
}