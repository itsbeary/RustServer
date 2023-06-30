using System;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
public class TerrainFilter : PrefabAttribute
{
	// Token: 0x060031AB RID: 12715 RVA: 0x001292E0 File Offset: 0x001274E0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * 50f * 0.5f, new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(base.transform.position + Vector3.up * 50f, 2f);
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x00129376 File Offset: 0x00127576
	public bool Check(Vector3 pos)
	{
		return this.Filter.GetFactor(pos, this.CheckPlacementMap) > 0f;
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x00129391 File Offset: 0x00127591
	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}

	// Token: 0x0400285D RID: 10333
	public SpawnFilter Filter;

	// Token: 0x0400285E RID: 10334
	public bool CheckPlacementMap = true;
}
