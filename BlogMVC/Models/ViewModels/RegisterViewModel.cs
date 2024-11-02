using System.ComponentModel.DataAnnotations;

public class RegisterViewModel {
    [Required(ErrorMessage ="{0} required")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "{0} required")]
    [StringLength(10, MinimumLength = 3,ErrorMessage = "NickName Size must be between {1} and {2} characters")]
    public string NickName { get; set; }
    [Required(ErrorMessage = "{0} required")]
    [DataType(DataType.Password)]
    [StringLength(16, MinimumLength =6, ErrorMessage = "Password must be between {1} and {2} characters")]
    public string Password { get; set; }
    [Required(ErrorMessage = "{0} required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "{0} must be equals to password")]
    public string ConfirmPassword { get; set; }
}