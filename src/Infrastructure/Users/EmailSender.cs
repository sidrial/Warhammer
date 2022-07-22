using SendGrid;
using SendGrid.Helpers.Mail;
using Warhammer.Domain.Users;

namespace Warhammer.Infrastructure.Users;

public class EmailSender : IEmailSender
{
	private SendGridClient Client { get; }
	private EmailAddress ReplyToEmail { get; }

	public EmailSender(string apiKey, string replyToEmail)
	{
		this.Client = new SendGridClient(apiKey);
		this.ReplyToEmail = new EmailAddress(replyToEmail, replyToEmail);
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var to = new EmailAddress(email, email);
		var htmlContent = htmlMessage;
		var msg = MailHelper.CreateSingleEmail(this.ReplyToEmail, to, subject, htmlContent, htmlContent);
		var response = await this.Client.SendEmailAsync(msg);
		if (!response.IsSuccessStatusCode) throw new Exception();
	}
}