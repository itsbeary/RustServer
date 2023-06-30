using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074E RID: 1870
public static class AssetNameCache
{
	// Token: 0x06003438 RID: 13368 RVA: 0x00142CDC File Offset: 0x00140EDC
	private static string LookupName(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string name;
		if (!AssetNameCache.mixed.TryGetValue(obj, out name))
		{
			name = obj.name;
			AssetNameCache.mixed.Add(obj, name);
		}
		return name;
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x00142D1C File Offset: 0x00140F1C
	private static string LookupNameLower(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.lower.TryGetValue(obj, out text))
		{
			text = obj.name.ToLower();
			AssetNameCache.lower.Add(obj, text);
		}
		return text;
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x00142D60 File Offset: 0x00140F60
	private static string LookupNameUpper(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.upper.TryGetValue(obj, out text))
		{
			text = obj.name.ToUpper();
			AssetNameCache.upper.Add(obj, text);
		}
		return text;
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x00142DA4 File Offset: 0x00140FA4
	public static string GetName(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x00142DAC File Offset: 0x00140FAC
	public static string GetNameLower(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x0600343D RID: 13373 RVA: 0x00142DB4 File Offset: 0x00140FB4
	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x00142DA4 File Offset: 0x00140FA4
	public static string GetName(this Material mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x00142DAC File Offset: 0x00140FAC
	public static string GetNameLower(this Material mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x00142DB4 File Offset: 0x00140FB4
	public static string GetNameUpper(this Material mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x04002AA7 RID: 10919
	private static Dictionary<UnityEngine.Object, string> mixed = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x04002AA8 RID: 10920
	private static Dictionary<UnityEngine.Object, string> lower = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x04002AA9 RID: 10921
	private static Dictionary<UnityEngine.Object, string> upper = new Dictionary<UnityEngine.Object, string>();
}
