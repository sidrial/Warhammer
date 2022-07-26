using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Warhammer.Application.Models.TournamentViewModels
{
    public class UpdatePlayerViewModel
    {
        private static readonly List<string> Factions = new()
        {
            "Adeptus Astartes",
            "Adeptus Custodes",
            "Adeptus Mechanicus",
            "Adeptus Sororitas",
            "Aeldari",
            "Anhrathe",
            "Astra Militarum",
            "Asuryani",
            "Blood Angels",
            "Chaos",
            "Chaos Daemons",
            "Dark Angels",
            "Dark Mechanicus",
            "Death Guard",
            "Deathwatch",
            "Drukhari",
            "Forces of the Hive Mind",
            "Genestealer Cults",
            "Grey Knights",
            "Harlequins",
            "Heretic Astartes",
            "Imperial Agents",
            "Imperium",
            "Khorne",
            "Necrons",
            "Nurgle",
            "Orks",
            "Questor Imperialis",
            "Questor Traitoris",
            "Renegades and heretics",
            "Slaanesh",
            "Space Wolves",
            "T'au",
            "Thousand Sons",
            "Tyranids",
            "Tzeentch",
            "Ynnari"
        };

        public string TournamentId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Club { get; set; }

        public SelectList FactionsDropDown => new(Factions);
        [Required]
        public string PrimaryFaction { get; set; }
        [Required]
        public string ArmyList { get; set; }
      
        [Required]
        [Range(1000, 9999, ErrorMessage = "Ongeldige PIN!")]
        public uint PinNumber { get; set; }


        public UpdatePlayerViewModel()
        {
        }

        public UpdatePlayerViewModel(Domain.Tournaments.Entities.Player player)
        {
            this.PlayerId = player.Id;
            this.PlayerName = player.Name;
            this.Club = player.Club;
            this.TournamentId = player.TournamentId;
            this.ArmyList = player.List;
            this.PrimaryFaction = player.Faction;
        }
    }
}