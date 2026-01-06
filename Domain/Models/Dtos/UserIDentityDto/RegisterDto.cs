using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos;

public class RegisterDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    //Making true structure by  Abubakr
    public string PhoneNumber { get; set; }
}