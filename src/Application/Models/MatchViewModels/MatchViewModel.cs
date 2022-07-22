using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Application.Models.MatchViewModels
{
	public class MatchViewModel
	{
		public string MatchId { get; set; }
		public string CreationDateTime { get; set; }
		public string Winner { get; set; }
		public string WinnerVp { get; set; }
		public string Loser { get; set; }
		public string LoserVp { get; set; }
		public string Rating { get; set; }
		public string Status { get; set; }
		public string Confirm { get; set; }

		public MatchViewModel()
		{
		}

		public MatchViewModel(Match match, bool canConfirmMatch, IEnumerable<Player> players)
		{
			switch (match.Status)
			{
				case MatchStatus.Pending:
					this.Status = "<i class=\"fa fa-clock-o\" style=\"color:#2e6ca4;\"></i>";
					break;
				case MatchStatus.Confirmed:
					this.Status = "<i class=\"fa fa-check\" style=\"color:#228b22;\"></i>";
					break;
				case MatchStatus.Declined:
				default:
					throw new ArgumentOutOfRangeException();
			}

			TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
			var matchDateTime = DateTime.SpecifyKind(match.CreationDateTime, DateTimeKind.Utc);
			DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(matchDateTime, cstZone);


			this.MatchId = match.Id.ToString();
			this.CreationDateTime = cstTime.ToString(CultureInfo.GetCultureInfo("nl-NL"));
			this.Winner = players.Single(player => player.Id == match.Winner.Id).Name + $" ({match.WinnerVictoryPoints})";
			this.Loser = players.Single(player => player.Id == match.Loser.Id).Name + $" ({match.LoserVictoryPoints})";
			this.Rating = Math.Round(match.RatingChange).ToString();
			this.Confirm = canConfirmMatch ? "<button class=\"pull-right btn btn-sm btn-outline-primary\">Goedkeuren</button>" : null;
		}
	}
}