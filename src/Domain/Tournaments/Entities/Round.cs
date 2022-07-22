using System.Collections.Generic;
using Architect.Identities;
using Newtonsoft.Json;

namespace Warhammer.Domain.Tournaments.Entities;

public class Round
{
	[JsonProperty("Id")]
	public string Id { get; private set; }

	[JsonProperty("TournamentId")]
	public string TournamentId { get; private set; }

	[JsonProperty("Number")]
	public uint Number { get; private set; }

	[JsonProperty("Matches")]
	public List<Match> Matches { get; private set; }

	public Round(string tournamentId, uint number, List<Match> matches)
	{
		this.Id = DistributedId.CreateId().ToAlphanumeric();
		this.TournamentId = tournamentId;
		this.Number = number;
		this.Matches = matches;
	}
}