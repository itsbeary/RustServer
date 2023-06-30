using System;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
public class ApplyTerrainModifiers : MonoBehaviour
{
	// Token: 0x0600326E RID: 12910 RVA: 0x00137264 File Offset: 0x00135464
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainModifier[] array = null;
		if (component.isServer)
		{
			array = PrefabAttribute.server.FindAll<TerrainModifier>(component.prefabID);
		}
		base.transform.ApplyTerrainModifiers(array);
		GameManager.Destroy(this, 0f);
	}
}
