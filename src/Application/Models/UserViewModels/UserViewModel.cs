using System.ComponentModel.DataAnnotations;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Models.UserViewModels
{
    public class UserViewModel
    {
	    [Required]
	    [EmailAddress]
		public string Email { get; set; }
		[Required]
		public string PlayerName { get; set; }
		public bool IsActive { get; set; }


		public UserViewModel()
	    {
	    }

		public UserViewModel(User user)
		{
			this.Email = user.Email;
			this.PlayerName = user.Player.Name;
			this.IsActive = user.IsActive;
		}
    }
}
