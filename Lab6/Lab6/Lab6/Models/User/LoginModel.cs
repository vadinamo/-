using System.ComponentModel.DataAnnotations;

namespace Lab6.Models.User;

public class LoginModel
{
    [Required(ErrorMessage = "Email?")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password?")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}