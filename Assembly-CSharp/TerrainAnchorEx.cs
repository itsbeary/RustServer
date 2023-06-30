using System;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public static class TerrainAnchorEx
{
	// Token: 0x06003028 RID: 12328 RVA: 0x0012198A File Offset: 0x0011FB8A
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x0012199C File Offset: 0x0011FB9C
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		float num = 0f;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		foreach (TerrainAnchor terrainAnchor in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainAnchor.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			float num4;
			float num5;
			float num6;
			terrainAnchor.Apply(out num4, out num5, out num6, vector2, scale);
			num += num4 - vector.y;
			num2 = Mathf.Max(num2, num5 - vector.y);
			num3 = Mathf.Min(num3, num6 - vector.y);
			if (num3 < num2)
			{
				return false;
			}
		}
		if (num3 > 1f && num2 < 1f)
		{
			num2 = 1f;
		}
		if (mode == TerrainAnchorMode.MinimizeError)
		{
			pos.y = Mathf.Clamp(num / (float)anchors.Length, num2, num3);
		}
		else
		{
			pos.y = Mathf.Clamp(pos.y, num2, num3);
		}
		return true;
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x00121AB0 File Offset: 0x0011FCB0
	public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
	{
		Vector3 position = transform.position;
		transform.ApplyTerrainAnchors(anchors, ref position, transform.rotation, transform.lossyScale, null);
		transform.position = position;
	}
}
