using System;
using System.Collections.Generic;

// Token: 0x02000926 RID: 2342
public static class LinqEx
{
	// Token: 0x06003847 RID: 14407 RVA: 0x0014EF00 File Offset: 0x0014D100
	public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
	{
		int num = -1;
		T t = default(T);
		int num2 = 0;
		foreach (T t2 in sequence)
		{
			if (t2.CompareTo(t) > 0 || num == -1)
			{
				num = num2;
				t = t2;
			}
			num2++;
		}
		return num;
	}
}
