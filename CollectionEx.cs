using System;
using System.Collections.Generic;

// Token: 0x02000923 RID: 2339
public static class CollectionEx
{
	// Token: 0x0600383D RID: 14397 RVA: 0x0014EB71 File Offset: 0x0014CD71
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x0014EB81 File Offset: 0x0014CD81
	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}
}
