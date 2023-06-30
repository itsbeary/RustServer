using System;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x020009D1 RID: 2513
	public static class Extensions
	{
		// Token: 0x06003BF3 RID: 15347 RVA: 0x00161BE4 File Offset: 0x0015FDE4
		public static bool AnyOfType<TSource>(this IEnumerable<TSource> source, Type expectedType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (expectedType == null)
			{
				throw new ArgumentNullException("expectedType");
			}
			foreach (TSource tsource in source)
			{
				if (expectedType.IsInstanceOfType(tsource))
				{
					return true;
				}
			}
			return false;
		}
	}
}
