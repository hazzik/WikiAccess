using System;
using System.Collections.Generic;

namespace WikiTools.Access.Extensions
{
	internal static class EnumerableExtensions
	{
		public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			if (enumerable is List<T>)
			{
				(enumerable as List<T>).ForEach(action);
			}
			else
			{
				foreach (T item in enumerable)
				{
					action(item);
				}
			}
		}
	}
}