using System;
using UnityEngine;

// Token: 0x02000672 RID: 1650
[RequireComponent(typeof(DungeonBaseLink))]
public class DungeonBaseLandmarkInfo : LandmarkInfo
{
	// Token: 0x06002FDF RID: 12255 RVA: 0x00120497 File Offset: 0x0011E697
	protected override void Awake()
	{
		base.Awake();
		this.baseLink = base.GetComponent<DungeonBaseLink>();
	}

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06002FE0 RID: 12256 RVA: 0x001204AC File Offset: 0x0011E6AC
	public override MapLayer MapLayer
	{
		get
		{
			if (this.layer != null)
			{
				return this.layer.Value;
			}
			DungeonBaseInfo dungeonBaseInfo = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, this.baseLink.transform.position);
			if (dungeonBaseInfo == null)
			{
				Debug.LogWarning("Couldn't determine which underwater lab a DungeonBaseLandmarkInfo belongs to", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
				return this.layer.Value;
			}
			int num = -1;
			for (int i = 0; i < dungeonBaseInfo.Floors.Count; i++)
			{
				if (dungeonBaseInfo.Floors[i].Links.Contains(this.baseLink))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				this.layer = new MapLayer?(MapLayer.Underwater1 + num);
			}
			else
			{
				Debug.LogWarning("Couldn't determine the floor of a DungeonBaseLandmarkInfo", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
			}
			return this.layer.Value;
		}
	}

	// Token: 0x0400274C RID: 10060
	private DungeonBaseLink baseLink;

	// Token: 0x0400274D RID: 10061
	private MapLayer? layer;
}
