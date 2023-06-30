using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000673 RID: 1651
public class DungeonBaseLink : MonoBehaviour
{
	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x06002FE2 RID: 12258 RVA: 0x001205A7 File Offset: 0x0011E7A7
	internal List<DungeonBaseSocket> Sockets
	{
		get
		{
			if (this.sockets == null)
			{
				this.sockets = new List<DungeonBaseSocket>();
				base.GetComponentsInChildren<DungeonBaseSocket>(true, this.sockets);
			}
			return this.sockets;
		}
	}

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002FE3 RID: 12259 RVA: 0x001205CF File Offset: 0x0011E7CF
	internal List<DungeonVolume> Volumes
	{
		get
		{
			if (this.volumes == null)
			{
				this.volumes = new List<DungeonVolume>();
				base.GetComponentsInChildren<DungeonVolume>(true, this.volumes);
			}
			return this.volumes;
		}
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x001205F8 File Offset: 0x0011E7F8
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		this.Dungeon = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, base.transform.position);
		if (this.Dungeon == null)
		{
			return;
		}
		this.Dungeon.Add(this);
	}

	// Token: 0x0400274E RID: 10062
	public DungeonBaseLinkType Type;

	// Token: 0x0400274F RID: 10063
	public int Cost = 1;

	// Token: 0x04002750 RID: 10064
	public int MaxFloor = -1;

	// Token: 0x04002751 RID: 10065
	public int MaxCountLocal = -1;

	// Token: 0x04002752 RID: 10066
	public int MaxCountGlobal = -1;

	// Token: 0x04002753 RID: 10067
	[Tooltip("If set to a positive number, all segments with the same MaxCountIdentifier are counted towards MaxCountLocal and MaxCountGlobal")]
	public int MaxCountIdentifier = -1;

	// Token: 0x04002754 RID: 10068
	internal DungeonBaseInfo Dungeon;

	// Token: 0x04002755 RID: 10069
	public MeshRenderer[] MapRenderers;

	// Token: 0x04002756 RID: 10070
	private List<DungeonBaseSocket> sockets;

	// Token: 0x04002757 RID: 10071
	private List<DungeonVolume> volumes;
}
