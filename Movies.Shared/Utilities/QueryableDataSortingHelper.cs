using Movies.Shared.Enums;
using System.Linq.Dynamic.Core;

namespace Movies.Shared.Utilities
{
	public static class QueryableDataSortingHelper
	{
		
		public static IQueryable<T> ApplySorting<T>(
			this IQueryable<T> query,
			string? sortField,
			SortOrder sortOrder)
		{
			if (string.IsNullOrWhiteSpace(sortField) || sortOrder == SortOrder.Unsorted)
				return query;

			string direction = sortOrder == SortOrder.Descending ? "descending" : "ascending";
			return query.OrderBy($"{sortField} {direction}");
		}
	}
}
