using System.Collections.Generic;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Application.Models.PlayerViewModels
{
    public class PlayersViewModel
    {
		public List<PlayerViewModel> Players { get; }

	    public PlayersViewModel(List<Player> players)
	    {
		    this.Players = new List<PlayerViewModel>();
			
		    foreach (var player in players)
		    {
				this.Players.Add(new PlayerViewModel(player));
		    }
	    }
    }
}
