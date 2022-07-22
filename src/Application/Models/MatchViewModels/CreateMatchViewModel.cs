using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Application.Models.MatchViewModels
{
	public class CreateMatchViewModel
	{
		public SelectList PlayerSelectList { get; set; } 
		public string WinnerId { get; set; }
		public uint WinnerVictoryPoints { get; set; }
		public string LoserId { get; set; }
		public uint LoserVictoryPoints { get; set; }

		public CreateMatchViewModel()
		{
		}

		public CreateMatchViewModel(List<Player> players)
		{
			this.PlayerSelectList = new SelectList(players, nameof(Player.Id), nameof(Player.Name));
		}
	}
	
}