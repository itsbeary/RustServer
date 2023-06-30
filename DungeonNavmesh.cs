using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200020B RID: 523
public class DungeonNavmesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x06001B7A RID: 7034 RVA: 0x000C27BC File Offset: 0x000C09BC
	public static bool NavReady()
	{
		if (DungeonNavmesh.Instances == null || DungeonNavmesh.Instances.Count == 0)
		{
			return true;
		}
		using (List<DungeonNavmesh>.Enumerator enumerator = DungeonNavmesh.Instances.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsBuilding)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06001B7B RID: 7035 RVA: 0x000C282C File Offset: 0x000C0A2C
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x000C2844 File Offset: 0x000C0A44
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
		DungeonNavmesh.Instances.Add(this);
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x000C28BE File Offset: 0x000C0ABE
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
		DungeonNavmesh.Instances.Remove(this);
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x000C28F4 File Offset: 0x000C0AF4
	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			return;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Dungeon Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= this.NavmeshResolutionModifier;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
		this.NotifyInformationZonesOfCompletion();
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x000C29D4 File Offset: 0x000C0BD4
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000C2A24 File Offset: 0x000C0C24
	public void SourcesCollected()
	{
		int count = this.sources.Count;
		UnityEngine.Debug.Log("Source count Pre cull : " + this.sources.Count);
		for (int i = this.sources.Count - 1; i >= 0; i--)
		{
			NavMeshBuildSource navMeshBuildSource = this.sources[i];
			Matrix4x4 transform = navMeshBuildSource.transform;
			Vector3 vector = new Vector3(transform[0, 3], transform[1, 3], transform[2, 3]);
			bool flag = false;
			using (List<AIInformationZone>.Enumerator enumerator = AIInformationZone.zones.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector3Ex.Distance2D(enumerator.Current.ClosestPointTo(vector), vector) <= 50f)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				this.sources.Remove(navMeshBuildSource);
			}
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"Source count post cull : ",
			this.sources.Count,
			" total removed : ",
			count - this.sources.Count
		}));
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x000C2B64 File Offset: 0x000C0D64
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.center = base.transform.position;
		this.Bounds.size = new Vector3(1000000f, 100000f, 100000f);
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(base.transform, this.LayerMask.value, this.NavMeshCollectGeometry, this.defaultArea, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync));
		if (AiManager.nav_wait)
		{
			yield return enumerator;
		}
		else
		{
			base.StartCoroutine(enumerator);
		}
		if (!AiManager.nav_wait)
		{
			UnityEngine.Debug.Log("nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int lastPct = 0;
		while (!this.HasBuildOperationStarted)
		{
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
		}
		while (this.BuildingOperation != null)
		{
			int num = (int)(this.BuildingOperation.progress * 100f);
			if (lastPct != num)
			{
				UnityEngine.Debug.LogFormat("{0}%", new object[] { num });
				lastPct = num;
			}
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
			this.FinishBuildingNavmesh();
		}
		yield break;
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x000C2B74 File Offset: 0x000C0D74
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & (1 << navMeshModifierVolume.gameObject.layer)) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 vector = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
				if (this.Bounds.Contains(vector))
				{
					Vector3 lossyScale = navMeshModifierVolume.transform.lossyScale;
					Vector3 vector2 = new Vector3(navMeshModifierVolume.size.x * Mathf.Abs(lossyScale.x), navMeshModifierVolume.size.y * Mathf.Abs(lossyScale.y), navMeshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
					sources.Add(new NavMeshBuildSource
					{
						shape = NavMeshBuildSourceShape.ModifierBox,
						transform = Matrix4x4.TRS(vector, navMeshModifierVolume.transform.rotation, Vector3.one),
						size = vector2,
						area = navMeshModifierVolume.area
					});
				}
			}
		}
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x000C2CCC File Offset: 0x000C0ECC
	public void FinishBuildingNavmesh()
	{
		if (this.BuildingOperation == null)
		{
			return;
		}
		if (!this.BuildingOperation.isDone)
		{
			return;
		}
		if (!this.NavMeshDataInstance.valid)
		{
			this.NavMeshDataInstance = NavMesh.AddNavMeshData(this.NavMeshData);
		}
		UnityEngine.Debug.Log(string.Format("Monument Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000C2D3C File Offset: 0x000C0F3C
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position, this.Bounds.size);
	}

	// Token: 0x04001348 RID: 4936
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001349 RID: 4937
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x0400134A RID: 4938
	public float NavmeshResolutionModifier = 1.25f;

	// Token: 0x0400134B RID: 4939
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x0400134C RID: 4940
	public NavMeshData NavMeshData;

	// Token: 0x0400134D RID: 4941
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400134E RID: 4942
	public LayerMask LayerMask;

	// Token: 0x0400134F RID: 4943
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001350 RID: 4944
	public static List<DungeonNavmesh> Instances = new List<DungeonNavmesh>();

	// Token: 0x04001351 RID: 4945
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001352 RID: 4946
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001353 RID: 4947
	private AsyncOperation BuildingOperation;

	// Token: 0x04001354 RID: 4948
	private bool HasBuildOperationStarted;

	// Token: 0x04001355 RID: 4949
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001356 RID: 4950
	private int defaultArea;

	// Token: 0x04001357 RID: 4951
	private int agentTypeId;
}
