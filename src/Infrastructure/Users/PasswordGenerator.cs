using System.Security.Cryptography;
using System.Text;
using Warhammer.Domain.Users;

namespace Warhammer.Infrastructure.Users
{
	public class PasswordGenerator : IPasswordGenerator
	{
		private const string Lower = "abcdefghijklmnopqrstuvwxyz";
		private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string Digit = "0123456789";
		private const string NonAlphanumeric = "!#$%";
		private const string All = Lower + Upper + Digit + NonAlphanumeric;

		private readonly RNGCryptoServiceProvider _rngProvider = new();

		public string GeneratePassword(int length)
		{
			var password = this.GenerateInitialPassword(length - 4);

			// Insert a random character from each category to comply with the password policy.
			password = this.InsertRandomCharacter(password, Lower);
			password = this.InsertRandomCharacter(password, Upper);
			password = this.InsertRandomCharacter(password, Digit);
			password = this.InsertRandomCharacter(password, NonAlphanumeric);

			return password;
		}

		private string GenerateInitialPassword(int length)
		{
			var randomBytes = new byte[length];
			this._rngProvider.GetBytes(randomBytes);
			var randomIntegers = Array.ConvertAll(randomBytes, Convert.ToInt32);

			var initialPassword = new StringBuilder();

			foreach (var randomInteger in randomIntegers)
			{
				var position = randomInteger % All.Length;
				initialPassword.Append(All[position]);
			}

			return initialPassword.ToString();
		}

		private string InsertRandomCharacter(string target, string possibleCharacters)
		{
			var randomBytes = new byte[2];
			this._rngProvider.GetBytes(randomBytes);

			var randomIndex = Convert.ToInt32(randomBytes[0]) % target.Length;
			var randomChar = Convert.ToInt32(randomBytes[1]) % possibleCharacters.Length;

			return target.Insert(randomIndex, possibleCharacters[randomChar].ToString());
		}
	}
}
