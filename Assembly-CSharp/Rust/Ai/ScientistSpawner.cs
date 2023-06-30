using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B50 RID: 2896
	public class ScientistSpawner : SpawnGroup
	{
		// Token: 0x06004614 RID: 17940 RVA: 0x001986E8 File Offset: 0x001968E8
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (base.currentPopulation == this.maxPopulation)
			{
				this._lastSpawnCallHadMaxAliveMembers = true;
				this._lastSpawnCallHadAliveMembers = true;
				return;
			}
			if (this._lastSpawnCallHadMaxAliveMembers)
			{
				this._nextForcedRespawn = UnityEngine.Time.time + 2200f;
			}
			if (UnityEngine.Time.time < this._nextForcedRespawn)
			{
				if (base.currentPopulation == 0 && this._lastSpawnCallHadAliveMembers)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = false;
					return;
				}
				if (base.currentPopulation > 0)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
					return;
				}
			}
			this._lastSpawnCallHadMaxAliveMembers = false;
			this._lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
			base.Spawn(numToSpawn);
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x000063A5 File Offset: 0x000045A5
		protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
		{
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x0019879C File Offset: 0x0019699C
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
			{
				Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
				foreach (Transform transform in this.LookAtInterestPointsStationary)
				{
					if (transform != null)
					{
						Gizmos.DrawSphere(transform.position, 0.1f);
						Gizmos.DrawLine(base.transform.position, transform.position);
					}
				}
			}
		}

		// Token: 0x04003EFB RID: 16123
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		// Token: 0x04003EFC RID: 16124
		public bool NeverMove;

		// Token: 0x04003EFD RID: 16125
		public bool SpawnHostile;

		// Token: 0x04003EFE RID: 16126
		public bool OnlyAggroMarkedTargets = true;

		// Token: 0x04003EFF RID: 16127
		public bool IsPeacekeeper = true;

		// Token: 0x04003F00 RID: 16128
		public bool IsBandit;

		// Token: 0x04003F01 RID: 16129
		public bool IsMilitaryTunnelLab;

		// Token: 0x04003F02 RID: 16130
		public WaypointSet Waypoints;

		// Token: 0x04003F03 RID: 16131
		public Transform[] LookAtInterestPointsStationary;

		// Token: 0x04003F04 RID: 16132
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04003F05 RID: 16133
		public Model Model;

		// Token: 0x04003F06 RID: 16134
		[SerializeField]
		private AiLocationManager _mgr;

		// Token: 0x04003F07 RID: 16135
		private float _nextForcedRespawn = float.PositiveInfinity;

		// Token: 0x04003F08 RID: 16136
		private bool _lastSpawnCallHadAliveMembers;

		// Token: 0x04003F09 RID: 16137
		private bool _lastSpawnCallHadMaxAliveMembers;
	}
}
