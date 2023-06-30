using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006B0 RID: 1712
public class TerrainPath : TerrainExtension
{
	// Token: 0x0600317F RID: 12671 RVA: 0x0012816C File Offset: 0x0012636C
	public override void PostSetup()
	{
		foreach (PathList pathList in this.Roads)
		{
			pathList.ProcgenStartNode = null;
			pathList.ProcgenEndNode = null;
		}
		foreach (PathList pathList2 in this.Rails)
		{
			pathList2.ProcgenStartNode = null;
			pathList2.ProcgenEndNode = null;
		}
		foreach (PathList pathList3 in this.Rivers)
		{
			pathList3.ProcgenStartNode = null;
			pathList3.ProcgenEndNode = null;
		}
		foreach (PathList pathList4 in this.Powerlines)
		{
			pathList4.ProcgenStartNode = null;
			pathList4.ProcgenEndNode = null;
		}
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0012829C File Offset: 0x0012649C
	public void Clear()
	{
		this.Roads.Clear();
		this.Rails.Clear();
		this.Rivers.Clear();
		this.Powerlines.Clear();
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x001282CC File Offset: 0x001264CC
	public T FindClosest<T>(List<T> list, Vector3 pos) where T : MonoBehaviour
	{
		T t = default(T);
		float num = float.MaxValue;
		foreach (T t2 in list)
		{
			float num2 = Vector3Ex.Distance2D(t2.transform.position, pos);
			if (num2 < num)
			{
				t = t2;
				num = num2;
			}
		}
		return t;
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x00128344 File Offset: 0x00126544
	public static int[,] CreatePowerlineCostmap(ref uint seed)
	{
		float num = 5f;
		int num2 = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num2, num2];
		for (int i = 0; i < num2; i++)
		{
			float num3 = ((float)i + 0.5f) / (float)num2;
			for (int j = 0; j < num2; j++)
			{
				float num4 = ((float)j + 0.5f) / (float)num2;
				float slope = heightMap.GetSlope(num4, num3);
				int topology = topologyMap.GetTopology(num4, num3, num);
				int num5 = 2295174;
				int num6 = 1628160;
				int num7 = 512;
				if ((topology & num5) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num6) != 0 || placementMap.GetBlocked(num4, num3, num))
				{
					array[j, i] = 2500;
				}
				else if ((topology & num7) != 0)
				{
					array[j, i] = 1000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f);
				}
			}
		}
		return array;
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x0012846C File Offset: 0x0012666C
	public static int[,] CreateRoadCostmap(ref uint seed)
	{
		float num = 5f;
		int num2 = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num2, num2];
		for (int i = 0; i < num2; i++)
		{
			float num3 = ((float)i + 0.5f) / (float)num2;
			for (int j = 0; j < num2; j++)
			{
				float num4 = ((float)j + 0.5f) / (float)num2;
				int num5 = SeedRandom.Range(ref seed, 100, 200);
				float slope = heightMap.GetSlope(num4, num3);
				int topology = topologyMap.GetTopology(num4, num3, num);
				int num6 = 2295686;
				int num7 = 49152;
				if (slope > 20f || (topology & num6) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num7) != 0 || placementMap.GetBlocked(num4, num3, num))
				{
					array[j, i] = 5000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f) + num5;
				}
			}
		}
		return array;
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x00128590 File Offset: 0x00126790
	public static int[,] CreateRailCostmap(ref uint seed)
	{
		float num = 5f;
		int num2 = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num2, num2];
		for (int i = 0; i < num2; i++)
		{
			float num3 = ((float)i + 0.5f) / (float)num2;
			for (int j = 0; j < num2; j++)
			{
				float num4 = ((float)j + 0.5f) / (float)num2;
				float slope = heightMap.GetSlope(num4, num3);
				int topology = topologyMap.GetTopology(num4, num3, num);
				int num5 = 2295686;
				int num6 = 49152;
				if (slope > 20f || (topology & num5) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num6) != 0 || placementMap.GetBlocked(num4, num3, num))
				{
					array[j, i] = 5000;
				}
				else if (slope > 10f)
				{
					array[j, i] = 1500;
				}
				else
				{
					array[j, i] = 1000;
				}
			}
		}
		return array;
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x001286B4 File Offset: 0x001268B4
	public static int[,] CreateBoatCostmap(float depth)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		int res = heightMap.res;
		int[,] array = new int[res, res];
		for (int i = 0; i < res; i++)
		{
			float num = ((float)i + 0.5f) / (float)res;
			for (int j = 0; j < res; j++)
			{
				float num2 = ((float)j + 0.5f) / (float)res;
				float height = heightMap.GetHeight(num2, num);
				if (waterMap.GetHeight(num2, num) - height < depth)
				{
					array[j, i] = int.MaxValue;
				}
				else
				{
					array[j, i] = 1;
				}
			}
		}
		return array;
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x00128754 File Offset: 0x00126954
	public void AddWire(PowerlineNode node)
	{
		string name = node.transform.root.name;
		if (!this.wires.ContainsKey(name))
		{
			this.wires.Add(name, new List<PowerlineNode>());
		}
		this.wires[name].Add(node);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x001287A4 File Offset: 0x001269A4
	public void CreateWires()
	{
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		GameObjectRef gameObjectRef = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> keyValuePair in this.wires)
		{
			foreach (PowerlineNode powerlineNode in keyValuePair.Value)
			{
				PowerLineWireConnectionHelper component = powerlineNode.GetComponent<PowerLineWireConnectionHelper>();
				if (component)
				{
					if (list.Count == 0)
					{
						gameObjectRef = powerlineNode.WirePrefab;
						num = component.connections.Count;
					}
					else
					{
						GameObject gameObject = list[list.Count - 1];
						if (powerlineNode.WirePrefab.guid != ((gameObjectRef != null) ? gameObjectRef.guid : null) || component.connections.Count != num || (gameObject.transform.position - powerlineNode.transform.position).sqrMagnitude > powerlineNode.MaxDistance * powerlineNode.MaxDistance)
						{
							this.CreateWire(keyValuePair.Key, list, gameObjectRef);
							list.Clear();
						}
					}
					list.Add(powerlineNode.gameObject);
				}
			}
			this.CreateWire(keyValuePair.Key, list, gameObjectRef);
			list.Clear();
		}
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x00128940 File Offset: 0x00126B40
	private void CreateWire(string name, List<GameObject> objects, GameObjectRef wirePrefab)
	{
		if (objects.Count < 3 || wirePrefab == null || !wirePrefab.isValid)
		{
			return;
		}
		PowerLineWire powerLineWire = PowerLineWire.Create(null, objects, wirePrefab, "Powerline Wires", null, 1f, 0.1f);
		if (powerLineWire)
		{
			powerLineWire.enabled = false;
			powerLineWire.gameObject.SetHierarchyGroup(name, true, false);
		}
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x00128998 File Offset: 0x00126B98
	public MonumentInfo FindMonumentWithBoundsOverlap(Vector3 position)
	{
		if (TerrainMeta.Path.Monuments == null)
		{
			return null;
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsInBounds(position))
			{
				return monumentInfo;
			}
		}
		return null;
	}

	// Token: 0x0400282F RID: 10287
	internal List<PathList> Roads = new List<PathList>();

	// Token: 0x04002830 RID: 10288
	internal List<PathList> Rails = new List<PathList>();

	// Token: 0x04002831 RID: 10289
	internal List<PathList> Rivers = new List<PathList>();

	// Token: 0x04002832 RID: 10290
	internal List<PathList> Powerlines = new List<PathList>();

	// Token: 0x04002833 RID: 10291
	internal List<LandmarkInfo> Landmarks = new List<LandmarkInfo>();

	// Token: 0x04002834 RID: 10292
	internal List<MonumentInfo> Monuments = new List<MonumentInfo>();

	// Token: 0x04002835 RID: 10293
	internal List<RiverInfo> RiverObjs = new List<RiverInfo>();

	// Token: 0x04002836 RID: 10294
	internal List<LakeInfo> LakeObjs = new List<LakeInfo>();

	// Token: 0x04002837 RID: 10295
	internal GameObject DungeonGridRoot;

	// Token: 0x04002838 RID: 10296
	internal List<DungeonGridInfo> DungeonGridEntrances = new List<DungeonGridInfo>();

	// Token: 0x04002839 RID: 10297
	internal List<DungeonGridCell> DungeonGridCells = new List<DungeonGridCell>();

	// Token: 0x0400283A RID: 10298
	internal GameObject DungeonBaseRoot;

	// Token: 0x0400283B RID: 10299
	internal List<DungeonBaseInfo> DungeonBaseEntrances = new List<DungeonBaseInfo>();

	// Token: 0x0400283C RID: 10300
	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	// Token: 0x0400283D RID: 10301
	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	// Token: 0x0400283E RID: 10302
	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();
}
