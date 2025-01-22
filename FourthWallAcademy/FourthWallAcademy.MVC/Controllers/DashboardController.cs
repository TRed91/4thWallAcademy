using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Core.Models;
using FourthWallAcademy.MVC.Models.ReportModels;
using FourthWallAcademy.MVC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ILogger _logger;
    private readonly IStudentService _studentService;
    private readonly ISectionService _sectionService;
    private readonly IPowerService _powerService;
    private readonly IWeaknessService _weaknessService;

    public DashboardController(ILogger<DashboardController> logger, 
        IStudentService studentService, 
        ISectionService sectionService,
        IPowerService powerService,
        IWeaknessService weaknessService)
    {
        _logger = logger;
        _studentService = studentService;
        _sectionService = sectionService;
        _powerService = powerService;
        _weaknessService = weaknessService;
    }
    
    public IActionResult Index()
    {
        var startDate = new DateTime(1990, 1, 1);
        var endDate = new DateTime(9999, 12, 31);
        var gradesResult = _studentService.GetGradesReport(startDate, endDate);
        var enrollmentResult = _sectionService.GetEnrollmentReport(startDate, endDate);
        var powersResult = _powerService.GetPowersReport();
        var weaknessResult = _weaknessService.GetWeaknessReport();

        if (!gradesResult.Ok || !enrollmentResult.Ok || !powersResult.Ok || !weaknessResult.Ok)
        {
            var errMsg = "There was an error retrieving reports data.";
            _logger.LogError(errMsg + ": " + string.Join(", ", new string[]
            {
                gradesResult.Message, 
                enrollmentResult.Message, 
                powersResult.Message, 
                weaknessResult.Message
            }));
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            
            return RedirectToAction("Index", "Home");
        }

        var model = new DashboardModel
        {
            GradesReport = gradesResult.Data,
            EnrollmentReport = enrollmentResult.Data,
            PowersReport = powersResult.Data,
            WeaknessesReport = weaknessResult.Data,
        };
        
        return View(model);
    }

    public IActionResult Grades(GradesReportModel model)
    {
        // fetch data
        var gradesResult = _studentService.GetGradesReport(model.Form.FromDate, model.Form.ToDate);
        if (!gradesResult.Ok)
        {
            var errMsg = "There was an error retrieving reports data.";
            _logger.LogError(errMsg + ": " + gradesResult.Message);
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            return RedirectToAction("Index");
        }
        
        var report = gradesResult.Data;
        
        // filter by search string
        if (!string.IsNullOrEmpty(model.Form.SearchString) || !string.IsNullOrWhiteSpace(model.Form.SearchString))
        {
            var searchString = model.Form.SearchString.Trim().ToLower();
            report.StudentGrades = report.StudentGrades
                .Where(s => s.Alias.ToLower().Contains(searchString))
                .ToList();
        }
        
        // order list
        switch (model.Form.Order)
        {
            case GradesReportOrder.Alias:
                report.StudentGrades = report.StudentGrades
                    .OrderBy(s => s.Alias)
                    .ToList();
                break;
            case GradesReportOrder.MinGrade:
                report.StudentGrades = report.StudentGrades
                    .OrderBy(s => s.StudentMinGrade)
                    .ToList();
                break;
            case GradesReportOrder.MaxGrade:
                report.StudentGrades = report.StudentGrades
                    .OrderByDescending(s => s.StudentMaxGrade)
                    .ToList();
                break;
            case GradesReportOrder.AvgGrade:
                report.StudentGrades = report.StudentGrades
                    .OrderByDescending(s => s.StudentAvgGrade)
                    .ToList();
                break;
        }

        model.GradesReport = report;
        
        return View(model);
    }

    public IActionResult Sections(SectionsReportModel model)
    {
        var enrollmentResult = _sectionService.GetEnrollmentReport(model.Form.StartDate, model.Form.EndDate);
        if (!enrollmentResult.Ok)
        {
            var errMsg = "There was an error retrieving reports data.";
            _logger.LogError(errMsg + ": " + enrollmentResult.Message);
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            return RedirectToAction("Index");
        }
        var report = enrollmentResult.Data;

        switch (model.Form.Order)
        {
            case SectionsReportOrder.Instructor:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.InstructorAlias).ToList();
                break;
            case SectionsReportOrder.Course:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.CourseName).ToList();
                break;
            case SectionsReportOrder.StudentCountAsc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.StudentCount).ToList();
                break;
            case SectionsReportOrder.StudentCountDesc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderByDescending(s => s.StudentCount).ToList();
                break;
            case SectionsReportOrder.AbsencesAsc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.Absences).ToList();
                break;
            case SectionsReportOrder.AbsencesDesc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderByDescending(s => s.Absences).ToList();
                break;
            case SectionsReportOrder.StartDateAsc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.StartDate).ToList();
                break;
            case SectionsReportOrder.StartDateDesc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderByDescending(s => s.StartDate).ToList();
                break;
            case SectionsReportOrder.EndDateAsc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderBy(s => s.EndDate).ToList();
                break;
            case SectionsReportOrder.EndDateDesc:
                report.SectionEnrollments = report.SectionEnrollments
                    .OrderByDescending(s => s.EndDate).ToList();
                break;
        }
        
        model.Report = report;
        return View(model);
    }
    
    public IActionResult Students(StudentsReportModel model)
    {
        var enrollmentResult = _sectionService.GetEnrollmentReport(model.Form.StartDate, model.Form.EndDate);
        if (!enrollmentResult.Ok)
        {
            var errMsg = "There was an error retrieving reports data.";
            _logger.LogError(errMsg + ": " + enrollmentResult.Message);
            var tempData = new TempDataExtension(false, errMsg);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            return RedirectToAction("Index");
        }
        var report = enrollmentResult.Data;

        switch (model.Form.Order)
        {
            case StudentsReportOrder.Student:
                report.StudentEnrollments = report.StudentEnrollments
                    .OrderBy(s => s.StudentAlias).ToList();
                break;
            case StudentsReportOrder.EnrollmentsAsc:
                report.StudentEnrollments = report.StudentEnrollments
                    .OrderBy(s => s.SectionsCount).ToList();
                break;
            case StudentsReportOrder.AbsencesAsc:
                report.StudentEnrollments = report.StudentEnrollments
                    .OrderBy(s => s.Absences).ToList();
                break;
            case StudentsReportOrder.EnrollmentsDesc:
                report.StudentEnrollments = report.StudentEnrollments
                    .OrderByDescending(s => s.SectionsCount).ToList();
                break;
            case StudentsReportOrder.AbsencesDesc:
                report.StudentEnrollments = report.StudentEnrollments
                    .OrderByDescending(s => s.Absences).ToList();
                break;
        }
        
        model.Report = report;
        return View(model);
    }

    public IActionResult Powers(PowersReportModel model)
    {
        // Fetch Data
        var reportsListResult = _powerService.GetPowersReportList();
        if (!reportsListResult.Ok)
        {
            var errMsg = "There was an error retrieving power reports.";
            var tempData = new TempDataExtension(false, errMsg);
            _logger.LogError(errMsg + ": " + reportsListResult.Message);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            return RedirectToAction("Index");
        }

        var reportsList = reportsListResult.Data;
        
        // Filter by selected Power Type
        if (model.Form.PowerTypeID != 0)
        {
            reportsList = reportsList
                .Where(p => p.PowerTypeID == model.Form.PowerTypeID)
                .ToList();
        }

        // Apply Search String
        if (!string.IsNullOrEmpty(model.Form.SearchString) && 
            !string.IsNullOrWhiteSpace(model.Form.SearchString))
        {
            var search = model.Form.SearchString.ToLower().Trim();
            foreach (var powerType in reportsList)
            {
                powerType.PowerRatings = powerType.PowerRatings
                    .Where(r => r.PowerName.ToLower().Contains(search))
                    .ToList();
            }
            reportsList = reportsList.Where(r => r.PowerRatings.Count > 0).ToList();
        }

        // Order Reports
        foreach (var type in reportsList)
        {
            switch (model.Form.Order)
            {
                case PowerRatingsOrder.Power:
                    type.PowerRatings = type.PowerRatings.OrderBy(p => p.PowerName).ToList();
                    break;
                case PowerRatingsOrder.MinRating:
                    type.PowerRatings = type.PowerRatings.OrderByDescending(p => p.MinRating).ToList();
                    break;
                case PowerRatingsOrder.AvgRating:
                    type.PowerRatings = type.PowerRatings.OrderByDescending(p => p.AvgRating).ToList();
                    break;
                case PowerRatingsOrder.MaxRating:
                    type.PowerRatings = type.PowerRatings.OrderByDescending(p => p.MaxRating).ToList();
                    break;
            }
        }

        model.Reports = reportsList;
        model.PowerTypes = new SelectList(reportsListResult.Data, "PowerTypeID", "PowerTypeName");
        
        return View(model);
    }

    public IActionResult Weaknesses(WeaknessReportModel model)
    {
        // Fetch Data
        var reportsListResult = _weaknessService.GetWeaknessReportList();
        if (!reportsListResult.Ok)
        {
            var errMsg = "There was an error retrieving power reports.";
            var tempData = new TempDataExtension(false, errMsg);
            _logger.LogError(errMsg + ": " + reportsListResult.Message);
            TempData["Message"] = TempDataSerializer.Serialize(tempData);
            return RedirectToAction("Index");
        }

        var reportsList = reportsListResult.Data;
        
        // Filter by selected Power Type
        if (model.Form.WeaknessTypeID != 0)
        {
            reportsList = reportsList
                .Where(w => w.WeaknessTypeID == model.Form.WeaknessTypeID)
                .ToList();
        }

        // Apply Search String
        if (!string.IsNullOrEmpty(model.Form.SearchString) && 
            !string.IsNullOrWhiteSpace(model.Form.SearchString))
        {
            var search = model.Form.SearchString.ToLower().Trim();
            foreach (var weaknessType in reportsList)
            {
                weaknessType.WeaknessRiskLvs = weaknessType.WeaknessRiskLvs
                    .Where(r => r.WeaknessName.ToLower().Contains(search))
                    .ToList();
            }
            reportsList = reportsList.Where(r => r.WeaknessRiskLvs.Count > 0).ToList();
        }

        // Order Reports
        foreach (var type in reportsList)
        {
            switch (model.Form.Order)
            {
                case WeaknessReportOrder.Weakness:
                    type.WeaknessRiskLvs = type.WeaknessRiskLvs.OrderBy(w => w.WeaknessName).ToList();
                    break;
                case WeaknessReportOrder.MinRisk:
                    type.WeaknessRiskLvs = type.WeaknessRiskLvs.OrderBy(w => w.MinRiskLv).ToList();
                    break;
                case WeaknessReportOrder.AvgRisk:
                    type.WeaknessRiskLvs = type.WeaknessRiskLvs.OrderBy(w => w.AvgRiskLv).ToList();
                    break;
                case WeaknessReportOrder.MaxRisk:
                    type.WeaknessRiskLvs = type.WeaknessRiskLvs.OrderBy(w => w.MaxRiskLv).ToList();
                    break;
            }
        }

        model.Reports = reportsList;
        model.WeaknessTypes = new SelectList(reportsListResult.Data, "WeaknessTypeID", "WeaknessTypeName");
        
        return View(model);
    }
}