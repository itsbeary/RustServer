using System;
using UnityEngine;

// Token: 0x020006FC RID: 1788
public class TerrainPlacementBlocked : TerrainModifier
{
	// Token: 0x0600327C RID: 12924 RVA: 0x001373EC File Offset: 0x001355EC
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.PlacementMap)
		{
			return;
		}
		TerrainMeta.PlacementMap.SetBlocked(position, radius, fade);
	}
}
