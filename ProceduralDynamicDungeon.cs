using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class ProceduralDynamicDungeon : global::BaseEntity
{
	// Token: 0x06001846 RID: 6214 RVA: 0x000B5E82 File Offset: 0x000B4082
	public override void InitShared()
	{
		base.InitShared();
		ProceduralDynamicDungeon.dungeons.Add(this);
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x000B5E98 File Offset: 0x000B4098
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			EntityFlag_Toggle[] componentsInChildren = proceduralDungeonCell.GetComponentsInChildren<EntityFlag_Toggle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].DoUpdate(this);
			}
		}
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x000B5F08 File Offset: 0x000B4108
	public global::BaseEntity GetExitPortal(bool serverSide)
	{
		return this.exitPortal.Get(serverSide);
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x000B5F16 File Offset: 0x000B4116
	public override void DestroyShared()
	{
		ProceduralDynamicDungeon.dungeons.Remove(this);
		this.RetireAllCells();
		base.DestroyShared();
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x000B5F30 File Offset: 0x000B4130
	public bool ContainsAnyPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				return true;
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x000B5FE8 File Offset: 0x000B41E8
	public void KillPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				basePlayer.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				basePlayer2.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x000B60B6 File Offset: 0x000B42B6
	internal override void DoServerDestroy()
	{
		this.KillPlayers();
		if (this.exitPortal.IsValid(true))
		{
			this.exitPortal.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x000B60E4 File Offset: 0x000B42E4
	public override void ServerInit()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.baseseed = (this.seed = (uint)UnityEngine.Random.Range(0, 12345567));
			Debug.Log("Spawning dungeon with seed :" + (int)this.seed);
		}
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.DoGeneration();
			BasePortal component = GameManager.server.CreateEntity(this.exitPortalPrefab.resourcePath, this.entranceHack.exitPointHack.position, this.entranceHack.exitPointHack.rotation, true).GetComponent<BasePortal>();
			component.Spawn();
			this.exitPortal.Set(component);
		}
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000B6190 File Offset: 0x000B4390
	public void DoGeneration()
	{
		this.GenerateGrid();
		this.CreateAIZ();
		if (base.isServer)
		{
			Debug.Log("Server DoGeneration,calling routine update nav mesh");
			base.StartCoroutine(this.UpdateNavMesh());
		}
		base.Invoke(new Action(this.InitSpawnGroups), 1f);
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000B61E0 File Offset: 0x000B43E0
	private void CreateAIZ()
	{
		AIInformationZone aiinformationZone = base.gameObject.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		aiinformationZone.bounds.extents = new Vector3((float)this.gridResolution * this.gridSpacing * 0.75f, 10f, (float)this.gridResolution * this.gridSpacing * 0.75f);
		aiinformationZone.Init();
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000B6241 File Offset: 0x000B4441
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.DoGeneration();
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x000B624F File Offset: 0x000B444F
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dungeon done!");
		yield break;
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x000B6260 File Offset: 0x000B4460
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.proceduralDungeon == null)
		{
			info.msg.proceduralDungeon = Pool.Get<ProceduralDungeon>();
		}
		info.msg.proceduralDungeon.seed = this.baseseed;
		info.msg.proceduralDungeon.exitPortalID = this.exitPortal.uid;
		info.msg.proceduralDungeon.mapOffset = this.mapOffset;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x000B62D8 File Offset: 0x000B44D8
	public BasePortal GetExitPortal()
	{
		return this.exitPortal.Get(true);
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x000B62E8 File Offset: 0x000B44E8
	public void InitSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			if (!(this.entranceHack != null) || Vector3.Distance(this.entranceHack.transform.position, proceduralDungeonCell.transform.position) >= 20f)
			{
				SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
				for (int i = 0; i < spawnGroups.Length; i++)
				{
					spawnGroups[i].Spawn();
				}
			}
		}
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x000B6388 File Offset: 0x000B4588
	public void CleanupSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
			for (int i = 0; i < spawnGroups.Length; i++)
			{
				spawnGroups[i].Clear();
			}
		}
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000B63F0 File Offset: 0x000B45F0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.proceduralDungeon != null)
		{
			this.baseseed = (this.seed = info.msg.proceduralDungeon.seed);
			this.exitPortal.uid = info.msg.proceduralDungeon.exitPortalID;
			this.mapOffset = info.msg.proceduralDungeon.mapOffset;
		}
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x000B6464 File Offset: 0x000B4664
	[ContextMenu("Test Grid")]
	[ExecuteInEditMode]
	public void GenerateGrid()
	{
		Vector3 vector = base.transform.position - new Vector3((float)this.gridResolution * this.gridSpacing * 0.5f, 0f, (float)this.gridResolution * this.gridSpacing * 0.5f);
		this.RetireAllCells();
		this.grid = new bool[this.gridResolution * this.gridResolution];
		for (int i = 0; i < this.grid.Length; i++)
		{
			this.grid[i] = ((SeedRandom.Range(ref this.seed, 0, 2) == 0) ? true : false);
		}
		this.SetEntrance(3, 0);
		for (int j = 0; j < this.gridResolution; j++)
		{
			for (int k = 0; k < this.gridResolution; k++)
			{
				if (this.GetGridState(j, k) && !this.HasPathToEntrance(j, k))
				{
					this.SetGridState(j, k, false);
				}
			}
		}
		for (int l = 0; l < this.gridResolution; l++)
		{
			for (int m = 0; m < this.gridResolution; m++)
			{
				if (this.GetGridState(l, m))
				{
					bool gridState = this.GetGridState(l, m + 1);
					bool gridState2 = this.GetGridState(l, m - 1);
					bool gridState3 = this.GetGridState(l - 1, m);
					bool gridState4 = this.GetGridState(l + 1, m);
					bool flag = this.IsEntrance(l, m);
					GameObjectRef gameObjectRef = null;
					ProceduralDungeonCell proceduralDungeonCell = null;
					if (proceduralDungeonCell == null)
					{
						foreach (GameObjectRef gameObjectRef2 in this.cellPrefabReferences)
						{
							ProceduralDungeonCell component = gameObjectRef2.Get().GetComponent<ProceduralDungeonCell>();
							if (component.north == gridState && component.south == gridState2 && component.west == gridState3 && component.east == gridState4 && component.entrance == flag)
							{
								proceduralDungeonCell = component;
								gameObjectRef = gameObjectRef2;
								break;
							}
						}
					}
					if (proceduralDungeonCell != null)
					{
						ProceduralDungeonCell proceduralDungeonCell2 = this.CellInstantiate(gameObjectRef.resourcePath);
						proceduralDungeonCell2.transform.position = vector + new Vector3((float)l * this.gridSpacing, 0f, (float)m * this.gridSpacing);
						this.spawnedCells.Add(proceduralDungeonCell2);
						proceduralDungeonCell2.transform.SetParent(base.transform);
						if (proceduralDungeonCell2.entrance && this.entranceHack == null)
						{
							this.entranceHack = proceduralDungeonCell2;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x000B66FC File Offset: 0x000B48FC
	public ProceduralDungeonCell CellInstantiate(string path)
	{
		if (base.isServer)
		{
			return GameManager.server.CreatePrefab(path, true).GetComponent<ProceduralDungeonCell>();
		}
		return null;
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x000B6719 File Offset: 0x000B4919
	public void RetireCell(GameObject cell)
	{
		if (cell == null)
		{
			return;
		}
		if (base.isServer)
		{
			GameManager.server.Retire(cell);
		}
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000B6738 File Offset: 0x000B4938
	public void RetireAllCells()
	{
		if (base.isServer)
		{
			this.CleanupSpawnGroups();
		}
		for (int i = this.spawnedCells.Count - 1; i >= 0; i--)
		{
			ProceduralDungeonCell proceduralDungeonCell = this.spawnedCells[i];
			if (proceduralDungeonCell)
			{
				this.RetireCell(proceduralDungeonCell.gameObject);
			}
		}
		this.spawnedCells.Clear();
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x000B6798 File Offset: 0x000B4998
	public bool CanSeeEntrance(int x, int y, ref List<int> checkedCells)
	{
		int gridIndex = this.GetGridIndex(x, y);
		if (checkedCells.Contains(gridIndex))
		{
			return false;
		}
		checkedCells.Add(gridIndex);
		if (!this.GetGridState(x, y))
		{
			return false;
		}
		if (this.IsEntrance(x, y))
		{
			return true;
		}
		bool flag = this.CanSeeEntrance(x, y + 1, ref checkedCells);
		bool flag2 = this.CanSeeEntrance(x, y - 1, ref checkedCells);
		bool flag3 = this.CanSeeEntrance(x - 1, y, ref checkedCells);
		bool flag4 = this.CanSeeEntrance(x + 1, y, ref checkedCells);
		return flag || flag4 || flag3 || flag2;
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x000B6810 File Offset: 0x000B4A10
	public bool HasPathToEntrance(int x, int y)
	{
		List<int> list = new List<int>();
		bool flag = this.CanSeeEntrance(x, y, ref list);
		list.Clear();
		return flag;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x000B6833 File Offset: 0x000B4A33
	public bool CanFindEntrance(int x, int y)
	{
		new List<int>();
		this.GetGridState(x, y + 1);
		this.GetGridState(x, y - 1);
		this.GetGridState(x - 1, y);
		this.GetGridState(x + 1, y);
		return true;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x000B6868 File Offset: 0x000B4A68
	public bool IsEntrance(int x, int y)
	{
		return this.GetGridIndex(x, y) == this.GetEntranceIndex();
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x000B687A File Offset: 0x000B4A7A
	public int GetEntranceIndex()
	{
		return this.GetGridIndex(3, 0);
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x000B6884 File Offset: 0x000B4A84
	public void SetEntrance(int x, int y)
	{
		this.grid[this.GetGridIndex(x, y)] = true;
		this.grid[this.GetGridIndex(x, y + 1)] = true;
		this.grid[this.GetGridIndex(x - 1, y)] = false;
		this.grid[this.GetGridIndex(x + 1, y)] = false;
		this.grid[this.GetGridIndex(x, y + 2)] = true;
		this.grid[this.GetGridIndex(x + 1, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x + 2, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x, y + 3)] = true;
		this.grid[this.GetGridIndex(x, y + 4)] = true;
		this.grid[this.GetGridIndex(x - 1, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x - 2, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x000B69AC File Offset: 0x000B4BAC
	public void SetGridState(int x, int y, bool state)
	{
		int gridIndex = this.GetGridIndex(x, y);
		this.grid[gridIndex] = state;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000B69CC File Offset: 0x000B4BCC
	public bool GetGridState(int x, int y)
	{
		return this.GetGridIndex(x, y) < this.grid.Length && x >= 0 && x < this.gridResolution && y >= 0 && y < this.gridResolution && this.grid[this.GetGridIndex(x, y)];
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x000B6A1A File Offset: 0x000B4C1A
	public int GetGridX(int index)
	{
		return index % this.gridResolution;
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x000B6A24 File Offset: 0x000B4C24
	public int GetGridY(int index)
	{
		return Mathf.FloorToInt((float)index / (float)this.gridResolution);
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000B6A35 File Offset: 0x000B4C35
	public int GetGridIndex(int x, int y)
	{
		return y * this.gridResolution + x;
	}

	// Token: 0x04001119 RID: 4377
	public int gridResolution = 6;

	// Token: 0x0400111A RID: 4378
	public float gridSpacing = 12f;

	// Token: 0x0400111B RID: 4379
	public bool[] grid;

	// Token: 0x0400111C RID: 4380
	public List<GameObjectRef> cellPrefabReferences = new List<GameObjectRef>();

	// Token: 0x0400111D RID: 4381
	public List<ProceduralDungeonCell> spawnedCells = new List<ProceduralDungeonCell>();

	// Token: 0x0400111E RID: 4382
	public EnvironmentVolume envVolume;

	// Token: 0x0400111F RID: 4383
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x04001120 RID: 4384
	public GameObjectRef exitPortalPrefab;

	// Token: 0x04001121 RID: 4385
	private EntityRef<BasePortal> exitPortal;

	// Token: 0x04001122 RID: 4386
	public TriggerRadiation exitRadiation;

	// Token: 0x04001123 RID: 4387
	public uint seed;

	// Token: 0x04001124 RID: 4388
	public uint baseseed;

	// Token: 0x04001125 RID: 4389
	public Vector3 mapOffset = Vector3.zero;

	// Token: 0x04001126 RID: 4390
	public static readonly List<ProceduralDynamicDungeon> dungeons = new List<ProceduralDynamicDungeon>();

	// Token: 0x04001127 RID: 4391
	public ProceduralDungeonCell entranceHack;
}
