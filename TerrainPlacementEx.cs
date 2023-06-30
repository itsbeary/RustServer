using System;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public static class TerrainPlacementEx
{
	// Token: 0x060032A3 RID: 12963 RVA: 0x00138454 File Offset: 0x00136654
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (placements.Length == 0)
		{
			return;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(pos, rot, scale);
		Matrix4x4 inverse = matrix4x.inverse;
		for (int i = 0; i < placements.Length; i++)
		{
			placements[i].Apply(matrix4x, inverse);
		}
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x00138490 File Offset: 0x00136690
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}
