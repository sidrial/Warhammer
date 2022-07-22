using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warhammer.Application.Extensions;
using Warhammer.Application.Models.IdentityViewModels;
using Warhammer.Application.Resources;
using Warhammer.Domain.Users;
using User = Warhammer.Domain.Users.Entities.User;

namespace Warhammer.Application.Controllers
{
    [Route("identity")]
    public class IdentityController : Controller
    {
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager { get; }
        private IEmailSender EmailSender { get; }
        private IUserRepo UserRepo { get; }

        private User CurrentUser { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.CurrentUser = this.UserManager.GetUserAsync(this.HttpContext.User).GetAwaiter().GetResult();
            if (this.CurrentUser != null)
            {
                this.HttpContext.Items["CurrentUser"] = this.CurrentUser;
            }
        }

        public IdentityController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            IUserRepo userRepo)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.EmailSender = emailSender;
            this.UserRepo = userRepo;
        }

        [HttpGet("change-password")]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var result = await this.UserManager.ChangePasswordAsync(this.CurrentUser, model.OldPassword, model.Password);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError("ChangePasswordAsync", result.Errors.FirstOrDefault()?.Description);
                return this.View();
            }

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("login")]
        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid) return this.View(model); // model invalid

            // first get user by email
            var user = await this.UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return this.View(model);
            }

            if (!user.EmailConfirmed)
            {
                this.ModelState.AddModelError(string.Empty, "Email address not confirmed.");
                return this.View(model);
            }

            var result = await this.SignInManager
                .PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false)
                .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return this.View(model);
            }

            if (!string.IsNullOrEmpty(returnUrl)) return this.Redirect(returnUrl);

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.SignInManager.SignOutAsync().ConfigureAwait(false);

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("register")]
        public ActionResult Register()
        {
            return this.View();
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model); // model invalid

            // first check if user email is already registered
            var user = await this.UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user != null)
            {
                this.ModelState.AddModelError(string.Empty, "Email address already registered.");
                return this.View(model);
            }

            var newUser = new User(model.Email, model.PlayerName);

            var createUser = await this.UserManager.CreateAsync(newUser)
                .ConfigureAwait(false);

            if (!createUser.Succeeded)
            {
                this.AddErrors(createUser);
                return this.View(model);
            }

            // Identity framework will hash given password before saving it.
            var savePassword = await this.UserManager.AddPasswordAsync(newUser, model.Password)
                .ConfigureAwait(false);

            if (!savePassword.Succeeded)
            {
	            await this.UserManager.DeleteAsync(newUser);
                this.AddErrors(savePassword);
                return this.View(model);
            }
            
            var code = await this.UserManager.GenerateEmailConfirmationTokenAsync(newUser).ConfigureAwait(false);
            var callbackUrl = this.Url.EmailConfirmationLink(newUser.Id, code, this.Request.Scheme);

            // send confirmation email
            var htmlMessage = Email.Greeting + "<br><br>" +
                              Email.AccountCreatedIntroduction + "<br>" +
                              Email.AccountCreatedConfirm + "<br>" +
                              $"<a href='{callbackUrl}'>" + Email.AccountCreatedAction + "</a><br><br>";

            await this.EmailSender.SendEmailAsync(newUser.Email, "De Dobbelsteen 40k Groep: Account created", htmlMessage).ConfigureAwait(false);

            return this.RedirectToAction(nameof(IdentityController.RegisterConfirmation), "Identity");
        }

        [HttpGet("register-confirmation")]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            return this.View();
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await this.UserManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            // Will confirm email, if successful also saves to db
            var result = await this.UserManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);

            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return this.View();
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.RedirectToAction(nameof(this.ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await this.UserManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                var callbackUrl = this.Url.ResetPasswordCallbackLink(user.Id, code, this.Request.Scheme);
                await this.EmailSender.SendEmailAsync(model.Email, "De Dobbelsteen 40k Groep: Reset Password",
                    Email.Greeting + "<br><br>" +
                    Email.PasswordResetInstruction + "<br>" +
                    $"<a href='{callbackUrl}'>" + Email.PasswordResetAction + "</a><br><br>" +
                    Email.PasswordResetNoActionRequired + "<br><br>" +
                    Email.PasswordResetExpiration + "<br><br>").ConfigureAwait(false);
                return this.RedirectToAction(nameof(this.ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        [HttpGet("forgot-password-confirmation")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        [HttpGet("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return this.View(model);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var user = await this.UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToAction(nameof(this.ResetPasswordConfirmation));
            }

            var result = await this.UserManager.ResetPasswordAsync(user, model.Code, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return this.RedirectToAction(nameof(this.ResetPasswordConfirmation));
            }

            this.AddErrors(result);
            return this.View();
        }

        [HttpGet("reset-password-confirmation")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }


        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return this.View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
