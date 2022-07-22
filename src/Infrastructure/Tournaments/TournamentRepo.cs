using Microsoft.Azure.Cosmos;
using Warhammer.Domain.Tournaments;
using Warhammer.Domain.Tournaments.Entities;

namespace Warhammer.Infrastructure.Tournaments;

public class TournamentRepo : ITournamentRepo
{
    private readonly Container _container;

    public TournamentRepo(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        this._container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task AddTournamentAsync(Tournament tournament)
    {
        await this._container.CreateItemAsync(tournament, new PartitionKey(tournament.Id));
    }

    public async Task DeleteTournamentAsync(uint id)
    {
        await this._container.DeleteItemAsync<Tournament>(id.ToString(), new PartitionKey(id));
    }

    public async Task<Tournament> GetTournamentAsync(string id)
    {
        try
        {
            var response = await this._container.ReadItemAsync<Tournament>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

    }

    public async Task<IEnumerable<Tournament>> GetTournamentsAsync()
    {
	    var queryString = "SELECT * FROM c";
        var query = this._container.GetItemQueryIterator<Tournament>(new QueryDefinition(queryString));
	    var results = new List<Tournament>();
	    while (query.HasMoreResults)
	    {
		    var response = await query.ReadNextAsync();

		    results.AddRange(response.ToList());
	    }

	    return results;
    }

    public async Task UpdateTournamentAsync(Tournament tournament)
    {
        await this._container.UpsertItemAsync(tournament, new PartitionKey(tournament.Id));
    }
}