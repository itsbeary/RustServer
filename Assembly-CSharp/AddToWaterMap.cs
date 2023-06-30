using System;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
public class AddToWaterMap : ProceduralObject
{
	// Token: 0x0600325C RID: 12892 RVA: 0x00136984 File Offset: 0x00134B84
	public override void Process()
	{
		Vector3 vector = new Vector3(10000f, 1000f, 10000f);
		Collider component = base.GetComponent<Collider>();
		Bounds bounds = (this.isOcean ? new Bounds(-vector / 2f, vector) : component.bounds);
		int num = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(bounds.min.x));
		int num2 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(bounds.max.x));
		int num3 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(bounds.min.z));
		int num4 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(bounds.max.z));
		if (this.isOcean || (component is BoxCollider && base.transform.rotation == Quaternion.identity))
		{
			float num5 = TerrainMeta.NormalizeY(bounds.max.y);
			for (int i = num3; i <= num4; i++)
			{
				for (int j = num; j <= num2; j++)
				{
					float height = TerrainMeta.WaterMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.WaterMap.SetHeight(j, i, num5);
					}
				}
			}
		}
		else
		{
			for (int k = num3; k <= num4; k++)
			{
				float num6 = TerrainMeta.WaterMap.Coordinate(k);
				for (int l = num; l <= num2; l++)
				{
					float num7 = TerrainMeta.WaterMap.Coordinate(l);
					Vector3 vector2 = new Vector3(TerrainMeta.DenormalizeX(num7), bounds.max.y + 1f, TerrainMeta.DenormalizeZ(num6));
					Ray ray = new Ray(vector2, Vector3.down);
					RaycastHit raycastHit;
					if (component.Raycast(ray, out raycastHit, bounds.size.y + 1f + 1f))
					{
						float num8 = TerrainMeta.NormalizeY(raycastHit.point.y);
						float height2 = TerrainMeta.WaterMap.GetHeight01(l, k);
						if (num8 > height2)
						{
							TerrainMeta.WaterMap.SetHeight(l, k, num8);
						}
					}
				}
			}
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x04002942 RID: 10562
	public bool isOcean;
}
