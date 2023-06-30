using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class DynamicDungeon : BaseEntity, IMissionEntityListener
{
	// Token: 0x06001821 RID: 6177 RVA: 0x000B52E8 File Offset: 0x000B34E8
	public static void AddDungeon(DynamicDungeon newDungeon)
	{
		DynamicDungeon._dungeons.Add(newDungeon);
		Vector3 position = newDungeon.transform.position;
		if (position.y >= DynamicDungeon.nextDungeonPos.y)
		{
			DynamicDungeon.nextDungeonPos = position + Vector3.up * DynamicDungeon.dungeonSpacing;
		}
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x000B5338 File Offset: 0x000B3538
	public static void RemoveDungeon(DynamicDungeon dungeon)
	{
		Vector3 position = dungeon.transform.position;
		if (DynamicDungeon._dungeons.Contains(dungeon))
		{
			DynamicDungeon._dungeons.Remove(dungeon);
		}
		DynamicDungeon.nextDungeonPos = position;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x000B5363 File Offset: 0x000B3563
	public static Vector3 GetNextDungeonPoint()
	{
		if (DynamicDungeon.nextDungeonPos == Vector3.zero)
		{
			DynamicDungeon.nextDungeonPos = Vector3.one * 700f;
		}
		return DynamicDungeon.nextDungeonPos;
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x000B538F File Offset: 0x000B358F
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dunngeon done!");
		yield break;
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x000B53A0 File Offset: 0x000B35A0
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
			if (this.exitPortal != null)
			{
				this.exitPortal.Kill(BaseNetworkable.DestroyMode.None);
			}
			DynamicDungeon.RemoveDungeon(this);
		}
		base.DestroyShared();
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x000B53F8 File Offset: 0x000B35F8
	public override void ServerInit()
	{
		base.ServerInit();
		DynamicDungeon.AddDungeon(this);
		if (this.portalPrefab.isValid)
		{
			this.exitPortal = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.exitPortal.SetParent(this, true, false);
			this.exitPortal.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
		this.MergeAIZones();
		base.StartCoroutine(this.UpdateNavMesh());
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x000B54E0 File Offset: 0x000B36E0
	private void MergeAIZones()
	{
		if (!this.AutoMergeAIZones)
		{
			return;
		}
		List<AIInformationZone> list = base.GetComponentsInChildren<AIInformationZone>().ToList<AIInformationZone>();
		foreach (AIInformationZone aiinformationZone in list)
		{
			aiinformationZone.AddInitialPoints();
		}
		GameObject gameObject = new GameObject("AIZ");
		gameObject.transform.position = base.transform.position;
		AIInformationZone.Merge(list, gameObject).ShouldSleepAI = false;
		gameObject.transform.SetParent(base.transform);
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x000B5580 File Offset: 0x000B3780
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		foreach (MissionEntity missionEntity in instance.createdEntities)
		{
			BunkerEntrance component = missionEntity.GetComponent<BunkerEntrance>();
			if (component != null)
			{
				BasePortal portalInstance = component.portalInstance;
				if (portalInstance)
				{
					portalInstance.targetPortal = this.exitPortal;
					this.exitPortal.targetPortal = portalInstance;
					Debug.Log("Dungeon portal linked...");
				}
			}
		}
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x040010F0 RID: 4336
	public Transform exitEntitySpawn;

	// Token: 0x040010F1 RID: 4337
	public GameObjectRef exitEntity;

	// Token: 0x040010F2 RID: 4338
	public string exitString;

	// Token: 0x040010F3 RID: 4339
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x040010F4 RID: 4340
	private static List<DynamicDungeon> _dungeons = new List<DynamicDungeon>();

	// Token: 0x040010F5 RID: 4341
	public GameObjectRef portalPrefab;

	// Token: 0x040010F6 RID: 4342
	public Transform portalSpawnPoint;

	// Token: 0x040010F7 RID: 4343
	public BasePortal exitPortal;

	// Token: 0x040010F8 RID: 4344
	public GameObjectRef doorPrefab;

	// Token: 0x040010F9 RID: 4345
	public Transform doorSpawnPoint;

	// Token: 0x040010FA RID: 4346
	public Door doorInstance;

	// Token: 0x040010FB RID: 4347
	public static Vector3 nextDungeonPos = Vector3.zero;

	// Token: 0x040010FC RID: 4348
	public static Vector3 dungeonStartPoint = Vector3.zero;

	// Token: 0x040010FD RID: 4349
	public static float dungeonSpacing = 50f;

	// Token: 0x040010FE RID: 4350
	public SpawnGroup[] spawnGroups;

	// Token: 0x040010FF RID: 4351
	public bool AutoMergeAIZones = true;
}
