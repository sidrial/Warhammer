using System.ComponentModel.DataAnnotations;

namespace Warhammer.Application.Models.IdentityViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Name")]
		public string PlayerName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The password must be at least {2} and at max {1} characters long, and must contain at least 1 lower case letter, 1 upper case, and 1 numeric character.", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
