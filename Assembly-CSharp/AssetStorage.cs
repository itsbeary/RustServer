using System;
using UnityEngine;

// Token: 0x02000935 RID: 2357
public static class AssetStorage
{
	// Token: 0x0600387D RID: 14461 RVA: 0x001504D7 File Offset: 0x0014E6D7
	public static void Save<T>(ref T asset, string path) where T : UnityEngine.Object
	{
		asset;
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Save(ref Texture2D asset)
	{
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x001504EA File Offset: 0x0014E6EA
	public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
	{
		asset;
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Load<T>(ref T asset, string path) where T : UnityEngine.Object
	{
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x001504F4 File Offset: 0x0014E6F4
	public static void Delete<T>(ref T asset) where T : UnityEngine.Object
	{
		if (!asset)
		{
			return;
		}
		UnityEngine.Object.Destroy(asset);
		asset = default(T);
	}
}
