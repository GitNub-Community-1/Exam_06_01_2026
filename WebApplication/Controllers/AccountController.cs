using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebAPIWithJWTAndIdentity.Response;
using Infrastructure.Services;
using Domain.Dtos;
using System.Net;

[Route("[controller]")]
[Authorize]
public class AccountController(IAccountService _accountService) : ControllerBase
{
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
    {
        if (ModelState.IsValid)
        {
            var response  = await _accountService.Register(registerDto);
            return StatusCode(response.StatusCode, response);
        }
        else
        {
            var errorMessages = ModelState.SelectMany(e => e.Value.Errors.Select(e => e.ErrorMessage)).ToList();
            var response = new Response<RegisterDto>(HttpStatusCode.BadRequest, errorMessages);
            return StatusCode(response.StatusCode, response);
        }
        
    }
    
    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody]LoginDto registerDto)
    {
        if (ModelState.IsValid)
        {
            var response  = await _accountService.Login(registerDto);
            return StatusCode(response.StatusCode, response);
        }
        else
        {
            var errorMessages = ModelState.SelectMany(e => e.Value.Errors.Select(e => e.ErrorMessage)).ToList();
            var response = new Response<RegisterDto>(HttpStatusCode.BadRequest, errorMessages);
            return StatusCode(response.StatusCode, response);
        }
        
    }

    [HttpPost("AddUserToRole")]
    public async Task<Response<string>> AddUserToRole(UserRoleDto userRoleDto)
    {
        return await _accountService.AddOrRemoveUserFromRole(userRoleDto,false);
    }
    
    
    [HttpDelete("DeleteRoleFromUser")]
    public async Task<Response<string>> DeleteRoleFromUser(UserRoleDto userRoleDto)
    {
        return await _accountService.AddOrRemoveUserFromRole(userRoleDto,true);
    }
    
    
    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<Response<string>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        return await _accountService.ForgotPasswordTokenGenerator(forgotPasswordDto);
    }
    
      
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<Response<string>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        return await _accountService.ResetPassword(resetPasswordDto);
    }




}

