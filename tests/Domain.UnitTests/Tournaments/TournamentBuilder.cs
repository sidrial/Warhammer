using System;
using System.Reflection;
using Architect.Identities;
using Warhammer.Domain.Tournaments.Entities;

namespace Warhammer.Domain.UnitTests.Tournaments;

public class TournamentBuilder : DummyBuilder<Tournament, TournamentBuilder>
{
	private static FieldInfo IdBackingField { get; } = typeof(Tournament).GetField($"<{nameof(Tournament.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic) ??
	                                                   throw new Exception($"Failed to reflect on {nameof(Tournament)}.{nameof(Tournament.Id)}.");

	private string Id { get; set; } = DistributedId.CreateId().ToAlphanumeric();
	public TournamentBuilder WithId(string value) => this.With(b => b.Id = value);
	
	private string Name { get; set; } = "Dummy";
	public TournamentBuilder WithName(string value) => this.With(b => b.Name = value);

	private uint Length { get; set; } = 3;
	public TournamentBuilder WithLength(uint value) => this.With(b => b.Length = value);
		
	public override Tournament Build()
	{
		var result = new Tournament(this.Name, this.Length);
		if (!string.IsNullOrWhiteSpace(this.Id)) IdBackingField.SetValue(result, this.Id);
		return result;
	}
}
