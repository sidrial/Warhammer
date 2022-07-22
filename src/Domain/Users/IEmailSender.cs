using System.Threading.Tasks;

namespace Warhammer.Domain.Users;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}