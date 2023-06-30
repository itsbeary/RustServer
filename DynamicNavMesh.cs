using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200020C RID: 524
public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06001B87 RID: 7047 RVA: 0x000C2DC7 File Offset: 0x000C0FC7
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x000C2DDC File Offset: 0x000C0FDC
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x000C2E4B File Offset: 0x000C104B
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x000C2E74 File Offset: 0x000C1074
	[ContextMenu("Update Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= 2f;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x000C2F44 File Offset: 0x000C1144
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.size = TerrainMeta.Size;
		NavMesh.pathfindingIterationsPerFrame = AiManager.pathfindingIterationsPerFrame;
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, DynamicNavMesh.use_baked_terrain_mesh, this.AsyncTerrainNavMeshBakeCellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), null);
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

	// Token: 0x06001B8C RID: 7052 RVA: 0x000C2F54 File Offset: 0x000C1154
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & (1 << navMeshModifierVolume.gameObject.layer)) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 vector = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
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

	// Token: 0x06001B8D RID: 7053 RVA: 0x000C3098 File Offset: 0x000C1298
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
		UnityEngine.Debug.Log(string.Format("Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	// Token: 0x04001358 RID: 4952
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001359 RID: 4953
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	// Token: 0x0400135A RID: 4954
	public int AsyncTerrainNavMeshBakeCellSize = 80;

	// Token: 0x0400135B RID: 4955
	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	// Token: 0x0400135C RID: 4956
	public Bounds Bounds;

	// Token: 0x0400135D RID: 4957
	public NavMeshData NavMeshData;

	// Token: 0x0400135E RID: 4958
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400135F RID: 4959
	public LayerMask LayerMask;

	// Token: 0x04001360 RID: 4960
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001361 RID: 4961
	[ServerVar]
	public static bool use_baked_terrain_mesh;

	// Token: 0x04001362 RID: 4962
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001363 RID: 4963
	private AsyncOperation BuildingOperation;

	// Token: 0x04001364 RID: 4964
	private bool HasBuildOperationStarted;

	// Token: 0x04001365 RID: 4965
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001366 RID: 4966
	private int defaultArea;

	// Token: 0x04001367 RID: 4967
	private int agentTypeId;
}
