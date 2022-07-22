using System;
using System.Collections.Generic;
using Architect.Identities;
using Newtonsoft.Json;
using Warhammer.Domain.Tournaments.Enums;

namespace Warhammer.Domain.Tournaments.Entities
{
    /// <summary>
    /// Represents the result of a match between two players.
    /// </summary>
    public class Match
    {
	    public override string ToString() => $"{this.Player1.Name} ({this.Player1VictoryPoints}) - {this.Player2.Name} ({this.Player2VictoryPoints})";

        public List<Player> Players => new() { this.Player1, this.Player2 };

        [JsonProperty("Id")]
        public string Id { get; private set; }

        [JsonProperty("TournamentId")]
        public string TournamentId { get; private set; }

        [JsonProperty("RoundId")]
        public string RoundId { get; private set; }

        [JsonProperty("CreationDateTime")]
        public DateTime CreationDateTime { get; private set; }

        [JsonProperty("Player1Id")]
        public string Player1Id { get; private set; }

        [JsonProperty("Player1")]
        public Player Player1 { get; private set; }

        [JsonProperty("Player1VictoryPoints")]
        public uint Player1VictoryPoints { get; private set; }

        [JsonProperty("Player2Id")]
        public string Player2Id { get; private set; }

        [JsonProperty("Player2")]
        public Player Player2 { get; private set; }

        [JsonProperty("Player2VictoryPoints")]
        public uint Player2VictoryPoints { get; private set; }

        [JsonProperty("Status")]
        public MatchStatus Status { get; private set; }

        [JsonProperty("LoggedByPin")]
        public uint? LoggedByPin { get; private set; }

        /// <summary>
        /// Record a new result from a match between two players.
        /// </summary>
        public Match(Player[] players, string roundId, string tournamentId)
        {
            if (players[0]?.Id == players[1]?.Id)
            {
                throw new ArgumentException("Players winner and loser are the same player");
            }

            this.Id = DistributedId.CreateId().ToAlphanumeric();
            this.TournamentId = tournamentId;
            this.CreationDateTime = DateTime.UtcNow;
            this.Player1Id = players[0].Id;
            this.Player1 = players[0];
            this.Player2Id = players[1].Id;
            this.Player2 = players[1];
            this.Status = MatchStatus.Created;
            this.RoundId = roundId;
        }

        public void LogMatchPoints(uint player1VictoryPoints, uint player2VictoryPoints, uint? pin)
        {
            this.Player1VictoryPoints = player1VictoryPoints;
            this.Player2VictoryPoints = player2VictoryPoints;
            this.Status = MatchStatus.PendingApproval;
            this.LoggedByPin = pin;
        }

        public void Finalize()
        {
            this.Status = MatchStatus.Confirmed;
        }
        
        public uint GetCurrentPlayerScore(string playerId)
        {
            if (this.Status != MatchStatus.Confirmed) return 0;
            uint points = 0;
            // Given player matches player 1
            if (this.Player1Id == playerId)
            {
                points += this.Player1VictoryPoints;
                if (this.Player1VictoryPoints > this.Player2VictoryPoints)
                {
                    points += 1000;
                }
                else if (this.Player1VictoryPoints == this.Player2VictoryPoints)
                {
                    points += 500;
                }
            }
            // Given player matches player 2
            else
            {
                points += this.Player2VictoryPoints;
                if (this.Player2VictoryPoints > this.Player1VictoryPoints)
                {
                    points += 1000;
                }
                else if (this.Player1VictoryPoints == this.Player2VictoryPoints)
                {
                    points += 500;
                }
            }

            return points;
        }

        public uint GetConfirmCount()
        {
	        return this.Status switch
	        {
		        MatchStatus.Confirmed => 2,
		        MatchStatus.PendingApproval => 1,
		        MatchStatus.Created => 0,
		        _ => throw new ArgumentOutOfRangeException()
	        };
        }

        public string GetToBeApprovedByLabel()
        {
	        if (!this.LoggedByPin.HasValue) return "PIN";
	        var playerLabel = this.Player1.Pin == this.LoggedByPin ? this.Player2.Name : this.Player1.Name;
	        return $"{playerLabel}'s PIN";
        }

    }
}