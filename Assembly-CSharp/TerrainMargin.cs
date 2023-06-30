using System;
using UnityEngine;

// Token: 0x020006AD RID: 1709
public class TerrainMargin
{
	// Token: 0x06003132 RID: 12594 RVA: 0x00127268 File Offset: 0x00125468
	public static void Create()
	{
		Material marginMaterial = TerrainMeta.Config.MarginMaterial;
		Vector3 center = TerrainMeta.Center;
		Vector3 size = TerrainMeta.Size;
		Vector3 vector = new Vector3(size.x, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, size.z);
		center.y = TerrainMeta.HeightMap.GetHeight(0, 0);
		TerrainMargin.Create(center - vector2, size, marginMaterial);
		TerrainMargin.Create(center - vector2 - vector, size, marginMaterial);
		TerrainMargin.Create(center - vector2 + vector, size, marginMaterial);
		TerrainMargin.Create(center - vector, size, marginMaterial);
		TerrainMargin.Create(center + vector, size, marginMaterial);
		TerrainMargin.Create(center + vector2, size, marginMaterial);
		TerrainMargin.Create(center + vector2 - vector, size, marginMaterial);
		TerrainMargin.Create(center + vector2 + vector, size, marginMaterial);
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x0012735C File Offset: 0x0012555C
	private static void Create(Vector3 position, Vector3 size, Material material)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "TerrainMargin";
		gameObject.layer = 16;
		gameObject.transform.position = position;
		gameObject.transform.localScale = size * 0.1f;
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshFilter>());
	}

	// Token: 0x0400280F RID: 10255
	private static MaterialPropertyBlock materialPropertyBlock;
}
