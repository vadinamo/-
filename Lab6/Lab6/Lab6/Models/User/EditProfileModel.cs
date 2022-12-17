using System.ComponentModel.DataAnnotations;

namespace Lab6.Models.User;

public class EditProfileModel
{
    [DataType(DataType.Text)]
    public string? Username { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords doesn't match")]
    public string? ConfirmPassword { get; set; }
}