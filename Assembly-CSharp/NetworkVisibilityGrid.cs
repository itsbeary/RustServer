using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200062B RID: 1579
public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	// Token: 0x06002E8B RID: 11915 RVA: 0x00117535 File Offset: 0x00115735
	private void Awake()
	{
		Debug.Assert(Network.Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Network.Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Network.Net.sv.visibility = new Manager(this);
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x00117570 File Offset: 0x00115770
	private void OnEnable()
	{
		this.halfGridSize = (float)this.gridSize / 2f;
		this.cellSize = (float)this.gridSize / (float)this.cellCount;
		this.halfCellSize = this.cellSize / 2f;
		this.numIDsPerLayer = this.cellCount * this.cellCount;
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x001175CA File Offset: 0x001157CA
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Network.Net.sv != null && Network.Net.sv.visibility != null)
		{
			Network.Net.sv.visibility.Dispose();
			Network.Net.sv.visibility = null;
		}
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x00117604 File Offset: 0x00115804
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		for (int i = 0; i <= this.cellCount; i++)
		{
			float num = -this.halfGridSize + (float)i * this.cellSize - this.halfCellSize;
			Gizmos.DrawLine(new Vector3(this.halfGridSize, position.y, num), new Vector3(-this.halfGridSize, position.y, num));
			Gizmos.DrawLine(new Vector3(num, position.y, this.halfGridSize), new Vector3(num, position.y, -this.halfGridSize));
		}
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x001176A5 File Offset: 0x001158A5
	private int PositionToGrid(float value)
	{
		return Mathf.RoundToInt((value + this.halfGridSize) / this.cellSize);
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x001176BB File Offset: 0x001158BB
	private float GridToPosition(int value)
	{
		return (float)value * this.cellSize - this.halfGridSize;
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x001176CD File Offset: 0x001158CD
	private int PositionToLayer(float y)
	{
		if (y < this.tunnelsThreshold)
		{
			return 2;
		}
		if (y < this.cavesThreshold)
		{
			return 1;
		}
		if (y >= this.dynamicDungeonsThreshold)
		{
			return 10 + Mathf.FloorToInt((y - this.dynamicDungeonsThreshold) / this.dynamicDungeonsInterval);
		}
		return 0;
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x00117707 File Offset: 0x00115907
	private uint CoordToID(int x, int y, int layer)
	{
		return (uint)(layer * this.numIDsPerLayer + (x * this.cellCount + y) + this.startID);
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x00117724 File Offset: 0x00115924
	private uint GetID(Vector3 vPos)
	{
		int num = this.PositionToGrid(vPos.x);
		int num2 = this.PositionToGrid(vPos.z);
		int num3 = this.PositionToLayer(vPos.y);
		if (num < 0)
		{
			return 0U;
		}
		if (num >= this.cellCount)
		{
			return 0U;
		}
		if (num2 < 0)
		{
			return 0U;
		}
		if (num2 >= this.cellCount)
		{
			return 0U;
		}
		uint num4 = this.CoordToID(num, num2, num3);
		if ((ulong)num4 < (ulong)((long)this.startID))
		{
			Debug.LogError(string.Format("NetworkVisibilityGrid.GetID - group is below range {0} {1} {2} {3} {4}", new object[] { num, num2, num3, num4, this.cellCount }));
		}
		return num4;
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x001177D8 File Offset: 0x001159D8
	[return: TupleElementNames(new string[] { "x", "y", "layer" })]
	private ValueTuple<int, int, int> DeconstructGroupId(int groupId)
	{
		groupId -= this.startID;
		int num2;
		int num = Math.DivRem(groupId, this.numIDsPerLayer, out num2);
		int num3;
		return new ValueTuple<int, int, int>(Math.DivRem(num2, this.cellCount, out num3), num3, num);
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x00117814 File Offset: 0x00115A14
	private Bounds GetBounds(uint uid)
	{
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId((int)uid);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		Vector3 vector = new Vector3(this.GridToPosition(item) - this.halfCellSize, 0f, this.GridToPosition(item2) - this.halfCellSize);
		Vector3 vector2 = new Vector3(vector.x + this.cellSize, 0f, vector.z + this.cellSize);
		if (item3 == 0)
		{
			vector.y = this.cavesThreshold;
			vector2.y = this.dynamicDungeonsThreshold;
		}
		else if (item3 == 1)
		{
			vector.y = this.tunnelsThreshold;
			vector2.y = this.cavesThreshold - float.Epsilon;
		}
		else if (item3 == 2)
		{
			vector.y = -10000f;
			vector2.y = this.tunnelsThreshold - float.Epsilon;
		}
		else if (item3 >= 10)
		{
			int num = item3 - 10;
			vector.y = this.dynamicDungeonsThreshold + (float)num * this.dynamicDungeonsInterval + float.Epsilon;
			vector2.y = vector.y + this.dynamicDungeonsInterval;
		}
		else
		{
			Debug.LogError(string.Format("Cannot get bounds for unknown layer {0}!", item3), this);
		}
		return new Bounds
		{
			min = vector,
			max = vector2
		};
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x00117967 File Offset: 0x00115B67
	public void OnGroupAdded(Group group)
	{
		group.bounds = this.GetBounds(group.ID);
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x0011797B File Offset: 0x00115B7B
	public bool IsInside(Group group, Vector3 vPos)
	{
		return false || group.ID == 0U || group.bounds.Contains(vPos) || group.bounds.SqrDistance(vPos) < this.switchTolerance;
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x001179B8 File Offset: 0x00115BB8
	public Group GetGroup(Vector3 vPos)
	{
		uint id = this.GetID(vPos);
		if (id == 0U)
		{
			return null;
		}
		Group group = Network.Net.sv.visibility.Get(id);
		if (!this.IsInside(group, vPos))
		{
			float num = group.bounds.SqrDistance(vPos);
			Debug.Log(string.Concat(new object[] { "Group is inside is all fucked ", id, "/", num, "/", vPos }));
		}
		return group;
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x00117A40 File Offset: 0x00115C40
	public void GetVisibleFromFar(Group group, List<Group> groups)
	{
		int visibilityRadiusFarOverride = ConVar.Net.visibilityRadiusFarOverride;
		int num = ((visibilityRadiusFarOverride > 0) ? visibilityRadiusFarOverride : this.visibilityRadiusFar);
		this.GetVisibleFrom(group, groups, num);
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x00117A6C File Offset: 0x00115C6C
	public void GetVisibleFromNear(Group group, List<Group> groups)
	{
		int visibilityRadiusNearOverride = ConVar.Net.visibilityRadiusNearOverride;
		int num = ((visibilityRadiusNearOverride > 0) ? visibilityRadiusNearOverride : this.visibilityRadiusNear);
		this.GetVisibleFrom(group, groups, num);
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x00117A98 File Offset: 0x00115C98
	private void GetVisibleFrom(Group group, List<Group> groups, int radius)
	{
		NetworkVisibilityGrid.<>c__DisplayClass34_0 CS$<>8__locals1;
		CS$<>8__locals1.groups = groups;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.groups.Add(Network.Net.sv.visibility.Get(0U));
		int id = (int)group.ID;
		if (id < this.startID)
		{
			return;
		}
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId(id);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		this.<GetVisibleFrom>g__AddLayers|34_0(item, item2, item3, ref CS$<>8__locals1);
		for (int i = 1; i <= radius; i++)
		{
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 + i, item3, ref CS$<>8__locals1);
			for (int j = 1; j < i; j++)
			{
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 + i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 + i, item3, ref CS$<>8__locals1);
			}
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + i, item3, ref CS$<>8__locals1);
		}
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x00117CBF File Offset: 0x00115EBF
	[CompilerGenerated]
	private void <GetVisibleFrom>g__AddLayers|34_0(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, groupLayer, ref A_4);
		if (groupLayer == 0)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 1, ref A_4);
		}
		if (groupLayer == 1)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 2, ref A_4);
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 0, ref A_4);
		}
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x00117CF4 File Offset: 0x00115EF4
	[CompilerGenerated]
	private void <GetVisibleFrom>g__Add|34_1(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		A_4.groups.Add(Network.Net.sv.visibility.Get(this.CoordToID(groupX, groupY, groupLayer)));
	}

	// Token: 0x04002639 RID: 9785
	public const int overworldLayer = 0;

	// Token: 0x0400263A RID: 9786
	public const int cavesLayer = 1;

	// Token: 0x0400263B RID: 9787
	public const int tunnelsLayer = 2;

	// Token: 0x0400263C RID: 9788
	public const int dynamicDungeonsFirstLayer = 10;

	// Token: 0x0400263D RID: 9789
	public int startID = 1024;

	// Token: 0x0400263E RID: 9790
	public int gridSize = 100;

	// Token: 0x0400263F RID: 9791
	public int cellCount = 32;

	// Token: 0x04002640 RID: 9792
	[FormerlySerializedAs("visibilityRadius")]
	public int visibilityRadiusFar = 2;

	// Token: 0x04002641 RID: 9793
	public int visibilityRadiusNear = 1;

	// Token: 0x04002642 RID: 9794
	public float switchTolerance = 20f;

	// Token: 0x04002643 RID: 9795
	public float cavesThreshold = -5f;

	// Token: 0x04002644 RID: 9796
	public float tunnelsThreshold = -50f;

	// Token: 0x04002645 RID: 9797
	public float dynamicDungeonsThreshold = 1000f;

	// Token: 0x04002646 RID: 9798
	public float dynamicDungeonsInterval = 100f;

	// Token: 0x04002647 RID: 9799
	private float halfGridSize;

	// Token: 0x04002648 RID: 9800
	private float cellSize;

	// Token: 0x04002649 RID: 9801
	private float halfCellSize;

	// Token: 0x0400264A RID: 9802
	private int numIDsPerLayer;
}
