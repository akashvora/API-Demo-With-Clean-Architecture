using Movies.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Utilities
{
	public static class ListDataSortingHelper
	{
			public static IEnumerable<T> ApplySorting<T>(
				IEnumerable<T> source,
				string? propertyName,
				SortOrder sortOrder)
			{
				if (string.IsNullOrWhiteSpace(propertyName) || sortOrder == SortOrder.Unsorted)
					return source;

				PropertyInfo? prop = typeof(T).GetProperty(propertyName,
					BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				if (prop == null) return source;

				return sortOrder == SortOrder.Descending
					? source.OrderByDescending(x => prop.GetValue(x))
					: source.OrderBy(x => prop.GetValue(x));
			}
		}
}
