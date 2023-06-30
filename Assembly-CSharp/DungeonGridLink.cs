using System;
using UnityEngine;

// Token: 0x0200067D RID: 1661
public class DungeonGridLink : MonoBehaviour
{
	// Token: 0x06002FF4 RID: 12276 RVA: 0x001208CC File Offset: 0x0011EACC
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		DungeonGridInfo dungeonGridInfo = TerrainMeta.Path.FindClosest<DungeonGridInfo>(TerrainMeta.Path.DungeonGridEntrances, base.transform.position);
		if (dungeonGridInfo == null)
		{
			return;
		}
		dungeonGridInfo.Links.Add(base.gameObject);
	}

	// Token: 0x0400277E RID: 10110
	public Transform UpSocket;

	// Token: 0x0400277F RID: 10111
	public Transform DownSocket;

	// Token: 0x04002780 RID: 10112
	public DungeonGridLinkType UpType;

	// Token: 0x04002781 RID: 10113
	public DungeonGridLinkType DownType;

	// Token: 0x04002782 RID: 10114
	public int Priority;

	// Token: 0x04002783 RID: 10115
	public int Rotation;
}
