using System;
using UnityEngine;

// Token: 0x0200070F RID: 1807
public class WaterCheck : PrefabAttribute
{
	// Token: 0x060032F1 RID: 13041 RVA: 0x00139081 File Offset: 0x00137281
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}

	// Token: 0x060032F2 RID: 13042 RVA: 0x001390B6 File Offset: 0x001372B6
	public bool Check(Vector3 pos)
	{
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	// Token: 0x060032F3 RID: 13043 RVA: 0x001390CE File Offset: 0x001372CE
	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}

	// Token: 0x040029A7 RID: 10663
	public bool Rotate = true;
}
