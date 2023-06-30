using System;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class TerrainSplatSet : TerrainModifier
{
	// Token: 0x0600327E RID: 12926 RVA: 0x00137409 File Offset: 0x00135609
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.SplatMap)
		{
			return;
		}
		TerrainMeta.SplatMap.SetSplat(position, (int)this.SplatType, opacity, radius, fade);
	}

	// Token: 0x04002959 RID: 10585
	public TerrainSplat.Enum SplatType;
}
