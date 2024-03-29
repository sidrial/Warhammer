﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warhammer.Application.Models.TournamentViewModels;
using Warhammer.Application.Resources;
using Warhammer.Domain.Tournaments;
using Warhammer.Domain.Tournaments.Entities;
using Warhammer.Domain.Tournaments.Enums;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Constants;
using User = Warhammer.Domain.Users.Entities.User;

namespace Warhammer.Application.Controllers;

[Route("tournaments/")]
public class TournamentsController : BaseController
{
	private ITournamentRepo TournamentRepo { get; }
	private IEmailSender EmailSender { get; }

	public TournamentsController(UserManager<User> userManager,
		ITournamentRepo tournamentRepo,
		IEmailSender emailSender)
		: base(userManager)
	{
		this.TournamentRepo = tournamentRepo;
		this.EmailSender = emailSender;
	}

	[HttpGet("")]
	public async Task<IActionResult> Index(string id)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(id);
		var viewModel = new TournamentViewModel(tournament);
		return this.View(viewModel);
	}


	[HttpGet("create")]
	public async Task<IActionResult> CreateTournament()
	{
		var tournament = new Tournament("Dummy", 3);
		await this.TournamentRepo.AddTournamentAsync(tournament);
		var viewModel = new TournamentViewModel(tournament);
		return this.View("Index", viewModel);
	}

	[HttpGet("next_status")]
	public async Task<IActionResult> NextTournamentStatus(string id)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(id);
		tournament.Progress();
		await this.TournamentRepo.UpdateTournamentAsync(tournament);
		return this.RedirectToAction(nameof(this.Index),new { id = tournament.Id });
	}

	[HttpPost("create_player")]
	[Authorize]
	public async Task<ActionResult> CreatePlayer(CreatePlayerViewModel createPlayerViewModel)
	{
		if (!this.ModelState.IsValid) return this.RedirectToAction(nameof(this.Index), new { id = createPlayerViewModel.TournamentId });
		var tournament = await this.TournamentRepo.GetTournamentAsync(createPlayerViewModel.TournamentId);
		var player = new Player(createPlayerViewModel.TournamentId, createPlayerViewModel.PlayerName, createPlayerViewModel.Email, createPlayerViewModel.Club);
		tournament.Players.Add(player);
		await this.TournamentRepo.UpdateTournamentAsync(tournament);

		// send confirmation email
		var htmlMessage = Email.Greeting + "<br><br>" +
		                  Email.AccountCreatedIntroduction + "<br>" +
		                  $"Your PIN code to confirm matches is: {player.Pin}";

		await this.EmailSender.SendEmailAsync(player.Email, "De Dobbelsteen 40k Groep: Account created", htmlMessage).ConfigureAwait(false);

		return this.RedirectToAction(nameof(this.Index), new { id = createPlayerViewModel.TournamentId });
	}

	[HttpGet("view_player")]
	public async Task<ActionResult> ViewPlayer(string playerId, string tournamentId)
	{
		var isAdmin = this.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Claims.Admin);
		var tournament = await this.TournamentRepo.GetTournamentAsync(tournamentId);
		var player = tournament.Players.Single(player => player.Id == playerId);

		var isArmyListVisible = isAdmin || tournament.Status != TournamentStatus.Created;
		var updatePlayerPossible = isAdmin || tournament.Status == TournamentStatus.Created;

		var viewModel = new ViewPlayerViewModel(player, isArmyListVisible, updatePlayerPossible);
		return this.View(viewModel);
	}

	[HttpPost("view_player")]
	public async Task<ActionResult> ViewPlayer(ViewPlayerViewModel viewPlayerViewModel)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(viewPlayerViewModel.TournamentId);
		var player = tournament.Players.Single(player => player.Id == viewPlayerViewModel.PlayerId);

		player.SetExtraPoints(viewPlayerViewModel.ExtraPointsListDeadline, viewPlayerViewModel.ExtraPointsListValid, viewPlayerViewModel.ExtraPointsListPainted);
		player.Rename(viewPlayerViewModel.PlayerName);
		await this.TournamentRepo.UpdateTournamentAsync(tournament);

		return this.RedirectToAction(nameof(this.ViewPlayer), new { playerId = player.Id, tournamentId = tournament.Id });
	}

	[HttpPost("delete_player")]
	public async Task<ActionResult> DeletePlayer(ViewPlayerViewModel viewPlayerViewModel)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(viewPlayerViewModel.TournamentId);
		var player = tournament.Players.Single(player => player.Id == viewPlayerViewModel.PlayerId);
		tournament.Players.Remove(player);

		await this.TournamentRepo.UpdateTournamentAsync(tournament);

		return this.RedirectToAction(nameof(this.Index), new { id = viewPlayerViewModel.TournamentId });
	}

	[HttpGet("update_player")]
	public async Task<ActionResult> UpdatePlayer(string playerId, string tournamentId)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(tournamentId);
		var player = tournament.Players.Single(player => player.Id == playerId);
		return this.View(new UpdatePlayerViewModel(player));
	}

	[HttpPost("update_player")]
	public async Task<ActionResult> UpdatePlayer(UpdatePlayerViewModel updatedPlayer)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(updatedPlayer.TournamentId);
		var player = tournament.Players.Single(player => player.Id == updatedPlayer.PlayerId);

		if (player.CanUpdatePlayerList(tournament.Status, updatedPlayer.PinNumber, this.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Claims.Admin)))
		{ 
			player.UpdatePlayerList(updatedPlayer.Club, updatedPlayer.PrimaryFaction, updatedPlayer.ArmyList);
			await this.TournamentRepo.UpdateTournamentAsync(tournament);
			return this.RedirectToAction(nameof(this.ViewPlayer), new { playerId = updatedPlayer.PlayerId, tournamentId = updatedPlayer.TournamentId });
		}

		this.ModelState.AddModelError(nameof(ConfirmMatchViewModel.PinNumber), "Invalid PIN.");
		return this.View(updatedPlayer);		
	}
	
	[HttpPost("create_round")]
	[Authorize]
	public async Task<ActionResult> CreateRound(CreatePlayerViewModel createPlayerViewModel)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(createPlayerViewModel.TournamentId);
		tournament.AddNextRound();
		
		await this.TournamentRepo.UpdateTournamentAsync(tournament);

		return this.RedirectToAction(nameof(this.Index), new { id = createPlayerViewModel.TournamentId });
	}

	[HttpPost("recreate_round")]
	[Authorize]
	public async Task<ActionResult> RecreateLastRound(CreatePlayerViewModel createPlayerViewModel)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(createPlayerViewModel.TournamentId);
		tournament.ShuffleLatestPairing();

		await this.TournamentRepo.UpdateTournamentAsync(tournament);

		return this.RedirectToAction(nameof(this.Index), new { id = createPlayerViewModel.TournamentId });
	}

	[HttpGet("confirm_match")]
	public async Task<ActionResult> ConfirmMatch(string matchId, string tournamentId)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(tournamentId);
		var match = tournament.Rounds.SelectMany(round => round.Matches).Single(match => match.Id == matchId);

		var pinLabel = match.GetToBeApprovedByLabel();
		var confirmCount = match.GetConfirmCount();
		return this.View(new ConfirmMatchViewModel(match.Id, match.TournamentId, match.Player1.Name, match.Player1VictoryPoints, match.Player2.Name, match.Player2VictoryPoints, pinLabel, confirmCount));
	}
	
	[HttpPost("confirm_match")]
	public async Task<ActionResult> ConfirmMatch(ConfirmMatchViewModel confirmMatchViewModel)
	{
		var tournament = await this.TournamentRepo.GetTournamentAsync(confirmMatchViewModel.TournamentId);

		try
		{
			tournament.UpdateMatch(confirmMatchViewModel.MatchId, confirmMatchViewModel.Player1VictoryPoints, confirmMatchViewModel.Player2VictoryPoints, confirmMatchViewModel.PinNumber, this.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Claims.Admin));
		}
		catch (ValidationException exception)
		{
			this.ModelState.AddModelError(nameof(ConfirmMatchViewModel.PinNumber), exception.Message);
			return this.View(confirmMatchViewModel);
		}

		await this.TournamentRepo.UpdateTournamentAsync(tournament);
		var match = tournament.Rounds.SelectMany(round => round.Matches).Single(match => match.Id.Equals(confirmMatchViewModel.MatchId));

		return match.Status == MatchStatus.Confirmed ? 
			this.RedirectToAction(nameof(this.Index), new { id = match.TournamentId }) :
			this.RedirectToAction(nameof(this.ConfirmMatch), new { matchId = confirmMatchViewModel.MatchId, tournamentId = tournament.Id });
	}
}