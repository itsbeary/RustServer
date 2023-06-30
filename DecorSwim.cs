using System;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class DecorSwim : DecorComponent
{
	// Token: 0x06002FCF RID: 12239 RVA: 0x0012013C File Offset: 0x0011E33C
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos.y = TerrainMeta.WaterMap.GetHeight(pos);
	}
}
