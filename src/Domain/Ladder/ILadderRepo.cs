using System.Collections.Generic;
using System.Threading.Tasks;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Domain.Ladder;

public interface ILadderRepo
{
	Task<IEnumerable<Match>> GetMatchesAsync();
	Task<(IEnumerable<Match> Matches, uint TotalCount)> GetMatchesPagedAsync(uint pageIndex, uint pageSize);
	Task<Match?> GetMatchAsync(string id);
	Task AddMatchAsync(Match match);
	Task UpdateMatchAsync(Match match);
	Task DeleteMatchAsync(uint id);
}