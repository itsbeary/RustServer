using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000671 RID: 1649
[Serializable]
public class DungeonBaseFloor
{
	// Token: 0x06002FDC RID: 12252 RVA: 0x00120437 File Offset: 0x0011E637
	public float Distance(Vector3 position)
	{
		return Mathf.Abs(this.Links[0].transform.position.y - position.y);
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00120460 File Offset: 0x0011E660
	public float SignedDistance(Vector3 position)
	{
		return this.Links[0].transform.position.y - position.y;
	}

	// Token: 0x0400274B RID: 10059
	public List<DungeonBaseLink> Links = new List<DungeonBaseLink>();
}
