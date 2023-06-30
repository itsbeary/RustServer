using System;
using UnityEngine;

// Token: 0x020006FB RID: 1787
public static class TerrainModifierEx
{
	// Token: 0x0600327A RID: 12922 RVA: 0x00137388 File Offset: 0x00135588
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		foreach (TerrainModifier terrainModifier in modifiers)
		{
			Vector3 vector = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 vector2 = pos + rot * vector;
			float y = scale.y;
			terrainModifier.Apply(vector2, y);
		}
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x001373D1 File Offset: 0x001355D1
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}
