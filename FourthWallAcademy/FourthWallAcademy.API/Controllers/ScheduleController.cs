using FourthWallAcademy.API.db.Entities;
using FourthWallAcademy.API.Models;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ISectionService _sectionService;
    private readonly IStudentService _studentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ScheduleController(ILogger<ScheduleController> logger, 
        ISectionService sectionService, 
        IStudentService studentService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _sectionService = sectionService;
        _studentService = studentService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Get the schedule of the logged in user
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StudentSchedule>> GetSchedule(DateTime? fromDate, DateTime? toDate)
    {
        var startDate = fromDate ?? DateTime.Now;
        var endDate = toDate ?? DateTime.Now.AddDays(7);
        
        var user = await _userManager.GetUserAsync(HttpContext.User);
        
        var scheduleResult = _sectionService.GetStudentSchedule(user.StudentID, startDate, endDate);
        if (!scheduleResult.Ok)
        {
            _logger.LogError("Unable to get student schedule: " + scheduleResult.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        return Ok(scheduleResult.Data);
    }
}