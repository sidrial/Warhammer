using Architect.Identities;
using Newtonsoft.Json;

namespace Warhammer.Domain.Users.Entities
{
	public class Role
	{
		[JsonProperty("id")]
		public string Id { get; private set; }
		[JsonProperty("Name")]
		public string Name { get; private set; }
		[JsonProperty("NormalizedName")]
		public string NormalizedName { get; private set; }

	
		public Role()
		{
		}

		public Role(string roleName, string normalizedName)
		{
			this.Id = DistributedId.CreateId().ToAlphanumeric();
			this.Name = roleName;
			this.NormalizedName = normalizedName;
		}

		public void SetName(string name)
		{
			this.Name = name;
		}

		public void SetNormalizedName(string normalizedName)
		{
			this.NormalizedName = normalizedName;
		}
	}
}
