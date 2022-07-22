using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warhammer.Domain.Tournaments;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Controllers
{
	[Route("")]
	public class HomeController : BaseController
	{
		private ITournamentRepo TournamentRepo { get; }

		public HomeController(ITournamentRepo tournamentRepo , UserManager<User> userManager)
			: base(userManager)
		{
			this.TournamentRepo = tournamentRepo;
		}

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var tournaments = await this.TournamentRepo.GetTournamentsAsync();
			return this.View(tournaments);
		}
	}
}
