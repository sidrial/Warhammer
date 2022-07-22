using System;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Application.Models.PlayerViewModels
{
    public class PlayerViewModel
    {
	    public string Name { get; set; }
		public double Rating { get; set; }


		public PlayerViewModel()
	    {
	    }

		public PlayerViewModel(Player player)
		{
			this.Name = player.Name;
			this.Rating = Math.Round(player.Rating);
		}
    }
}
