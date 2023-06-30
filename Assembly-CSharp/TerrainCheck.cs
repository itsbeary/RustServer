using System;
using UnityEngine;

// Token: 0x02000698 RID: 1688
public class TerrainCheck : PrefabAttribute
{
	// Token: 0x0600302C RID: 12332 RVA: 0x00121B24 File Offset: 0x0011FD24
	public bool Check(Vector3 pos)
	{
		float extents = this.Extents;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float num = pos.y - extents;
		float num2 = pos.y + extents;
		return num <= height && num2 >= height;
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x00121B61 File Offset: 0x0011FD61
	protected override Type GetIndexedType()
	{
		return typeof(TerrainCheck);
	}

	// Token: 0x040027D1 RID: 10193
	public bool Rotate = true;

	// Token: 0x040027D2 RID: 10194
	public float Extents = 1f;
}
