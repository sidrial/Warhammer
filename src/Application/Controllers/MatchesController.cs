using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warhammer.Application.Models.MatchViewModels;
using Warhammer.Domain.Ladder;
using Warhammer.Domain.Ladder.Entities;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Constants;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Controllers
{
	[Route("matches")]
	public class MatchesController : BaseController
	{
		private IUserRepo UserRepo { get; }
		private ILadderRepo LadderRepo { get; }

		public MatchesController(UserManager<User> userManager,
			IUserRepo userRepo,
			ILadderRepo ladderRepo)
			: base(userManager)
		{
			this.UserRepo = userRepo;
			this.LadderRepo = ladderRepo;
		}

		[HttpGet("overview")]
		public async Task<ActionResult> Index()
		{
			var users = await this.UserRepo.GetUsersAsync();
			var players = users.Select(user => user.Player);
			var createMatchViewModel = this.CurrentUser.EmailConfirmed ? new CreateMatchViewModel(players.ToList()) : null;
			return this.View(createMatchViewModel);
		}

		[HttpPost("getmatches")]
		public async Task<ActionResult> GetMatches(int draw, uint start, uint length) // Do NOT rename! Required by the JS component
		{
			var (matches, totalCount) = await this.LadderRepo.GetMatchesPagedAsync(start, length);

			// Filter out declined matches.
			var nonDeclinedMatches = matches.Where(match => match.Status != MatchStatus.Declined).ToList();
			var users = await this.UserRepo.GetUsersAsync();
			var players = users.Select(user => user.Player);
			
			// Map matches to viewmodels
			var matchViewModels = nonDeclinedMatches
				.Select(match => new MatchViewModel(match, this.UserCanConfirmCurrentMatch(match), players))
				.OrderByDescending(match => match.CreationDateTime).ToList();

			return this.Json(new AreaViewModel(draw, matchViewModels, totalCount));
		}

		/// <summary>
		/// User can confirm pending matches when they're not the creator of the match and participated in it.
		/// Or if you're cool and admin!
		/// </summary>
		private bool UserCanConfirmCurrentMatch(Match match)
		{
			// Only pending matches can be confirmed!
			if (match.Status != MatchStatus.Pending) return false;

			// Admins can always confirm matches
			if (this.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Claims.Admin)) return true;

			// Current user was participant in match
			if (match.Winner.Id == this.CurrentUser?.Player.Id ||
			    match.Loser.Id == this.CurrentUser?.Player.Id)
			{
				return match.LoggedByPlayerId != this.CurrentUser?.Player.Id;
			}

			return false;
		}

		[HttpPost("confirm")]
		[Authorize]
		public async Task<ActionResult> ConfirmMatch(string matchId)
		{
			var match = await this.LadderRepo.GetMatchAsync(matchId);
			if (match is null) return this.Json("Match not found");

			if (!this.UserCanConfirmCurrentMatch(match)) return this.Json("Unauthorized");

			var users = await this.UserRepo.GetUsersAsync();
			var winner = users.Single(user => user.Player.Id == match.Winner.Id);
			var loser = users.Single(user => user.Player.Id == match.Loser.Id);
			
			// Match can now be confirmed, calculate new ratings for participants
			// Confirm match, calculate ratings.
			var calculator = new RatingCalculator();
			var results = new RatingPeriodResults();
			var matchResult = new Result(winner.Player, loser.Player);
			results.AddResult(matchResult);

			calculator.UpdateRatings(results);
			
			// Rating difference : New rating - old rating
			var ratingDifference = winner.Player.Rating - match.Winner.Rating;
			
			// Update match
			match.ConfirmMatch();
			match.SetRatingChange(ratingDifference);
			await this.LadderRepo.UpdateMatchAsync(match);

			// Update players
			await this.UserRepo.UpdateUserAsync(winner);
			await this.UserRepo.UpdateUserAsync(loser);


			return this.Json("Confirmed match");
		}
		
		[HttpPost("create")]
		[Authorize]
		public async Task<ActionResult> Create(CreateMatchViewModel matchViewModel)
		{
			if (!this.ModelState.IsValid) return this.RedirectToAction(nameof(this.Index));
			if (matchViewModel.WinnerId == matchViewModel.LoserId) return this.RedirectToAction(nameof(this.Index));

			var users = await this.UserRepo.GetUsersAsync();
			var players = users.Select(user => user.Player);
			var winner = players.Single(player => player.Id == matchViewModel.WinnerId);
			var loser = players.Single(player => player.Id == matchViewModel.LoserId);
			var match = new Match(winner, matchViewModel.WinnerVictoryPoints, loser, matchViewModel.LoserVictoryPoints, this.CurrentUser.Player.Id);
			await this.LadderRepo.AddMatchAsync(match);

			return this.RedirectToAction(nameof(this.Index));
		}
	}
}
