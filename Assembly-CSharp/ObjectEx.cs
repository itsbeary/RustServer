using System;

// Token: 0x02000928 RID: 2344
public static class ObjectEx
{
	// Token: 0x06003849 RID: 14409 RVA: 0x0014F2A6 File Offset: 0x0014D4A6
	public static bool IsUnityNull<T>(this T obj) where T : class
	{
		return obj == null || obj.Equals(null);
	}
}
