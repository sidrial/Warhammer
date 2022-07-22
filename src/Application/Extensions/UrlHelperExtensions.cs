using Microsoft.AspNetCore.Mvc;
using Warhammer.Application.Controllers;

namespace Warhammer.Application.Extensions
{
	public static class UrlHelperExtensions
	{
		public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action(
				action: nameof(IdentityController.ConfirmEmail),
				controller: "Identity",
				values: new { userId, code },
				protocol: scheme);
		}

		public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code,
			string scheme)
		{
			return urlHelper.Action(
				action: nameof(IdentityController.ResetPassword),
				controller: "Identity",
				values: new { userId, code },
				protocol: scheme);
		}
	}
}
