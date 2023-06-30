using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B4A RID: 2890
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent
	{
		// Token: 0x060045F5 RID: 17909 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void OnEnableAgency()
		{
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void OnDisableAgency()
		{
		}

		// Token: 0x060045F7 RID: 17911 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void UpdateAgency()
		{
		}

		// Token: 0x060045F8 RID: 17912 RVA: 0x00197BE0 File Offset: 0x00195DE0
		internal void OnEnableCover()
		{
			if (this.coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, this.CoverPointVolumeCellSize);
			}
		}

		// Token: 0x060045F9 RID: 17913 RVA: 0x00197C14 File Offset: 0x00195E14
		internal void OnDisableCover()
		{
			if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
			{
				return;
			}
			for (int i = 0; i < this.coverPointVolumeGrid.Cells.Length; i++)
			{
				UnityEngine.Object.Destroy(this.coverPointVolumeGrid.Cells[i]);
			}
		}

		// Token: 0x060045FA RID: 17914 RVA: 0x00197C64 File Offset: 0x00195E64
		public static CoverPointVolume CreateNewCoverVolume(Vector3 point, Transform coverPointGroup)
		{
			if (SingletonComponent<AiManager>.Instance != null && SingletonComponent<AiManager>.Instance.enabled && SingletonComponent<AiManager>.Instance.UseCover)
			{
				CoverPointVolume coverPointVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(point);
				if (coverPointVolume == null)
				{
					Vector2i vector2i = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.WorldToGridCoords(point);
					if (SingletonComponent<AiManager>.Instance.cpvPrefab != null)
					{
						coverPointVolume = UnityEngine.Object.Instantiate<CoverPointVolume>(SingletonComponent<AiManager>.Instance.cpvPrefab);
					}
					else
					{
						coverPointVolume = new GameObject("CoverPointVolume").AddComponent<CoverPointVolume>();
					}
					coverPointVolume.transform.localPosition = default(Vector3);
					coverPointVolume.transform.position = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.GridToWorldCoords(vector2i) + Vector3.up * point.y;
					coverPointVolume.transform.localScale = new Vector3(SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellHeight, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize);
					coverPointVolume.CoverLayerMask = SingletonComponent<AiManager>.Instance.DynamicCoverPointVolumeLayerMask;
					coverPointVolume.CoverPointRayLength = SingletonComponent<AiManager>.Instance.CoverPointRayLength;
					SingletonComponent<AiManager>.Instance.coverPointVolumeGrid[vector2i] = coverPointVolume;
					coverPointVolume.GenerateCoverPoints(coverPointGroup);
				}
				return coverPointVolume;
			}
			return null;
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x00197DAC File Offset: 0x00195FAC
		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			if (this.coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i vector2i = this.coverPointVolumeGrid.WorldToGridCoords(point);
			return this.coverPointVolumeGrid[vector2i];
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x060045FD RID: 17917 RVA: 0x00197DE4 File Offset: 0x00195FE4
		// (set) Token: 0x060045FC RID: 17916 RVA: 0x00197DDC File Offset: 0x00195FDC
		[ServerVar(Help = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.pathfindingIterationsPerFrame;
			}
			set
			{
				NavMesh.pathfindingIterationsPerFrame = value;
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x060045FE RID: 17918 RVA: 0x0000441C File Offset: 0x0000261C
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060045FF RID: 17919 RVA: 0x00197DEB File Offset: 0x00195FEB
		public void Initialize()
		{
			this.OnEnableAgency();
			if (this.UseCover)
			{
				this.OnEnableCover();
			}
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x00197E01 File Offset: 0x00196001
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			this.OnDisableAgency();
			if (this.UseCover)
			{
				this.OnDisableCover();
			}
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x00197E1F File Offset: 0x0019601F
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (AiManager.nav_disable)
			{
				return new float?(nextInterval);
			}
			this.UpdateAgency();
			return new float?(UnityEngine.Random.value + 1f);
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x00197E48 File Offset: 0x00196048
		private static bool InterestedInPlayersOnly(BaseEntity entity)
		{
			BasePlayer basePlayer = entity as BasePlayer;
			return !(basePlayer == null) && !basePlayer.IsSleeping() && basePlayer.IsConnected;
		}

		// Token: 0x04003ED7 RID: 16087
		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		// Token: 0x04003ED8 RID: 16088
		public float CoverPointVolumeCellSize = 20f;

		// Token: 0x04003ED9 RID: 16089
		public float CoverPointVolumeCellHeight = 8f;

		// Token: 0x04003EDA RID: 16090
		public float CoverPointRayLength = 1f;

		// Token: 0x04003EDB RID: 16091
		public CoverPointVolume cpvPrefab;

		// Token: 0x04003EDC RID: 16092
		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		// Token: 0x04003EDD RID: 16093
		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		// Token: 0x04003EDE RID: 16094
		[ServerVar(Help = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
		public static bool nav_wait = true;

		// Token: 0x04003EDF RID: 16095
		[ServerVar(Help = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
		public static bool nav_disable = false;

		// Token: 0x04003EE0 RID: 16096
		[ServerVar(Help = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.")]
		public static bool setdestination_navmesh_failsafe = false;

		// Token: 0x04003EE1 RID: 16097
		[ServerVar(Help = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
		public static bool ai_dormant = true;

		// Token: 0x04003EE2 RID: 16098
		[ServerVar(Help = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
		public static float ai_to_player_distance_wakeup_range = 160f;

		// Token: 0x04003EE3 RID: 16099
		[ServerVar(Help = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
		public static int nav_obstacles_carve_state = 2;

		// Token: 0x04003EE4 RID: 16100
		[ServerVar(Help = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
		public static int ai_dormant_max_wakeup_per_tick = 30;

		// Token: 0x04003EE5 RID: 16101
		[ServerVar(Help = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_tick_budget = 4f;

		// Token: 0x04003EE6 RID: 16102
		[ServerVar(Help = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_junkpile_tick_budget = 4f;

		// Token: 0x04003EE7 RID: 16103
		[ServerVar(Help = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_animal_tick_budget = 4f;

		// Token: 0x04003EE8 RID: 16104
		[ServerVar(Help = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
		public static bool ai_htn_use_agency_tick = true;

		// Token: 0x04003EE9 RID: 16105
		private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];

		// Token: 0x04003EEA RID: 16106
		private readonly Func<BasePlayer, bool> filter = new Func<BasePlayer, bool>(AiManager.InterestedInPlayersOnly);
	}
}
