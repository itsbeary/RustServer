using System;
using UnityEngine;

// Token: 0x0200065E RID: 1630
[Serializable]
public class SpawnFilter
{
	// Token: 0x06002F48 RID: 12104 RVA: 0x0011DD43 File Offset: 0x0011BF43
	public bool Test(Vector3 worldPos)
	{
		return this.GetFactor(worldPos, true) > 0.5f;
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x0011DD54 File Offset: 0x0011BF54
	public bool Test(float normX, float normZ)
	{
		return this.GetFactor(normX, normZ, true) > 0.5f;
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x0011DD68 File Offset: 0x0011BF68
	public float GetFactor(Vector3 worldPos, bool checkPlacementMap = true)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetFactor(num, num2, checkPlacementMap);
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x0011DD98 File Offset: 0x0011BF98
	public float GetFactor(float normX, float normZ, bool checkPlacementMap = true)
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return 0f;
		}
		if (checkPlacementMap && TerrainMeta.PlacementMap != null && TerrainMeta.PlacementMap.GetBlocked(normX, normZ))
		{
			return 0f;
		}
		int splatType = (int)this.SplatType;
		int biomeType = (int)this.BiomeType;
		int topologyAny = (int)this.TopologyAny;
		int topologyAll = (int)this.TopologyAll;
		int topologyNot = (int)this.TopologyNot;
		if (topologyAny == 0)
		{
			Debug.LogError("Empty topology filter is invalid.");
		}
		else if (topologyAny != -1 || topologyAll != 0 || topologyNot != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (topologyAny != -1 && (topology & topologyAny) == 0)
			{
				return 0f;
			}
			if (topologyNot != 0 && (topology & topologyNot) != 0)
			{
				return 0f;
			}
			if (topologyAll != 0 && (topology & topologyAll) != topologyAll)
			{
				return 0f;
			}
		}
		if (biomeType == 0)
		{
			Debug.LogError("Empty biome filter is invalid.");
		}
		else if (biomeType != -1 && (TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
		{
			return 0f;
		}
		if (splatType == 0)
		{
			Debug.LogError("Empty splat filter is invalid.");
		}
		else if (splatType != -1)
		{
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
		}
		return 1f;
	}

	// Token: 0x040026FA RID: 9978
	[InspectorFlags]
	public TerrainSplat.Enum SplatType = (TerrainSplat.Enum)(-1);

	// Token: 0x040026FB RID: 9979
	[InspectorFlags]
	public TerrainBiome.Enum BiomeType = (TerrainBiome.Enum)(-1);

	// Token: 0x040026FC RID: 9980
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAny = (TerrainTopology.Enum)(-1);

	// Token: 0x040026FD RID: 9981
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAll;

	// Token: 0x040026FE RID: 9982
	[InspectorFlags]
	public TerrainTopology.Enum TopologyNot;
}
