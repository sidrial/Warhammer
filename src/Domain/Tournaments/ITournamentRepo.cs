using System.Collections.Generic;
using System.Threading.Tasks;
using Warhammer.Domain.Tournaments.Entities;

namespace Warhammer.Domain.Tournaments;

public interface ITournamentRepo
{
	Task<IEnumerable<Tournament>> GetTournamentsAsync();
	Task<Tournament> GetTournamentAsync(string id);
	Task AddTournamentAsync(Tournament tournament);
	Task UpdateTournamentAsync(Tournament tournament);
	Task DeleteTournamentAsync(uint id);
}