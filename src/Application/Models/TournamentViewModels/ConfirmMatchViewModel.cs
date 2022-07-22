using System.ComponentModel.DataAnnotations;

namespace Warhammer.Application.Models.TournamentViewModels
{
    public class ConfirmMatchViewModel
    {
        public string MatchId { get; set; }
        public string TournamentId { get; set; }
        public string Player1 { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Score moet tussen 0 en 100 punten zijn.")]
        public uint Player1VictoryPoints { get; set; }
        public string Player2 { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Score moet tussen 0 en 100 punten zijn.")]
        public uint Player2VictoryPoints { get; set; }
        [Required]
        [Range(1000, 9999, ErrorMessage = "Ongeldige PIN!")]
        public uint PinNumber { get; set; }
        public string PinLabel { get; set; }
        public uint ConfirmCount { get; set; }

        public ConfirmMatchViewModel(string matchId, string tournamentId, string player1, uint player1VictoryPoints,
	        string player2, uint player2VictoryPoints, string pinLabel, uint confirmCount)
        {
            this.MatchId = matchId;
            this.TournamentId = tournamentId;
            this.Player1 = player1;
            this.Player1VictoryPoints = player1VictoryPoints;
            this.Player2 = player2;
            this.Player2VictoryPoints = player2VictoryPoints;
            this.PinLabel = pinLabel;
            this.ConfirmCount = confirmCount;
        }

        public ConfirmMatchViewModel()
        {
        }
    }
}
