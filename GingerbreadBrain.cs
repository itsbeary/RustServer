using System;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class GingerbreadBrain : BaseAIBrain
{
	// Token: 0x06001A24 RID: 6692 RVA: 0x000BDB05 File Offset: 0x000BBD05
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new GingerbreadBrain.AttackState());
		base.AddState(new GingerbreadBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x000BDB44 File Offset: 0x000BBD44
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x000BDB7F File Offset: 0x000BBD7F
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C5D RID: 3165
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EBA RID: 20154 RVA: 0x0019F562 File Offset: 0x0019D762
		public AttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004EBB RID: 20155 RVA: 0x001A399C File Offset: 0x001A1B9C
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
				Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004EBC RID: 20156 RVA: 0x001A3A90 File Offset: 0x001A1C90
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

		// Token: 0x06004EBD RID: 20157 RVA: 0x001A3AF6 File Offset: 0x001A1CF6
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x001A3B04 File Offset: 0x001A1D04
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
			Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
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

		// Token: 0x06004EBF RID: 20159 RVA: 0x0019F74D File Offset: 0x0019D94D
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x001A3C17 File Offset: 0x001A1E17
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x0400436A RID: 17258
		private IAIAttack attack;

		// Token: 0x0400436B RID: 17259
		private float originalStoppingDistance;
	}

	// Token: 0x02000C5E RID: 3166
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EC1 RID: 20161 RVA: 0x001A3C26 File Offset: 0x001A1E26
		public RoamState()
			: base(AIState.Roam)
		{
		}

		// Token: 0x06004EC2 RID: 20162 RVA: 0x001A3C36 File Offset: 0x001A1E36
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004EC3 RID: 20163 RVA: 0x001A3C48 File Offset: 0x001A1E48
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

		// Token: 0x06004EC4 RID: 20164 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x001A3D25 File Offset: 0x001A1F25
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

		// Token: 0x0400436C RID: 17260
		private StateStatus status = StateStatus.Error;
	}
}
