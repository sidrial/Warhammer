using Warhammer.Domain.Tournaments.Entities;

namespace Warhammer.Application.Models.TournamentViewModels
{
    public class ViewPlayerViewModel
    {
	    public string TournamentId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Club { get; set; }
        public string PrimaryFaction { get; set; }
        public string ArmyList { get; set; }
        public bool IsArmyVisible { get; set; }
        public bool UpdatePlayerPossible { get; set; }
        public bool ExtraPointsListDeadline { get; set; }
        public bool ExtraPointsListValid { get; set; }
        public ushort ExtraPointsListPainted { get; set; }
        public uint Secret { get; set; }

        public ViewPlayerViewModel()
        {
        }

        public ViewPlayerViewModel(Player player, bool isArmyVisible, bool updatePlayerPossible)
        {
	        this.TournamentId = player.TournamentId;
            this.PlayerId = player.Id;
            this.PlayerName = player.Name;
            this.Club = player.Club;
            this.PrimaryFaction = player.Faction;
            this.ArmyList = player.List;
            this.IsArmyVisible = isArmyVisible;
            this.UpdatePlayerPossible = updatePlayerPossible;
            this.ExtraPointsListDeadline = player.ExtraPointsListDeadline;
            this.ExtraPointsListValid = player.ExtraPointsListValid;
            this.ExtraPointsListPainted = player.ExtraPointsListPainted;
            this.Secret = player.Pin;
        }
    }
}