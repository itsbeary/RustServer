using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200020D RID: 525
public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06001B90 RID: 7056 RVA: 0x000C3136 File Offset: 0x000C1336
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x000C314C File Offset: 0x000C134C
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x000C31BB File Offset: 0x000C13BB
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x000C31E4 File Offset: 0x000C13E4
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
		UnityEngine.Debug.Log("Starting Monument Navmesh Build with " + this.sources.Count + " sources");
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
		if (this.shouldNotifyAIZones)
		{
			this.NotifyInformationZonesOfCompletion();
		}
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x000C32CA File Offset: 0x000C14CA
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
		if (!this.overrideAutoBounds)
		{
			this.Bounds.size = new Vector3((float)(this.CellSize * this.CellCount), (float)this.Height, (float)(this.CellSize * this.CellCount));
		}
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, MonumentNavMesh.use_baked_terrain_mesh && !this.forceCollectTerrain, this.CellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), this.CustomNavMeshRoot);
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

	// Token: 0x06001B95 RID: 7061 RVA: 0x000C32DC File Offset: 0x000C14DC
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x000C332C File Offset: 0x000C152C
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

	// Token: 0x06001B97 RID: 7063 RVA: 0x000C3484 File Offset: 0x000C1684
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

	// Token: 0x06001B98 RID: 7064 RVA: 0x000C34F4 File Offset: 0x000C16F4
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position + this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x04001368 RID: 4968
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001369 RID: 4969
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x0400136A RID: 4970
	[Tooltip("How many cells to use squared")]
	public int CellCount = 1;

	// Token: 0x0400136B RID: 4971
	[Tooltip("The size of each cell for async object gathering")]
	public int CellSize = 80;

	// Token: 0x0400136C RID: 4972
	public int Height = 100;

	// Token: 0x0400136D RID: 4973
	public float NavmeshResolutionModifier = 0.5f;

	// Token: 0x0400136E RID: 4974
	[Tooltip("Use the bounds specified in editor instead of generating it from cellsize * cellcount")]
	public bool overrideAutoBounds;

	// Token: 0x0400136F RID: 4975
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x04001370 RID: 4976
	public NavMeshData NavMeshData;

	// Token: 0x04001371 RID: 4977
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x04001372 RID: 4978
	public LayerMask LayerMask;

	// Token: 0x04001373 RID: 4979
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001374 RID: 4980
	public bool forceCollectTerrain;

	// Token: 0x04001375 RID: 4981
	public bool shouldNotifyAIZones = true;

	// Token: 0x04001376 RID: 4982
	public Transform CustomNavMeshRoot;

	// Token: 0x04001377 RID: 4983
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001378 RID: 4984
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001379 RID: 4985
	private AsyncOperation BuildingOperation;

	// Token: 0x0400137A RID: 4986
	private bool HasBuildOperationStarted;

	// Token: 0x0400137B RID: 4987
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x0400137C RID: 4988
	private int defaultArea;

	// Token: 0x0400137D RID: 4989
	private int agentTypeId;
}
