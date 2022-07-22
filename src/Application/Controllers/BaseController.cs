using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warhammer.Domain.Users.Entities;

namespace Warhammer.Application.Controllers
{
	public class BaseController : Controller
	{
		private User _currentUser;
		protected User CurrentUser
		{
			get
			{
				if (this._currentUser == null)
				{
					this.SetCurrentUser();
				}

				return this._currentUser;
			}
		}

		protected readonly UserManager<User> UserManager;

		public BaseController(UserManager<User> userManager)
		{
			this.UserManager = userManager;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (this._currentUser == null) this.SetCurrentUser();
		}
		
		private void SetCurrentUser()
		{
			this._currentUser = this.UserManager.GetUserAsync(this.HttpContext.User).GetAwaiter().GetResult();

			if (this._currentUser != null)
			{
				this.HttpContext.Items["CurrentUser"] = this._currentUser;
			}
		}

		protected void ReloadCurrentUser()
		{
			this.SetCurrentUser();
		}
	}
}
