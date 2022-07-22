using System;
using System.Collections.Generic;
using System.Linq;
using Warhammer.Domain.Tournaments.Entities;
using Warhammer.Domain.Tournaments.Enums;

namespace Warhammer.Application.Models.TournamentViewModels
{
    public class TournamentViewModel
	{
        public string Id { get; }
        public List<Player> Players { get; }
        public List<Round> Rounds { get; }
        public uint Length { get; }
        public TournamentStatus Status { get; }
        public string Name { get; }
        public bool IsNextTournamentStatusPossible => this.Status switch
        {
	        TournamentStatus.Created => true,
	        TournamentStatus.PendingPlayerReview => true,
	        TournamentStatus.RoundsStarted => 
                this.Rounds.Count == this.Length && 
                this.Rounds.LastOrDefault()?.Matches.All(match => match.Status == MatchStatus.Confirmed) == true,
	        TournamentStatus.Finished => false,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public TournamentViewModel(Tournament tournament)
        {
            this.Id = tournament.Id;
            this.Rounds = tournament.Rounds;
            this.Length = tournament.Length;
            this.Players = tournament.Players?.OrderByDescending(p => p.CalculatedPlayerScore).ToList();
            this.Status = tournament.Status;
            this.Name = tournament.Name;
        }

     
    }
}