using System;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class TerrainTopologySet : TerrainModifier
{
	// Token: 0x06003282 RID: 12930 RVA: 0x00137463 File Offset: 0x00135663
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.SetTopology(position, (int)this.TopologyType, radius, fade);
	}

	// Token: 0x0400295B RID: 10587
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;
}
