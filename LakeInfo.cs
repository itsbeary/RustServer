using System;
using UnityEngine;

// Token: 0x02000551 RID: 1361
public class LakeInfo : MonoBehaviour
{
	// Token: 0x06002A34 RID: 10804 RVA: 0x00101E77 File Offset: 0x00100077
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.LakeObjs.Add(this);
		}
	}
}
