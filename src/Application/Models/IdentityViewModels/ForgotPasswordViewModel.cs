using System.ComponentModel.DataAnnotations;

namespace Warhammer.Application.Models.IdentityViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
