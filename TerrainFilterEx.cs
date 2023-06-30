using System;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public static class TerrainFilterEx
{
	// Token: 0x060031AF RID: 12719 RVA: 0x001293AC File Offset: 0x001275AC
	public static bool ApplyTerrainFilters(this Transform transform, TerrainFilter[] filters, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter globalFilter = null)
	{
		if (filters.Length == 0)
		{
			return true;
		}
		foreach (TerrainFilter terrainFilter in filters)
		{
			Vector3 vector = Vector3.Scale(terrainFilter.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (globalFilter != null && globalFilter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			if (!terrainFilter.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
