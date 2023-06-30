using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class SpawnDistribution
{
	// Token: 0x06002ABC RID: 10940 RVA: 0x00104C6C File Offset: 0x00102E6C
	public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
	{
		this.Handler = handler;
		this.quadtree.UpdateValues(baseValues);
		this.origin = origin;
		float num = 0f;
		for (int i = 0; i < baseValues.Length; i++)
		{
			num += (float)baseValues[i];
		}
		this.Density = num / (float)(255 * baseValues.Length);
		this.Count = 0;
		this.area = new Vector3(area.x / (float)this.quadtree.Size, area.y, area.z / (float)this.quadtree.Size);
		this.grid = new WorldSpaceGrid<int>(area.x, 20f);
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x00104D34 File Offset: 0x00102F34
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, bool alignToNormal = false, float dithering = 0f)
	{
		return this.Sample(out spawnPos, out spawnRot, this.SampleNode(), alignToNormal, dithering);
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x00104D48 File Offset: 0x00102F48
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, ByteQuadtree.Element node, bool alignToNormal = false, float dithering = 0f)
	{
		if (this.Handler == null || TerrainMeta.HeightMap == null)
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
			return false;
		}
		LayerMask placementMask = this.Handler.PlacementMask;
		LayerMask placementCheckMask = this.Handler.PlacementCheckMask;
		float placementCheckHeight = this.Handler.PlacementCheckHeight;
		LayerMask radiusCheckMask = this.Handler.RadiusCheckMask;
		float radiusCheckDistance = this.Handler.RadiusCheckDistance;
		for (int i = 0; i < 15; i++)
		{
			spawnPos = this.origin;
			spawnPos.x += node.Coords.x * this.area.x;
			spawnPos.z += node.Coords.y * this.area.z;
			spawnPos.x += UnityEngine.Random.value * this.area.x;
			spawnPos.z += UnityEngine.Random.value * this.area.z;
			spawnPos.x += UnityEngine.Random.Range(-dithering, dithering);
			spawnPos.z += UnityEngine.Random.Range(-dithering, dithering);
			Vector3 vector = new Vector3(spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), spawnPos.z);
			if (vector.y > spawnPos.y)
			{
				RaycastHit raycastHit;
				if (placementCheckMask != 0 && Physics.Raycast(vector + Vector3.up * placementCheckHeight, Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
				{
					if (((1 << raycastHit.transform.gameObject.layer) & placementMask) == 0)
					{
						goto IL_243;
					}
					vector.y = raycastHit.point.y;
				}
				if (radiusCheckMask == 0 || !Physics.CheckSphere(vector, radiusCheckDistance, radiusCheckMask))
				{
					spawnPos.y = vector.y;
					spawnRot = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
					if (alignToNormal)
					{
						Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
						spawnRot = QuaternionEx.LookRotationForcedUp(spawnRot * Vector3.forward, normal);
					}
					return true;
				}
			}
			IL_243:;
		}
		spawnPos = Vector3.zero;
		spawnRot = Quaternion.identity;
		return false;
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x00104FC0 File Offset: 0x001031C0
	public ByteQuadtree.Element SampleNode()
	{
		ByteQuadtree.Element element = this.quadtree.Root;
		while (!element.IsLeaf)
		{
			element = element.RandChild;
		}
		return element;
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x00104FED File Offset: 0x001031ED
	public void AddInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, 1);
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x00104FF7 File Offset: 0x001031F7
	public void RemoveInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, -1);
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x00105004 File Offset: 0x00103204
	private void UpdateCount(Spawnable spawnable, int delta)
	{
		this.Count += delta;
		WorldSpaceGrid<int> worldSpaceGrid = this.grid;
		Vector3 spawnPosition = spawnable.SpawnPosition;
		worldSpaceGrid[spawnPosition] += delta;
		BaseEntity component = spawnable.GetComponent<BaseEntity>();
		if (component)
		{
			int num;
			if (this.dict.TryGetValue(component.prefabID, out num))
			{
				this.dict[component.prefabID] = num + delta;
				return;
			}
			num = delta;
			this.dict.Add(component.prefabID, num);
		}
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x0010508C File Offset: 0x0010328C
	public int GetCount(uint prefabID)
	{
		int num;
		this.dict.TryGetValue(prefabID, out num);
		return num;
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x001050A9 File Offset: 0x001032A9
	public int GetCount(Vector3 position)
	{
		return this.grid[position];
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x001050B7 File Offset: 0x001032B7
	public float GetGridCellArea()
	{
		return this.grid.CellArea;
	}

	// Token: 0x040022E9 RID: 8937
	internal SpawnHandler Handler;

	// Token: 0x040022EA RID: 8938
	internal float Density;

	// Token: 0x040022EB RID: 8939
	internal int Count;

	// Token: 0x040022EC RID: 8940
	private WorldSpaceGrid<int> grid;

	// Token: 0x040022ED RID: 8941
	private Dictionary<uint, int> dict = new Dictionary<uint, int>();

	// Token: 0x040022EE RID: 8942
	private ByteQuadtree quadtree = new ByteQuadtree();

	// Token: 0x040022EF RID: 8943
	private Vector3 origin;

	// Token: 0x040022F0 RID: 8944
	private Vector3 area;
}
