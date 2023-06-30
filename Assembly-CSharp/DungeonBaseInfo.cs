using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class DungeonBaseInfo : LandmarkInfo
{
	// Token: 0x06002FD5 RID: 12245 RVA: 0x0012029C File Offset: 0x0011E49C
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x001202C4 File Offset: 0x0011E4C4
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x001202EC File Offset: 0x0011E4EC
	public void Add(DungeonBaseLink link)
	{
		this.Links.Add(link.gameObject);
		if (link.Type == DungeonBaseLinkType.End)
		{
			return;
		}
		DungeonBaseFloor dungeonBaseFloor = null;
		float num = float.MaxValue;
		for (int i = 0; i < this.Floors.Count; i++)
		{
			DungeonBaseFloor dungeonBaseFloor2 = this.Floors[i];
			float num2 = dungeonBaseFloor2.Distance(link.transform.position);
			if (num2 < 1f && num2 < num)
			{
				dungeonBaseFloor = dungeonBaseFloor2;
				num = num2;
			}
		}
		if (dungeonBaseFloor == null)
		{
			dungeonBaseFloor = new DungeonBaseFloor();
			dungeonBaseFloor.Links.Add(link);
			this.Floors.Add(dungeonBaseFloor);
			this.Floors.Sort((DungeonBaseFloor l, DungeonBaseFloor r) => l.SignedDistance(base.transform.position).CompareTo(r.SignedDistance(base.transform.position)));
			return;
		}
		dungeonBaseFloor.Links.Add(link);
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x001203AA File Offset: 0x0011E5AA
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonBaseEntrances.Add(this);
		}
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x001203CE File Offset: 0x0011E5CE
	protected void Start()
	{
		base.transform.SetHierarchyGroup("DungeonBase", true, false);
	}

	// Token: 0x04002749 RID: 10057
	internal List<GameObject> Links = new List<GameObject>();

	// Token: 0x0400274A RID: 10058
	internal List<DungeonBaseFloor> Floors = new List<DungeonBaseFloor>();
}
