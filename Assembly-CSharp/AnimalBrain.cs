using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class AnimalBrain : BaseAIBrain
{
	// Token: 0x060019C9 RID: 6601 RVA: 0x000BC7C0 File Offset: 0x000BA9C0
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new AnimalBrain.IdleState());
		base.AddState(new AnimalBrain.MoveTowardsState());
		base.AddState(new AnimalBrain.FleeState());
		base.AddState(new AnimalBrain.RoamState());
		base.AddState(new AnimalBrain.AttackState());
		base.AddState(new BaseAIBrain.BaseSleepState());
		base.AddState(new AnimalBrain.ChaseState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
		base.AddState(new AnimalBrain.MoveToPointState());
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x000BC836 File Offset: 0x000BAA36
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new BasePathFinder();
		AnimalBrain.Count++;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x000BC867 File Offset: 0x000BAA67
	public override void OnDestroy()
	{
		base.OnDestroy();
		AnimalBrain.Count--;
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x000BC87B File Offset: 0x000BAA7B
	public BaseAnimalNPC GetEntity()
	{
		return this.GetBaseEntity() as BaseAnimalNPC;
	}

	// Token: 0x0400126F RID: 4719
	public static BaseNavigator.NavigationSpeed ControlTestAnimalSpeed = BaseNavigator.NavigationSpeed.Fast;

	// Token: 0x04001270 RID: 4720
	public static int Count;

	// Token: 0x02000C51 RID: 3153
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E79 RID: 20089 RVA: 0x0019F562 File Offset: 0x0019D762
		public AttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x001A2AD0 File Offset: 0x001A0CD0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = entity as IAIAttack;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return;
			}
			if (baseEntity != null && baseEntity.Health() > 0f)
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, baseCombatEntity);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x001A2B9E File Offset: 0x001A0D9E
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x001A2BC4 File Offset: 0x001A0DC4
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004E7D RID: 20093 RVA: 0x001A2BD4 File Offset: 0x001A0DD4
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
			if (baseEntity.Health() <= 0f)
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BaseVehicle baseVehicle = ((basePlayer != null) ? basePlayer.GetMountedVehicle() : null);
			if (baseVehicle != null && baseVehicle is BaseModularVehicle)
			{
				this.StopAttacking();
				return StateStatus.Error;
			}
			if (brain.Senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, baseCombatEntity);
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

		// Token: 0x06004E7E RID: 20094 RVA: 0x001A2D38 File Offset: 0x001A0F38
		private static Vector3 GetAimDirection(BaseCombatEntity from, BaseCombatEntity target)
		{
			if (!(from == null) && !(target == null))
			{
				return Vector3Ex.Direction2D(target.transform.position, from.transform.position);
			}
			if (!(from != null))
			{
				return Vector3.forward;
			}
			return from.transform.forward;
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x001A2D8D File Offset: 0x001A0F8D
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04004358 RID: 17240
		private IAIAttack attack;
	}

	// Token: 0x02000C52 RID: 3154
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E80 RID: 20096 RVA: 0x0019F76F File Offset: 0x0019D96F
		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x001A2D9C File Offset: 0x001A0F9C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = entity as IAIAttack;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004E82 RID: 20098 RVA: 0x001A2E09 File Offset: 0x001A1009
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E83 RID: 20099 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E84 RID: 20100 RVA: 0x001A2E1C File Offset: 0x001A101C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004359 RID: 17241
		private IAIAttack attack;
	}

	// Token: 0x02000C53 RID: 3155
	public class FleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E85 RID: 20101 RVA: 0x001A2EB7 File Offset: 0x001A10B7
		public FleeState()
			: base(AIState.Flee)
		{
		}

		// Token: 0x06004E86 RID: 20102 RVA: 0x001A2ECC File Offset: 0x001A10CC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x06004E87 RID: 20103 RVA: 0x001A2F78 File Offset: 0x001A1178
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E88 RID: 20104 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E89 RID: 20105 RVA: 0x001A2F88 File Offset: 0x001A1188
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004E8A RID: 20106 RVA: 0x001A3028 File Offset: 0x001A1228
		private bool FleeFrom(BaseEntity fleeFromEntity, BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 vector;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out vector))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}

		// Token: 0x0400435A RID: 17242
		private float nextInterval = 2f;

		// Token: 0x0400435B RID: 17243
		private float stopFleeDistance;
	}

	// Token: 0x02000C54 RID: 3156
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004E8B RID: 20107 RVA: 0x001A30D5 File Offset: 0x001A12D5
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.FaceNewDirection(entity);
		}

		// Token: 0x06004E8C RID: 20108 RVA: 0x001A30E6 File Offset: 0x001A12E6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E8D RID: 20109 RVA: 0x001A30FC File Offset: 0x001A12FC
		private void FaceNewDirection(BaseEntity entity)
		{
			if (UnityEngine.Random.Range(0, 100) <= this.turnChance)
			{
				Vector3 position = entity.transform.position;
				Vector3 normalized = (BasePathFinder.GetPointOnCircle(position, 1f, UnityEngine.Random.Range(0f, 594f)) - position).normalized;
				this.brain.Navigator.SetFacingDirectionOverride(normalized);
			}
			this.nextTurnTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(this.minTurnTime, this.maxTurnTime);
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x001A317C File Offset: 0x001A137C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Time.realtimeSinceStartup >= this.nextTurnTime)
			{
				this.FaceNewDirection(entity);
			}
			return StateStatus.Running;
		}

		// Token: 0x0400435C RID: 17244
		private float nextTurnTime;

		// Token: 0x0400435D RID: 17245
		private float minTurnTime = 10f;

		// Token: 0x0400435E RID: 17246
		private float maxTurnTime = 20f;

		// Token: 0x0400435F RID: 17247
		private int turnChance = 33;
	}

	// Token: 0x02000C55 RID: 3157
	public class MoveToPointState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E90 RID: 20112 RVA: 0x001A31C3 File Offset: 0x001A13C3
		public MoveToPointState()
			: base(AIState.MoveToPoint)
		{
		}

		// Token: 0x06004E91 RID: 20113 RVA: 0x001A31D0 File Offset: 0x001A13D0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			this.originalStopDistance = navigator.StoppingDistance;
			navigator.StoppingDistance = 0.5f;
		}

		// Token: 0x06004E92 RID: 20114 RVA: 0x001A3203 File Offset: 0x001A1403
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.StoppingDistance = this.originalStopDistance;
			this.Stop();
		}

		// Token: 0x06004E93 RID: 20115 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E94 RID: 20116 RVA: 0x001A3224 File Offset: 0x001A1424
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 vector = brain.Events.Memory.Position.Get(6);
			if (!brain.Navigator.SetDestination(vector, AnimalBrain.ControlTestAnimalSpeed, 0f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004360 RID: 17248
		private float originalStopDistance;
	}

	// Token: 0x02000C56 RID: 3158
	public class MoveTowardsState : BaseAIBrain.BaseMoveTorwardsState
	{
	}

	// Token: 0x02000C57 RID: 3159
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E96 RID: 20118 RVA: 0x001A3289 File Offset: 0x001A1489
		public RoamState()
			: base(AIState.Roam)
		{
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x001A3299 File Offset: 0x001A1499
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x001A32AC File Offset: 0x001A14AC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 vector;
			if (brain.InGroup() && !brain.IsGroupLeader)
			{
				vector = brain.Events.Memory.Position.Get(5);
				vector = BasePathFinder.GetPointOnCircle(vector, UnityEngine.Random.Range(2f, 7f), UnityEngine.Random.Range(0f, 359f));
			}
			else
			{
				vector = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 20f, 100f);
			}
			if (brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				if (brain.InGroup() && brain.IsGroupLeader)
				{
					brain.SetGroupRoamRootPosition(vector);
				}
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x001A3396 File Offset: 0x001A1596
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

		// Token: 0x04004361 RID: 17249
		private StateStatus status = StateStatus.Error;
	}
}
