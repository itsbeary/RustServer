using System;
using UnityEngine;

// Token: 0x02000568 RID: 1384
public class RiverInfo : MonoBehaviour
{
	// Token: 0x06002A9E RID: 10910 RVA: 0x00103B09 File Offset: 0x00101D09
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.RiverObjs.Add(this);
		}
	}
}
