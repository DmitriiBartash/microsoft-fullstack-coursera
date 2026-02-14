using System.ComponentModel.DataAnnotations;

namespace UserAuthInMemoryApp.Models.ViewModels;

/// <summary>
/// View model for user registration form.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
