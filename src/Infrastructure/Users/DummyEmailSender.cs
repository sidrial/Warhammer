using SendGrid.Helpers.Mail;
using Warhammer.Domain.Users;

namespace Warhammer.Infrastructure.Users;

public class DummyEmailSender : IEmailSender
{
	private EmailAddress ReplyToEmail { get; }

	public DummyEmailSender(string replyToEmail)
	{
		this.ReplyToEmail = new EmailAddress(replyToEmail, replyToEmail);
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var to = new EmailAddress(email, email);
		var htmlContent = htmlMessage;
		var msg = MailHelper.CreateSingleEmail(this.ReplyToEmail, to, subject, htmlContent, htmlContent);
	}
}