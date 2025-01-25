using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.API.db.Entities;
using FourthWallAcademy.API.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FourthWallAcademy.API.Controllers;

[ApiController]
[Route("api/Login")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationController(ILogger<AuthenticationController> logger, 
        SignInManager<ApplicationUser> signInManager, 
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return TypedResults.Problem(ModelState.ToString(), statusCode: 400);
        }
        
        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        
        var signInResult = await _signInManager.PasswordSignInAsync(model.Alias, model.Password, false, false);
        if (!signInResult.Succeeded)
        {
            return TypedResults.Problem(signInResult.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        return TypedResults.Empty;
    }
}