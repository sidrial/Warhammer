#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Domain.Users;

public interface IUserRepo
{
	Task<IEnumerable<User>> GetUsersAsync();
	Task<User> GetUserAsync(string id);
	Task<User?> GetUserByGuidAsync(string guid);
	Task<User?> GetUserByEmailAsync(string email);
	Task<User?> GetUserByUserNameAsync(string email);
	Task AddUserAsync(User user);
	Task UpdateUserAsync(User user);
	Task DeleteUserAsync(string id);
}