using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warhammer.Application.Models.UserViewModels;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Constants;
using User = Warhammer.Domain.Users.Entities.User;

namespace Warhammer.Application.Controllers.Admin
{
	[Route("users")]
	public class UserController : BaseController
	{
		private IUserRepo UserRepo { get; }

		public UserController(UserManager<User> userManager, IUserRepo userRepo) 
			: base(userManager)
		{
			this.UserRepo = userRepo;
		}

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var users = await this.UserRepo.GetUsersAsync();
			var viewModel = new UsersViewModel(users.ToList().AsReadOnly());
			return this.View(viewModel);
		}

		[HttpGet("edit")]
		[HttpGet("edit/{userGuid}")]
		[Authorize(Roles = Claims.Admin)]
		public async Task<IActionResult> Edit(string userGuid = null)
		{
			try
			{
				var userViewModel = await this.GetUserViewModel(userGuid);

				return this.View(userViewModel);
			}
			catch (Exception e)
			{
				return this.StatusCode(400);
			}
		}

		private async Task<UserViewModel> GetUserViewModel(string userGuid)
		{
			var dbUser = await this.UserRepo.GetUserByGuidAsync(userGuid);
			var userViewModel = new UserViewModel(dbUser);

			return userViewModel;
		}

		[HttpPost("edit")]
		[HttpPost("edit/{userGuid}")]
		[Authorize(Roles = Claims.Admin)]
		public async Task<IActionResult> Edit(UserViewModel user, string userGuid = null)
		{
			var dbUser = await this.UserRepo.GetUserByGuidAsync(userGuid);

			// admin tries to approve new user. Can only do this is user confirmed emailaddress
			if (user.IsActive && !dbUser.EmailConfirmed)
			{
				this.ModelState.AddModelError(string.Empty, "User must first confirm emailaddress!");
				return this.View(user);
			}

			dbUser.SetEmail(user.Email);
			dbUser.SetUserName(user.Email);
			dbUser.SetPlayerName(user.PlayerName);
			dbUser.SetActive(user.IsActive);
			dbUser.SetUpdateDatetime(DateTime.UtcNow);
			
			await this.UserRepo.UpdateUserAsync(dbUser);
			
			// Reload the current user after an update
			if (this.CurrentUser.Id == dbUser.Id) this.ReloadCurrentUser();

			return this.RedirectToAction("Index");
		}
	}
}