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
    /// get the schedule for logged in student
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<StudentSchedule> GetSchedule()
    {
        return Ok(new StudentSchedule());
    }
}