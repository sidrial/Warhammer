using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Architect.Identities;
using Newtonsoft.Json;
using Warhammer.Domain.Tournaments.Enums;

namespace Warhammer.Domain.Tournaments.Entities;

public class Tournament
{
	// this id must be lower case in cosmos!
	[JsonProperty("id")] 
	public string Id { get; private set; }

	[JsonProperty("Name")]
	public string Name { get; private set; }

	[JsonProperty("Status")]
	public TournamentStatus Status { get; private set; }

	/// <summary>
	/// A list of all attending players of the tournament
	/// </summary>
	[JsonProperty("Players")]
	public List<Player> Players { get; private set; } = new();

	/// <summary>
	/// A list of all rounds with matchups created for and during the tournament
	/// </summary>
	[JsonProperty("Rounds")]
	public List<Round> Rounds { get; private set; } = new();


	/// <summary>
	/// The predefined length of the tournament, how many rounds it will last.
	/// </summary>
	[JsonProperty("Length")]
	public uint Length { get; private set; }

	public Tournament(string name, uint length)
	{
		this.Id = DistributedId.CreateId().ToAlphanumeric();
		this.Name = name;
		this.Status = TournamentStatus.Created;
		this.Length = length;
	}

	/// <summary>
	/// Progresses the tournament, moving through the different tournament statuses.
	/// </summary>
	public void Progress()
	{
		var currentStatus = this.Status;
		var newStatus = currentStatus + 1;
		if (Enum.IsDefined(typeof(TournamentStatus), newStatus)) this.Status = newStatus;
	}

	public void UpdateMatch(string matchId, uint player1Score, uint player2Score, uint loggingPinNumber, bool isAdmin = false)
	{
		var match = this.Rounds.SelectMany(round => round.Matches).Single(match => match.Id.Equals(matchId));
		if (match is null) throw new ArgumentException($"No match found for id {matchId}.");

		// Validate if user can confirm current match
		if (!this.CanConfirmCurrentMatch(match, loggingPinNumber, isAdmin)) throw new ValidationException($"Incorrect PIN number!");

		// Admins can always confirm, and in addition immediately set Match status to Confirmed
		if (isAdmin)
		{
			match.LogMatchPoints(player1Score, player2Score, loggingPinNumber);
			FinalizeMatch();
			return;
		}

		// Match is already pending approval, check if final submitter didn't change point values.
		// If final approvers PIN is correct, finalize match!
		if (match.Status == MatchStatus.PendingApproval &&
			match.Player1VictoryPoints == player1Score &&
			match.Player2VictoryPoints == player2Score &&
			match.LoggedByPin != loggingPinNumber)
		{
			FinalizeMatch();
			return;
		}

		// Above statements all false, so this is first submittal.
		match.LogMatchPoints(player1Score, player2Score, loggingPinNumber);
		
		void FinalizeMatch()
		{
			match.Finalize();
			var player1 = this.Players.Single(player => player.Id == match.Player1.Id);
			player1.Points = this.GetPlayerScore(match.Player1.Id);
			var player2 = this.Players.Single(player => player.Id == match.Player2.Id);
			player2.Points = this.GetPlayerScore(match.Player2.Id);
		}
	}

	/// <summary>
	/// User can confirm  matches when they participated in it.
	/// Or if you're cool and admin!
	/// </summary>
	private bool CanConfirmCurrentMatch(Match match, uint loggingPinNumber, bool isAdmin)
	{
		// Admins can always confirm matches
		if (isAdmin) return true;

		// Current user was participant in match
		var userIsPlayer1 = match.Player1.Pin == loggingPinNumber;
		var userIsPlayer2 = match.Player2.Pin == loggingPinNumber;
		return userIsPlayer1 || userIsPlayer2;
	}

	public uint GetPlayerScore(string playerId)
	{
		var playerMatches = this.Rounds.SelectMany(round => round.Matches).Where(match => match.Player1Id == playerId || match.Player2Id == playerId).ToList();
		return playerMatches.Aggregate<Match, uint>(0, (current, match) => current + match.GetCurrentPlayerScore(playerId));
	}

	public void AddNextRound()
	{
		if (!this.Rounds.Any()) this.Rounds = new List<Round>();
		
		if (this.Status == TournamentStatus.Finished)
		{
			throw new ValidationException($"Can not create new round for {nameof(Tournament)} with status {nameof(TournamentStatus.Finished)}");
		}

		if (!(this.Rounds?.Count < this.Length))
		{
			throw new ValidationException($"Can not create new round for {nameof(Tournament)}, maximum number of rounds ({nameof(Tournament)}.{nameof(Length)}:{this.Length}) has been reached.");
		}

		var roundNumber = 1 + (uint)this.Rounds.Count;
		var upcomingMatches = this.GetNextRoundMatches().ToList();
		var round = new Round(this.Id, roundNumber, upcomingMatches);
		this.Rounds.Add(round);
	}

	private IEnumerable<Match> GetNextRoundMatches()
	{
		var previousPairings = this.Rounds
			.SelectMany(round => round.Matches)
			.Select(match => match
				.Players
				.OrderBy(player => player.Name))
			.ToArray();

		var roundRng = new Random();
		var pairingPool = this.Players
			.Select(player => new
			{
				Player = player,
				Randomizer = roundRng.Next(),
			})
			.OrderByDescending(o => o.Player.Points)
			.ThenBy(o => o.Randomizer)
			.Select(o => o.Player)
			.ToArray()      // Flatten to ensure the RNG is only called once per player
			.AsEnumerable().ToList();


		var matchSets = this.GenerateAllMatches(pairingPool, 1);
		var selectedMatchSet = matchSets
			.Where(matchSet => matchSet
				.Select(match => match
					.Players
					.OrderBy(player => player.Name)
				)
				.All(players => !previousPairings
					.Any(previous => previous
						.SequenceEqual(players)
					)
				)
			)
			.FirstOrDefault();
		;
		return selectedMatchSet;
	}

	private IEnumerable<IEnumerable<Match>> GenerateAllMatches(IEnumerable<Player> unpairedPlayers, int matchNumber)
	{
		if (!unpairedPlayers.Any())
		{
			yield return Enumerable.Empty<Match>();
			yield break;
		}

		var remainingPairings = unpairedPlayers.Count() / 2;

		for (var skip = 0; skip < remainingPairings; skip++)
		for (var offset = 0; offset < remainingPairings; offset++)
		{
			var match = new Match(
				roundId: matchNumber.ToString(),
				players: new[]
				{
					unpairedPlayers.Skip(offset).First(),
					unpairedPlayers.Skip(offset + 1 + skip).First()
				},
				tournamentId: this.Id);

			var subsequentMatches = this.GenerateAllMatches(unpairedPlayers.Except(match.Players), matchNumber + 1);
			foreach (var subsequentMatch in subsequentMatches)
				yield return new[] { match }.Concat(subsequentMatch);
		}
	}
}