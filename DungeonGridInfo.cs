using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class DungeonGridInfo : LandmarkInfo
{
	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x06002FEC RID: 12268 RVA: 0x00120717 File Offset: 0x0011E917
	public float MinDistance
	{
		get
		{
			return (float)this.CellSize * 2f;
		}
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x00120728 File Offset: 0x0011E928
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x00120750 File Offset: 0x0011E950
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x00120778 File Offset: 0x0011E978
	public bool IsValidSpawnPosition(Vector3 position)
	{
		OBB bounds = base.GetComponentInChildren<DungeonVolume>().GetBounds(position, Quaternion.identity);
		Vector3 vector = WorldSpaceGrid.ClosestGridCell(bounds.position, TerrainMeta.Size.x * 2f, (float)this.CellSize);
		Vector3 vector2 = bounds.position - vector;
		return Mathf.Abs(vector2.x) > 3f || Mathf.Abs(vector2.z) > 3f;
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x001207EC File Offset: 0x0011E9EC
	public Vector3 SnapPosition(Vector3 pos)
	{
		pos.x = (float)Mathf.RoundToInt(pos.x / this.LinkRadius) * this.LinkRadius;
		pos.y = (float)Mathf.CeilToInt(pos.y / this.LinkHeight) * this.LinkHeight;
		pos.z = (float)Mathf.RoundToInt(pos.z / this.LinkRadius) * this.LinkRadius;
		return pos;
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x0012085D File Offset: 0x0011EA5D
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridEntrances.Add(this);
		}
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x00120881 File Offset: 0x0011EA81
	protected void Start()
	{
		base.transform.SetHierarchyGroup("Dungeon", true, false);
	}

	// Token: 0x0400277A RID: 10106
	[Header("DungeonGridInfo")]
	public int CellSize = 216;

	// Token: 0x0400277B RID: 10107
	public float LinkHeight = 1.5f;

	// Token: 0x0400277C RID: 10108
	public float LinkRadius = 3f;

	// Token: 0x0400277D RID: 10109
	internal List<GameObject> Links = new List<GameObject>();
}
