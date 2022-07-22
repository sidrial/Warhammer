using Microsoft.AspNetCore.Identity;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Infrastructure.Users
{
	/// <summary>
	/// Glue (for the Role class) between Core Identity and our own ORM so we don't have to use Entity Framework 
	/// </summary>
	public class RoleStore : IRoleStore<Role>
	{
		public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Id.ToString());
		}

		public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Name);
		}

		public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
		{
			role.SetName(roleName);
			return Task.FromResult(0);
		}

		public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.NormalizedName);
		}

		public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
		{
			role.SetNormalizedName(normalizedName);
			return Task.FromResult(0);
		}
		
		public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			// Nothing to dispose.
		}
	}
}
