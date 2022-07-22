using Microsoft.AspNetCore.Identity;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Infrastructure.Users
{
	/// <summary>
	/// Glue (for the User class) between Core Identity and our own ORM so we don't have to use Entity Framework 
	/// </summary>
	public class UserStore : IUserEmailStore<User>, IUserTwoFactorStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>, IQueryableUserStore<User>
	{
		private IUserRepo UserRepo { get; }

		public UserStore(IUserRepo userRepo)
		{
			this.UserRepo = userRepo;
		}

		public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await this.UserRepo.AddUserAsync(user).ConfigureAwait(false);

			return IdentityResult.Success;
		}
		
		public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await this.UserRepo.DeleteUserAsync(user.Id).ConfigureAwait(false);

			return IdentityResult.Success;
		}

		public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await this.UserRepo.GetUserAsync(userId).ConfigureAwait(false);
		}

		public async Task<User> FindByNameAsync(string userName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await this.UserRepo.GetUserByUserNameAsync(userName).ConfigureAwait(false);
		}

		public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName.ToUpper());
		}

		public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
		{
			user.SetUserName(userName);
			return Task.FromResult(0);
		}

		public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await this.UserRepo.UpdateUserAsync(user).ConfigureAwait(false);

			return IdentityResult.Success;
		}

		public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
		{
			user.SetEmail(email);
			return Task.FromResult(0);
		}

		public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
		{
			user.SetEmailConfirmed(confirmed);
			return Task.FromResult(0);
		}

		public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await this.UserRepo.GetUserByEmailAsync(normalizedEmail).ConfigureAwait(false);
		}

		public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Email.ToUpper());
		}

		public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
		
		public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
		{
			user.SetTwoFactorEnabled(enabled);
			return Task.FromResult(0);
		}

		public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.TwoFactorEnabled);
		}

		public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
		{
			user.SetPasswordHash(passwordHash);
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash != null);
		}

		public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return user.Roles.Select(role => role.Name).ToList();

		}

		public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return user.Roles.Any(role => role.NormalizedName == roleName.ToUpper());
		}

		public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			// Nothing to dispose.
		}

		public IQueryable<User> Users => this.UserRepo.GetUsersAsync().GetAwaiter().GetResult().AsQueryable();
	}
}
