namespace Warhammer.Domain.Users
{
	public interface IPasswordGenerator
	{
		string GeneratePassword(int length);
	}
}
