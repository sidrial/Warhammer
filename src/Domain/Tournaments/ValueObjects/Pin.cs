using System;
using System.Diagnostics.CodeAnalysis;

namespace Warhammer.Domain.Tournaments.ValueObjects;

/// <summary>
/// Secret PIN assigned to a player. Used to confirm the player's matches during the tournament
/// </summary>
public sealed class Pin
{
	public override int GetHashCode() => this.Value.GetHashCode();
	public override bool Equals(object? obj) => obj is Pin other && other.Value.Equals(this.Value);

	public uint Value { get; }

	public Pin(uint value)
	{
		if (value is < 1000 or > 9999) throw new ArgumentException($"A {nameof(Pin)} must have a value between 1000 and 9999: {value}.");
		this.Value = value;
	}

	public static explicit operator Pin(uint value) => new(value);
	public static implicit operator uint(Pin pin) => pin.Value;
}