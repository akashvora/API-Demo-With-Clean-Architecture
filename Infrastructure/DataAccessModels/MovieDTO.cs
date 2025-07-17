using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessModels
{
	public partial class MovieDTO
	{
		public required Guid Id { get; init; }
		public required string Title { get; set; }
		public string Slug => GenerateSlug();

		public required int YearOfRelease { get; set; }
		public required List<string> Genres { get; init; } = new();
		private string GenerateSlug()
		{
			var SluggedTitle = SlugRegex().Replace(Title, string.Empty)
					.ToLower().Replace(" ", "-");
			return $"{SluggedTitle}-{YearOfRelease}";
		}

		[GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking,5)]
		private static partial Regex SlugRegex();
	}
}
