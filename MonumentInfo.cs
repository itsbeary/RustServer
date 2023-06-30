using System;
using UnityEngine;

// Token: 0x02000556 RID: 1366
public class MonumentInfo : LandmarkInfo, IPrefabPreProcess
{
	// Token: 0x06002A41 RID: 10817 RVA: 0x00101F6C File Offset: 0x0010016C
	protected override void Awake()
	{
		base.Awake();
		this.obbBounds = new OBB(base.transform.position, base.transform.rotation, this.Bounds);
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x00101FC4 File Offset: 0x001001C4
	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		OBB obb = new OBB(pos, scale, rot, this.Bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = MonumentInfo.TierToMask(this.Tier);
		int num2 = 0;
		if ((num & topology) != 0)
		{
			num2++;
		}
		if ((num & topology2) != 0)
		{
			num2++;
		}
		if ((num & topology3) != 0)
		{
			num2++;
		}
		if ((num & topology4) != 0)
		{
			num2++;
		}
		return num2 >= 3;
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x001020B8 File Offset: 0x001002B8
	public float Distance(Vector3 position)
	{
		return this.obbBounds.Distance(position);
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x001020C6 File Offset: 0x001002C6
	public float SqrDistance(Vector3 position)
	{
		return this.obbBounds.SqrDistance(position);
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x001020D4 File Offset: 0x001002D4
	public float Distance(OBB obb)
	{
		return this.obbBounds.Distance(obb);
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x001020E2 File Offset: 0x001002E2
	public float SqrDistance(OBB obb)
	{
		return this.obbBounds.SqrDistance(obb);
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x001020F0 File Offset: 0x001002F0
	public bool IsInBounds(Vector3 position)
	{
		return this.obbBounds.Contains(position);
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x001020FE File Offset: 0x001002FE
	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		return this.obbBounds.ClosestPoint(position);
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x0010210C File Offset: 0x0010030C
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		Vector3 position = base.transform.position;
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x00102170 File Offset: 0x00100370
	public int GetPathFinderRadius(int res)
	{
		float num = this.Bounds.extents.x * TerrainMeta.OneOverSize.x;
		float num2 = this.Bounds.extents.z * TerrainMeta.OneOverSize.z;
		return Mathf.CeilToInt(Mathf.Max(num, num2) * (float)res);
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x001021C4 File Offset: 0x001003C4
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(this.Bounds.center, this.Bounds.size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x00102253 File Offset: 0x00100453
	public MonumentNavMesh GetMonumentNavMesh()
	{
		return base.GetComponent<MonumentNavMesh>();
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x0010225C File Offset: 0x0010045C
	public static int TierToMask(MonumentTier tier)
	{
		int num = 0;
		if ((tier & MonumentTier.Tier0) != (MonumentTier)0)
		{
			num |= 67108864;
		}
		if ((tier & MonumentTier.Tier1) != (MonumentTier)0)
		{
			num |= 134217728;
		}
		if ((tier & MonumentTier.Tier2) != (MonumentTier)0)
		{
			num |= 268435456;
		}
		return num;
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x00102293 File Offset: 0x00100493
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.HasDungeonLink = this.DetermineHasDungeonLink();
		this.WantsDungeonLink = this.DetermineWantsDungeonLink();
		this.DungeonEntrance = this.FindDungeonEntrance();
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x001022B9 File Offset: 0x001004B9
	private DungeonGridInfo FindDungeonEntrance()
	{
		return base.GetComponentInChildren<DungeonGridInfo>();
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x001022C1 File Offset: 0x001004C1
	private bool DetermineHasDungeonLink()
	{
		return base.GetComponentInChildren<DungeonGridLink>() != null;
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x001022D0 File Offset: 0x001004D0
	private bool DetermineWantsDungeonLink()
	{
		return this.Type != MonumentType.WaterWell && (this.Type != MonumentType.Building || !this.displayPhrase.token.StartsWith("mining_quarry")) && (this.Type != MonumentType.Radtown || !this.displayPhrase.token.StartsWith("swamp"));
	}

	// Token: 0x04002293 RID: 8851
	[Header("MonumentInfo")]
	public MonumentType Type = MonumentType.Building;

	// Token: 0x04002294 RID: 8852
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x04002295 RID: 8853
	public int MinWorldSize;

	// Token: 0x04002296 RID: 8854
	public Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04002297 RID: 8855
	public bool HasNavmesh;

	// Token: 0x04002298 RID: 8856
	public bool IsSafeZone;

	// Token: 0x04002299 RID: 8857
	[HideInInspector]
	public bool WantsDungeonLink;

	// Token: 0x0400229A RID: 8858
	[HideInInspector]
	public bool HasDungeonLink;

	// Token: 0x0400229B RID: 8859
	[HideInInspector]
	public DungeonGridInfo DungeonEntrance;

	// Token: 0x0400229C RID: 8860
	private OBB obbBounds;
}
