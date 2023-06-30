using System;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
public class TerrainCarve : TerrainModifier
{
	// Token: 0x06003270 RID: 12912 RVA: 0x001372AA File Offset: 0x001354AA
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.AlphaMap)
		{
			return;
		}
		TerrainMeta.AlphaMap.SetAlpha(position, 0f, opacity, radius, fade);
	}
}
