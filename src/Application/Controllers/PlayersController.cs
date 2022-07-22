using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warhammer.Application.Models.PlayerViewModels;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Controllers
{
	[Route("players/ranking")]
	public class PlayersController : BaseController
	{
		private IUserRepo UserRepo { get; }
	
		public PlayersController(UserManager<User> userManager,
			IUserRepo userRepo) 
			: base(userManager)
		{
			this.UserRepo = userRepo;
		}

		[HttpGet("")]
		public IActionResult Index()
		{
			var allPlayers = this.UserRepo.GetUsersAsync().Result.Where(user => user.EmailConfirmed).Select(user => user.Player)
				.OrderByDescending(player => player.Rating)
				.ToList();
			var viewModel = new PlayersViewModel(allPlayers);
			return this.View(viewModel);
		}
	}
}
