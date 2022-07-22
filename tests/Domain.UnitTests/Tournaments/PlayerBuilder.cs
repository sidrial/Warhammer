using System;
using System.Reflection;
using Architect.Identities;
using Warhammer.Domain.Tournaments.Entities;

namespace Warhammer.Domain.UnitTests.Tournaments;

public class PlayerBuilder : DummyBuilder<Player, PlayerBuilder>
{
	private static FieldInfo IdBackingField { get; } = typeof(Player).GetField($"<{nameof(Player.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic) ??
	                                                   throw new Exception($"Failed to reflect on {nameof(Player)}.{nameof(Player.Id)}.");

	private string Id { get; set; } = DistributedId.CreateId().ToAlphanumeric();
	public PlayerBuilder WithId(string value) => this.With(b => b.Id = value);

	private string TournamentId { get; set; } = DistributedId.CreateId().ToAlphanumeric();
	public PlayerBuilder WithTournamentId(string value) => this.With(b => b.TournamentId = value);

	private string Name { get; set; } = "Dummy";
	public PlayerBuilder WithName(string value) => this.With(b => b.Name = value);

	private string Email { get; set; } = "dummy@dummy.nl";
	public PlayerBuilder WithEmail(string value) => this.With(b => b.Email = value);

	private string Club { get; set; } = "Dummy & Dragon";
	public PlayerBuilder WithClub(string value) => this.With(b => b.Club = value);
	
	public override Player Build()
	{
		var result = new Player(this.TournamentId, this.Name, this.Email, this.Club);
		if (!string.IsNullOrWhiteSpace(this.Id)) IdBackingField.SetValue(result, this.Id);
		return result;
	}
}
