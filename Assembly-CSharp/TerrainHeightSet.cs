using System;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class TerrainHeightSet : TerrainModifier
{
	// Token: 0x06003274 RID: 12916 RVA: 0x00137318 File Offset: 0x00135518
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.SetHeight(position, opacity, radius, fade);
	}
}
