using System;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class AddToAlphaMap : ProceduralObject
{
	// Token: 0x06003257 RID: 12887 RVA: 0x00136728 File Offset: 0x00134928
	public override void Process()
	{
		OBB obb = new OBB(base.transform, this.bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		TerrainMeta.AlphaMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
		{
			TerrainMeta.AlphaMap.SetAlpha(x, z, 0f);
		});
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x04002940 RID: 10560
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
