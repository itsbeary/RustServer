using System;
using UnityEngine;

// Token: 0x020005A8 RID: 1448
public class WaterResource
{
	// Token: 0x06002C30 RID: 11312 RVA: 0x0010BA58 File Offset: 0x00109C58
	public static ItemDefinition GetAtPoint(Vector3 pos)
	{
		return ItemManager.FindItemDefinition(WaterResource.IsFreshWater(pos) ? "water" : "water.salt");
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x0010BA73 File Offset: 0x00109C73
	public static bool IsFreshWater(Vector3 pos)
	{
		return !(TerrainMeta.TopologyMap == null) && TerrainMeta.TopologyMap.GetTopology(pos, 245760);
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x0010BA94 File Offset: 0x00109C94
	public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
	{
		if (first == second)
		{
			return first;
		}
		if (first.shortname == "water.salt" || second.shortname == "water.salt")
		{
			return ItemManager.FindItemDefinition("water.salt");
		}
		return ItemManager.FindItemDefinition("water");
	}
}
