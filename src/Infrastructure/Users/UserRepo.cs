using Microsoft.Azure.Cosmos;
using Warhammer.Domain.Users;
namespace Warhammer.Infrastructure.Users;
using Entity = Domain.Users.Entities.User;

public class UserRepo : IUserRepo
{
    private readonly Container _container;

    public UserRepo(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        this._container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task AddUserAsync(Entity user)
    {
        await this._container.CreateItemAsync(user, new PartitionKey(user.Id));
    }

    public async Task DeleteUserAsync(string id)
    {
        await this._container.DeleteItemAsync<Entity>(id, new PartitionKey(id));
    }

    public async Task<Entity> GetUserAsync(string id)
    {
        try
        {
            var response = await this._container.ReadItemAsync<Entity>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Entity?> GetUserByGuidAsync(string guid)
    {
	    var users = await this.GetUsersAsync();
	    return users.SingleOrDefault(x => x.Id == guid);
    }

    public async Task<Entity?> GetUserByEmailAsync(string email)
    {
	    var users = await this.GetUsersAsync();
	    return users.SingleOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Entity?> GetUserByUserNameAsync(string userName)
    {
	    var users = await this.GetUsersAsync();
	    return users.SingleOrDefault(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<Entity>> GetUsersAsync()
    {
	    var queryString = "SELECT * FROM c";
        var query = this._container.GetItemQueryIterator<Entity>(new QueryDefinition(queryString));
	    var results = new List<Entity>();
	    while (query.HasMoreResults)
	    {
		    var response = await query.ReadNextAsync();

		    results.AddRange(response.ToList());
	    }

	    return results;
    }
    
    public async Task UpdateUserAsync(Entity User)
    {
        await this._container.UpsertItemAsync(User, new PartitionKey(User.Id));
    }
}