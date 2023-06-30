using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200092B RID: 2347
public static class RendererEx
{
	// Token: 0x06003854 RID: 14420 RVA: 0x0014F410 File Offset: 0x0014D610
	public static void SetSharedMaterials(this Renderer renderer, List<Material> materials)
	{
		if (materials.Count == 0)
		{
			return;
		}
		if (materials.Count > 10)
		{
			throw new ArgumentOutOfRangeException("materials");
		}
		Material[] array = RendererEx.ArrayCache.Get(materials.Count);
		for (int i = 0; i < materials.Count; i++)
		{
			array[i] = materials[i];
		}
		renderer.sharedMaterials = array;
	}

	// Token: 0x04003399 RID: 13209
	private static readonly Memoized<Material[], int> ArrayCache = new Memoized<Material[], int>((int n) => new Material[n]);
}
