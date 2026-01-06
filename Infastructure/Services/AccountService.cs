using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Dtos;
using Infastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using WebAPIWithJWTAndIdentity.Response;

namespace Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IEmailService _emailService;

    public AccountService(
        UserManager<User> userManager,
        IConfiguration configuration,
        ApplicationDbContext context,
        RoleManager<IdentityRole<int>> roleManager,
        IEmailService emailService)

    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _roleManager = roleManager;
        _emailService = emailService;
    }

    public async Task<Response<RegisterDto>> Register(RegisterDto model)
    {
        var mapped = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var response = await _userManager.CreateAsync(mapped, model.Password);

        if (response.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(mapped, "User");  

            // if (!roleResult.Succeeded)
            // {
            //     var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            //     return new Response<RegisterDto>(HttpStatusCode.BadRequest, $"Не удалось назначить роль User: {errors}");
            // }

            return new Response<RegisterDto>(HttpStatusCode.OK, "The user registered sussecfully!", model);
        }
        else
        {
            // var errors = string.Join(", ", response.Errors.Select(e => e.Description));
            return new Response<RegisterDto>(HttpStatusCode.BadRequest, $"Something went wrong!");
        }
    }

    public async Task<Response<string>> AddOrRemoveUserFromRole(UserRoleDto userRole, bool delete = false)
    {
        var role = await _roleManager.FindByIdAsync(userRole.RoleId);
        var user = await _userManager.FindByIdAsync(userRole.UserId);
        if (delete)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return new Response<string>(HttpStatusCode.OK, "removed");
        }

        var userInRole = await _userManager.IsInRoleAsync(user, role.Name);
        if (userInRole) return new Response<string>(HttpStatusCode.BadRequest, "Role exists");
        await _userManager.AddToRoleAsync(user, role.Name);
        return new Response<string>(HttpStatusCode.OK, "done");
    }


    public async Task<Response<string>> Login(LoginDto login)
    {
        var user = await _userManager.FindByNameAsync(login.Username);
        if (user != null)
        {
            var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            if (checkPassword)
            {
                var token = await GenerateJwtToken(user);
                return new Response<string>(token);
            }

            return new Response<string>(HttpStatusCode.BadRequest, "login or password is incorrect");
        }

        return new Response<string>(HttpStatusCode.BadRequest, "login or password is incorrect");
    }

    //Method to generate The Token
    private async Task<string> GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Name, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString())
        };

        //add roles
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        //Making true structure by Abubakr
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    public async Task<Response<string>> ChangePassword(ChangePasswordDto passwordDto, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var checkPassword = await _userManager.CheckPasswordAsync(user!, passwordDto.OldPassword);
        if (!checkPassword) return new Response<string>(HttpStatusCode.BadRequest, "password is incorrect");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user!);
        var result = await _userManager.ResetPasswordAsync(user!, token, passwordDto.Password);
        if (result.Succeeded)
            return new Response<string>(HttpStatusCode.OK, "success");
        return new Response<string>(HttpStatusCode.BadRequest, "could not reset your password");
    }

    public async Task<Response<string>> ForgotPasswordTokenGenerator(ForgotPasswordDto forgotPasswordDto)
    {
        var existing = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (existing == null) return new Response<string>(HttpStatusCode.BadRequest, "not found");
        var token = await _userManager.GeneratePasswordResetTokenAsync(existing);
        var url = $"token={token} email={forgotPasswordDto.Email}";
        _emailService.SendEmail(
            new MessageDto(new[] { forgotPasswordDto.Email }, "reset password message",
                $"<h1>{url} --  {token}   - copy this and past in your swagger for reseting your password)</h1>"),
            TextFormat.Html);

        return new Response<string>(HttpStatusCode.OK, "reset password has been sent");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return new Response<string>(HttpStatusCode.BadRequest, "user not found");

        var resetPassResult =
            await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
        if (resetPassResult.Succeeded)
            return new Response<string>(HttpStatusCode.OK, "success");

        return new Response<string>(HttpStatusCode.BadRequest, "please try again");
    }
}