using System;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
public class TerrainHeightAdd : TerrainModifier
{
	// Token: 0x06003272 RID: 12914 RVA: 0x001372D5 File Offset: 0x001354D5
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.AddHeight(position, opacity * this.Delta * TerrainMeta.OneOverSize.y, radius, fade);
	}

	// Token: 0x04002955 RID: 10581
	public float Delta = 1f;
}
