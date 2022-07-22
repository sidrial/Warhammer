using System;
using Architect.Identities;
using Newtonsoft.Json;

namespace Warhammer.Domain.Ladder.Entities
{
    /// <summary>
    /// Represents the result of a match between two players.
    /// </summary>
    public class Match
    {
	    [JsonProperty("id")]
        public string Id { get; private set; }
       
        [JsonProperty("CreationDateTime")]
        public DateTime CreationDateTime { get; private set; }

        [JsonProperty("Winner")]
        public Player Winner { get; private set; }
       
        [JsonProperty("WinnerVictoryPoints")]
        public uint WinnerVictoryPoints { get; private set; }
       
        [JsonProperty("Loser")]
        public Player Loser { get; private set; }
       
        [JsonProperty("LoserVictoryPoints")]
        public uint LoserVictoryPoints { get; private set; }
       
        [JsonProperty("RatingChange")]
        public double RatingChange { get; private set; }

        [JsonProperty("Status")]
        public MatchStatus Status { get; private set; }

        [JsonProperty("LoggedByPlayerId")]
        public string LoggedByPlayerId { get; private set; }
        
        /// <summary>
        /// Record a new result from a match between two players.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        /// <param name="isDraw"></param>
        public Match(Player winner, uint winnerVictoryPoints, Player loser, uint loserVictoryPoints, string loggedByPlayerId)
        {
	        this.Id = DistributedId.CreateId().ToAlphanumeric();
            if (!this.ValidatePlayers(winner, loser))
            {
                throw new ArgumentException("Players winner and loser are the same player");
            }

            this.CreationDateTime = DateTime.UtcNow;
            this.LoggedByPlayerId = loggedByPlayerId;
            this.Winner = winner;
            this.WinnerVictoryPoints = winnerVictoryPoints;
            this.Loser = loser;
            this.LoserVictoryPoints = loserVictoryPoints;
            this.Status = MatchStatus.Pending;
        }

        public void SetRatingChange(double ratingChange)
        {
	        this.RatingChange = ratingChange;
        }

        public void ConfirmMatch()
        {
	        this.Status = MatchStatus.Confirmed;
        }

        /// <summary>
        /// Check that we're not doing anything silly like recording a match with only one player.
        /// </summary>
        internal bool ValidatePlayers(Player player1, Player player2)
        {
            return player1?.Id != player2?.Id;
        }

        /// <summary>
        /// Test whether a particular player participated in the match represented by this result.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Participated(Player player)
        {
	        return player?.Id == this.Winner?.Id || player?.Id == this.Loser?.Id;
        }

    }

    public enum MatchStatus
    {
        Pending = 0,
        Confirmed = 1,
        Declined = 2
    }
}