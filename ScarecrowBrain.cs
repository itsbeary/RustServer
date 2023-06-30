using System;
using ConVar;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class ScarecrowBrain : BaseAIBrain
{
	// Token: 0x06001AC4 RID: 6852 RVA: 0x000BFE1F File Offset: 0x000BE01F
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new ScarecrowBrain.ChaseState());
		base.AddState(new ScarecrowBrain.AttackState());
		base.AddState(new ScarecrowBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x000BDB44 File Offset: 0x000BBD44
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x000BDB7F File Offset: 0x000BBD7F
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C70 RID: 3184
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004F07 RID: 20231 RVA: 0x0019F562 File Offset: 0x0019D762
		public AttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004F08 RID: 20232 RVA: 0x001A4C6C File Offset: 0x001A2E6C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.originalStoppingDistance = brain.Navigator.StoppingDistance;
			brain.Navigator.Agent.stoppingDistance = 1f;
			brain.Navigator.StoppingDistance = 1f;
			base.StateEnter(brain, entity);
			this.attack = entity as IAIAttack;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004F09 RID: 20233 RVA: 0x001A4D60 File Offset: 0x001A2F60
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			brain.Navigator.Agent.stoppingDistance = this.originalStoppingDistance;
			brain.Navigator.StoppingDistance = this.originalStoppingDistance;
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004F0A RID: 20234 RVA: 0x001A4DC6 File Offset: 0x001A2FC6
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004F0B RID: 20235 RVA: 0x001A4DD4 File Offset: 0x001A2FD4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			Vector3Ex.Direction2D(baseEntity.transform.position, entity.transform.position);
			Vector3 position = baseEntity.transform.position;
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Fast, 0.2f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x0019F74D File Offset: 0x0019D94D
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004F0D RID: 20237 RVA: 0x001A4EE7 File Offset: 0x001A30E7
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04004389 RID: 17289
		private IAIAttack attack;

		// Token: 0x0400438A RID: 17290
		private float originalStoppingDistance;
	}

	// Token: 0x02000C71 RID: 3185
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004F0E RID: 20238 RVA: 0x0019F76F File Offset: 0x0019D96F
		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004F0F RID: 20239 RVA: 0x001A4EF8 File Offset: 0x001A30F8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.throwDelayTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.2f, 0.5f);
			this.useBeanCan = (float)UnityEngine.Random.Range(0, 100) <= 20f;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004F10 RID: 20240 RVA: 0x001A4F9B File Offset: 0x001A319B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			this.Stop();
		}

		// Token: 0x06004F11 RID: 20241 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004F12 RID: 20242 RVA: 0x001A4FBC File Offset: 0x001A31BC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (this.useBeanCan && UnityEngine.Time.time >= this.throwDelayTime && AI.npc_use_thrown_weapons && Halloween.scarecrows_throw_beancans && UnityEngine.Time.time >= ScarecrowNPC.NextBeanCanAllowedTime && (brain.GetBrainBaseEntity() as ScarecrowNPC).TryUseThrownWeapon(baseEntity, 10f))
			{
				brain.Navigator.Stop();
				return StateStatus.Running;
			}
			if ((brain.GetBrainBaseEntity() as BasePlayer).modelState.aiming)
			{
				return StateStatus.Running;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x0400438B RID: 17291
		private float throwDelayTime;

		// Token: 0x0400438C RID: 17292
		private bool useBeanCan;
	}

	// Token: 0x02000C72 RID: 3186
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004F13 RID: 20243 RVA: 0x001A50A7 File Offset: 0x001A32A7
		public RoamState()
			: base(AIState.Roam)
		{
		}

		// Token: 0x06004F14 RID: 20244 RVA: 0x001A50B7 File Offset: 0x001A32B7
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004F15 RID: 20245 RVA: 0x001A50C8 File Offset: 0x001A32C8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			ScarecrowNPC scarecrowNPC = entity as ScarecrowNPC;
			if (scarecrowNPC == null)
			{
				return;
			}
			Vector3 vector = brain.Events.Memory.Position.Get(4);
			Vector3 vector2;
			if (scarecrowNPC.RoamAroundHomePoint)
			{
				vector2 = brain.PathFinder.GetBestRoamPositionFromAnchor(brain.Navigator, vector, vector, 1f, brain.Navigator.BestRoamPointMaxDistance);
			}
			else
			{
				vector2 = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 10f, brain.Navigator.BestRoamPointMaxDistance);
			}
			if (brain.Navigator.SetDestination(vector2, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004F16 RID: 20246 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004F17 RID: 20247 RVA: 0x001A51A5 File Offset: 0x001A33A5
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x0400438D RID: 17293
		private StateStatus status = StateStatus.Error;
	}
}
