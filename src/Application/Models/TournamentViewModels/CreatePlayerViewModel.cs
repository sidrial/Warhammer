using System.ComponentModel.DataAnnotations;

namespace Warhammer.Application.Models.TournamentViewModels
{
    public class CreatePlayerViewModel
    {
        [Required]
        public string TournamentId { get; set; }
        [Required]
		public string PlayerName { get; set; }
        public string Club { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public CreatePlayerViewModel()
        {
        }

        public CreatePlayerViewModel(string tournamentId)
        {
            this.TournamentId = tournamentId;
        }
    }
}
