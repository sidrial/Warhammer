using Microsoft.Azure.Cosmos;
using Warhammer.Domain.Ladder;
using Warhammer.Domain.Ladder.Entities;

namespace Warhammer.Infrastructure.Ladder;

public class LadderRepo : ILadderRepo
{
    private readonly Container _container;

    public LadderRepo(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        this._container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task AddMatchAsync(Match match)
    {
        await this._container.CreateItemAsync(match, new PartitionKey(match.Id));
    }

    public async Task DeleteMatchAsync(uint id)
    {
        await this._container.DeleteItemAsync<Match>(id.ToString(), new PartitionKey(id));
    }

    public async Task<Match?> GetMatchAsync(string id)
    {
        try
        {
            ItemResponse<Match?> response = await this._container.ReadItemAsync<Match>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

    }

    public async Task<IEnumerable<Match>> GetMatchesAsync()
    {
	    var queryString = "SELECT * FROM c";
        var query = this._container.GetItemQueryIterator<Match>(new QueryDefinition(queryString));
	    var results = new List<Match>();
	    while (query.HasMoreResults)
	    {
		    var response = await query.ReadNextAsync();

		    results.AddRange(response.ToList());
	    }

	    return results;
    }

    public async Task<(IEnumerable<Match> Matches, uint TotalCount)> GetMatchesPagedAsync(uint pageIndex, uint pageSize)
    {
	    var queryString = "SELECT * FROM c";
	    var query = this._container.GetItemQueryIterator<Match>(new QueryDefinition(queryString));
	    var results = new List<Match>();
	    while (query.HasMoreResults)
	    {
		    var response = await query.ReadNextAsync();

		    results.AddRange(response.ToList());
	    }

        var total = (uint)results.Count;
        var selectedMatches = results.Skip((int)(pageIndex * pageSize)).Take((int)pageSize);
        return (selectedMatches, total);
    }

    public async Task UpdateMatchAsync(Match match)
    {
        await this._container.UpsertItemAsync(match, new PartitionKey(match.Id));
    }
}