using System;
using UnityEngine;

// Token: 0x020006FE RID: 1790
public class TerrainTopologyAdd : TerrainModifier
{
	// Token: 0x06003280 RID: 12928 RVA: 0x0013742D File Offset: 0x0013562D
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.AddTopology(position, (int)this.TopologyType, radius, fade);
	}

	// Token: 0x0400295A RID: 10586
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;
}
