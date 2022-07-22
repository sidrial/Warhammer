using System.Collections.Generic;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Models.UserViewModels
{
    public class UsersViewModel
    {
		public IReadOnlyCollection<User> Users { get; }

	    public UsersViewModel(IReadOnlyCollection<User> users)
	    {
		    this.Users = users;
	    }
    }
}
