using System.ComponentModel.DataAnnotations;

namespace Warhammer.Application.Models.IdentityViewModels
{
    public class ChangePasswordViewModel
    {
	    [Required]
	    [StringLength(100, ErrorMessage = "The password must be at max {1} characters long.")]
	    [DataType(DataType.Password)]
	    public string OldPassword { get; set; }

	    [Required]
	    [StringLength(100, ErrorMessage = "The password must be at max {1} characters long.")]
	    [DataType(DataType.Password)]
	    public string Password { get; set; }

	    [DataType(DataType.Password)]
	    [Display(Name = "Confirm password")]
	    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
	    public string ConfirmPassword { get; set; }
    }
}
