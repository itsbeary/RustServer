using System;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public class ApplyTerrainAnchors : MonoBehaviour
{
	// Token: 0x06003023 RID: 12323 RVA: 0x001217F8 File Offset: 0x0011F9F8
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainAnchor[] array = null;
		if (component.isServer)
		{
			array = PrefabAttribute.server.FindAll<TerrainAnchor>(component.prefabID);
		}
		base.transform.ApplyTerrainAnchors(array);
		GameManager.Destroy(this, 0f);
	}
}
