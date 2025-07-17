using System;
using System.Text.RegularExpressions;

namespace Movies.Domain.Models
{
	public partial class Movie
	{
		public required Guid Id { get; init; }
		public required string Title { get; set; }
		public string Slug => GenerateSlug();

		public required int YearOfRelease { get; set; }
		public required List<string> Genres { get; init; } = new();
		//private string GenerateSlug()
		//{
		//	var SluggedTitle = SlugRegex().Replace(Title, string.Empty)
		//			.ToLower().Replace(" ", "-");
		//	return $"{SluggedTitle}-{YearOfRelease}";
		//}

		//[GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking,5)]
		//private static partial Regex SlugRegex();

		private string GenerateSlug()
		{
			var SluggedTitle = SlugRegex().Replace(Title, string.Empty)
					.ToLower().Replace(" ", "-");
			return $"{SluggedTitle}-{YearOfRelease}";
		}

		[GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
		private static partial Regex SlugRegex();
		public float? Rating { get; set; }
		public int? UserRating { get; set; }

	}
}
