using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Warhammer.Domain.Ladder.Entities;
using Warhammer.Domain.Users.Constants;

namespace Warhammer.Domain.Users.Entities
{
	/// <summary>
	/// Acquiring portal application user entity
	/// </summary>
	[Serializable]
	public class User
	{
		[JsonProperty("id")]
		public string Id { get; private set; }
		
		[JsonProperty("UserName")]
		public string UserName { get; private set; }
		
		[JsonProperty("Email")]
		public string Email { get; private set; }
		
		[JsonProperty("EmailConfirmed")]
		public bool EmailConfirmed { get; private set; }
		
		[JsonProperty("PasswordHash")]
		public string PasswordHash { get; private set; }
	
		[JsonProperty("TwoFactorEnabled")]
		public bool TwoFactorEnabled { get; private set; }
		
		/// <summary>
		/// Everyone can create an account, but it needs to be activated by an Admin to participate in player ranking.
		/// </summary>
		[JsonProperty("IsActive")]
		public bool IsActive { get; private set; }
		
		[JsonProperty("CreationDateTime")]
		public DateTime CreationDateTime { get; private set; }
		
		[JsonProperty("UpdateDateTime")]
		public DateTime UpdateDateTime { get; private set; }

		[JsonProperty("Roles")]
		public IReadOnlyCollection<Role> Roles { get; private set; }

		[JsonProperty("Player")]
		public Player Player { get; private set; }
		
		/// <summary>
		/// Constructs a new user, password must be set via userManager interface
		/// </summary>
		public User(string email, string playerName)
		{
			this.Id = Guid.NewGuid().ToString("N").ToUpper();
			this.CreationDateTime = DateTime.UtcNow;
			this.UpdateDateTime = DateTime.UtcNow;
			this.UserName = email;
			this.Email = email;
			this.Player = new Player(playerName);
			this.AddRole(new Role(Claims.Member, Claims.Member.ToUpper()));
		}

		public bool HasRole(string roleName)
		{
			return this.Roles.Any(role => role.NormalizedName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
		}

		public void AddRole(Role role)
		{
			this.Roles ??= new List<Role>();
			if (this.Roles.Contains(role)) throw new InvalidOperationException($"{this} already contains {role}.");
			var roles = this.Roles.ToList();
			roles.Add(role);
			this.Roles = roles.AsReadOnly();
		}

		public void RemoveRole(Role role)
		{
			if (!this.Roles.Contains(role)) throw new InvalidOperationException($"{this} does not have {role}.");
			var roles = this.Roles.ToList();
			roles.Remove(role);
			this.Roles = roles.AsReadOnly();
		}

		public void SetUserName(string userName)
		{
			this.UserName = userName;
		}
		

		public void SetEmail(string email)
		{
			this.Email = email;
		}

		public void SetPlayerName(string playerName)
		{
			this.Player.SetPlayerName(playerName);
		}

		public void SetEmailConfirmed(bool emailConfirmed)
		{
			this.EmailConfirmed = emailConfirmed;
		}
		
		public void SetTwoFactorEnabled(bool twoFactorConfirmed)
		{
			this.TwoFactorEnabled = twoFactorConfirmed;
		}

		public void SetPasswordHash(string passwordHash)
		{
			this.PasswordHash = passwordHash;
		}

		public void SetActive(bool userIsActive)
		{
			this.IsActive = userIsActive;
		}
		public void SetUpdateDatetime(DateTime datetime)
		{
			this.UpdateDateTime = datetime;
		}
	}
}
