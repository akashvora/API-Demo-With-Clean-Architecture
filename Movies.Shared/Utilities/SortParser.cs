using Movies.Shared.Enums;

namespace Movies.Shared.Utilities
{
	public static class SortParser
	{
		public static (string? sortField, SortOrder sortOrder) ParseSortBy(string? sortBy)
		{
			if (string.IsNullOrWhiteSpace(sortBy))
				return (null, SortOrder.Unsorted);

			var field = sortBy.Trim('+', '-').Trim();
			var order = sortBy.StartsWith('-')
				? SortOrder.Descending
				: SortOrder.Ascending;

			return (field, order);
		}
	}

}
