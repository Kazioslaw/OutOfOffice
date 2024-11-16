using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp
{
	public enum Status
	{
		New,
		Approved,
		Rejected,
		Submitted,
		Cancelled
	}

	public class Utilities
	{

		private readonly UserManager<User> _userManager;

		public Utilities(UserManager<User> user)
		{
			_userManager = user;
		}

		public SelectList CreateSelectList(IEnumerable<object> items, string dataValue, string dataText)
		{
			return new SelectList(items, dataValue, dataText);
		}

		public async Task<string> GenerateBasename(string fullName)
		{
			return fullName.Replace(" ", "_").Replace("ą", "a")
					.Replace("ę", "ę")
					.Replace("ł", "l")
					.Replace("ń", "n")
					.Replace("ó", "o")
					.Replace("ś", "s")
					.Replace("ż", "z")
					.Replace("ź", "z")
					.Replace("Ą", "A")
					.Replace("Ę", "E")
					.Replace("Ł", "L")
					.Replace("Ń", "N")
					.Replace("Ó", "O")
					.Replace("Ś", "S")
					.Replace("Ż", "Z")
					.Replace("Ź", "Z");
		}

		public async Task<string> GenerateUsername(string fullName)
		{
			var basename = await GenerateBasename(fullName);
			var username = basename;
			var counter = 1;
			var userExist = await _userManager.FindByNameAsync(fullName) != null;
			while (userExist)
			{
				username = $"{basename}{counter}";
				counter++;
				userExist = await _userManager.FindByEmailAsync(fullName) != null;
			};

			return username;
		}

	}
}
