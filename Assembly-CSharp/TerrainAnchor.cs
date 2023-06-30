using System;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class TerrainAnchor : PrefabAttribute
{
	// Token: 0x06003025 RID: 12325 RVA: 0x00121840 File Offset: 0x0011FA40
	public void Apply(out float height, out float min, out float max, Vector3 pos, Vector3 scale)
	{
		float num = this.Extents * scale.y;
		float num2 = this.Offset * scale.y;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - num2 - num;
		max = height - num2 + num;
		if (this.Radius > 0f)
		{
			int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x - this.Radius));
			int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x + this.Radius));
			int num5 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z - this.Radius));
			int num6 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z + this.Radius));
			int num7 = num5;
			while (num7 <= num6 && max >= min)
			{
				int num8 = num3;
				while (num8 <= num4 && max >= min)
				{
					float height2 = TerrainMeta.HeightMap.GetHeight(num8, num7);
					min = Mathf.Max(min, height2 - num2 - num);
					max = Mathf.Min(max, height2 - num2 + num);
					num8++;
				}
				num7++;
			}
		}
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x0012196B File Offset: 0x0011FB6B
	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}

	// Token: 0x040027C3 RID: 10179
	public float Extents = 1f;

	// Token: 0x040027C4 RID: 10180
	public float Offset;

	// Token: 0x040027C5 RID: 10181
	public float Radius;
}
