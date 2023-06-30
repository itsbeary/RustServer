using System;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
public class AddToHeightMap : ProceduralObject
{
	// Token: 0x06003259 RID: 12889 RVA: 0x001367FC File Offset: 0x001349FC
	public void Apply()
	{
		Collider component = base.GetComponent<Collider>();
		Bounds bounds = component.bounds;
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.x));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.z));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.z));
		for (int i = num3; i <= num4; i++)
		{
			float num5 = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num2; j++)
			{
				float num6 = TerrainMeta.HeightMap.Coordinate(j);
				Vector3 vector = new Vector3(TerrainMeta.DenormalizeX(num6), bounds.max.y, TerrainMeta.DenormalizeZ(num5));
				Ray ray = new Ray(vector, Vector3.down);
				RaycastHit raycastHit;
				if (component.Raycast(ray, out raycastHit, bounds.size.y))
				{
					float num7 = TerrainMeta.NormalizeY(raycastHit.point.y);
					float height = TerrainMeta.HeightMap.GetHeight01(j, i);
					if (num7 > height)
					{
						TerrainMeta.HeightMap.SetHeight(j, i, num7);
					}
				}
			}
		}
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x0013694F File Offset: 0x00134B4F
	public override void Process()
	{
		this.Apply();
		if (this.DestroyGameObject)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x04002941 RID: 10561
	public bool DestroyGameObject;
}
