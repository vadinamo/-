using System.ComponentModel.DataAnnotations;

namespace Lab6.Models.User;

public class RegisterModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Username")]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    [RegularExpression (@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Incorrect email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword")]
    [Compare("Password", ErrorMessage = "Passwords doesn't match")]
    public string ConfirmPassword { get; set; }
}