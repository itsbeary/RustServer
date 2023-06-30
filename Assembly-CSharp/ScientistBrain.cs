using System;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class ScientistBrain : BaseAIBrain
{
	// Token: 0x06001A74 RID: 6772 RVA: 0x000BEE18 File Offset: 0x000BD018
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new ScientistBrain.IdleState());
		base.AddState(new ScientistBrain.RoamState());
		base.AddState(new ScientistBrain.ChaseState());
		base.AddState(new ScientistBrain.CombatState());
		base.AddState(new ScientistBrain.TakeCoverState());
		base.AddState(new ScientistBrain.CoverState());
		base.AddState(new ScientistBrain.MountedState());
		base.AddState(new ScientistBrain.DismountedState());
		base.AddState(new BaseAIBrain.BaseFollowPathState());
		base.AddState(new BaseAIBrain.BaseNavigateHomeState());
		base.AddState(new ScientistBrain.CombatStationaryState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new ScientistBrain.MoveToVector3State());
		base.AddState(new ScientistBrain.BlindedState());
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x000BEEC8 File Offset: 0x000BD0C8
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		ScientistBrain.Count++;
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000BEF1A File Offset: 0x000BD11A
	public override void OnDestroy()
	{
		base.OnDestroy();
		ScientistBrain.Count--;
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x000BEF2E File Offset: 0x000BD12E
	public HumanNPC GetEntity()
	{
		return this.GetBaseEntity() as HumanNPC;
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000BEF3C File Offset: 0x000BD13C
	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (base.CurrentState != null)
		{
			AIState stateType = base.CurrentState.StateType;
			if (stateType <= AIState.Patrol)
			{
				if (stateType - AIState.Idle > 1 && stateType != AIState.Patrol)
				{
					goto IL_46;
				}
			}
			else if (stateType != AIState.FollowPath && stateType != AIState.Cooldown)
			{
				goto IL_46;
			}
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
			return;
			IL_46:
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
		}
	}

	// Token: 0x040012CD RID: 4813
	public static int Count;

	// Token: 0x02000C61 RID: 3169
	public class BlindedState : BaseAIBrain.BaseBlindedState
	{
		// Token: 0x06004ECC RID: 20172 RVA: 0x001A3E38 File Offset: 0x001A2038
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(false);
			humanNPC.Server_StartGesture(235662700U);
			brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(entity.transform.position, 1f, 2.5f), BaseNavigator.NavigationSpeed.Slowest, 0f, 0f);
		}

		// Token: 0x06004ECD RID: 20173 RVA: 0x001A3E9B File Offset: 0x001A209B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			if (entity.ToPlayer() != null)
			{
				entity.ToPlayer().Server_CancelGesture();
			}
		}
	}

	// Token: 0x02000C62 RID: 3170
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004ECF RID: 20175 RVA: 0x001A3ED1 File Offset: 0x001A20D1
		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004ED0 RID: 20176 RVA: 0x001A3EE8 File Offset: 0x001A20E8
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004ED1 RID: 20177 RVA: 0x001A3EF8 File Offset: 0x001A20F8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.nextPositionUpdateTime = 0f;
		}

		// Token: 0x06004ED2 RID: 20178 RVA: 0x001A3F24 File Offset: 0x001A2124
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004ED3 RID: 20179 RVA: 0x001A3F48 File Offset: 0x001A2148
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Error;
			}
			float num = Vector3.Distance(baseEntity.transform.position, entity.transform.position);
			if (brain.Senses.Memory.IsLOS(baseEntity) || num <= 10f || base.TimeInState <= 5f)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			if (num <= 10f)
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Normal);
			}
			else
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Fast);
			}
			if (Time.time > this.nextPositionUpdateTime)
			{
				this.nextPositionUpdateTime = Time.time + UnityEngine.Random.Range(0.5f, 1f);
				Vector3 vector = entity.transform.position;
				AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(baseEntity.transform.position);
				bool flag = false;
				if (informationZone != null)
				{
					AIMovePoint bestMovePointNear = informationZone.GetBestMovePointNear(baseEntity.transform.position, entity.transform.position, 0f, brain.Navigator.BestMovementPointMaxDistance, true, entity, true);
					if (bestMovePointNear)
					{
						bestMovePointNear.SetUsedBy(entity, 5f);
						vector = brain.PathFinder.GetRandomPositionAround(bestMovePointNear.transform.position, 0f, bestMovePointNear.radius - 0.3f);
						flag = true;
					}
				}
				if (!flag)
				{
					return StateStatus.Error;
				}
				if (num < 10f)
				{
					brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
				else
				{
					brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
				}
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x04004376 RID: 17270
		private StateStatus status = StateStatus.Error;

		// Token: 0x04004377 RID: 17271
		private float nextPositionUpdateTime;
	}

	// Token: 0x02000C63 RID: 3171
	public class CombatState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004ED4 RID: 20180 RVA: 0x001A4138 File Offset: 0x001A2338
		public CombatState()
			: base(AIState.Combat)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004ED5 RID: 20181 RVA: 0x001A4148 File Offset: 0x001A2348
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.combatStartPosition = entity.transform.position;
			this.FaceTarget();
		}

		// Token: 0x06004ED6 RID: 20182 RVA: 0x001A4169 File Offset: 0x001A2369
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004ED7 RID: 20183 RVA: 0x001A418C File Offset: 0x001A238C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			this.FaceTarget();
			if (Time.time > this.nextActionTime)
			{
				if (UnityEngine.Random.Range(0, 3) == 1)
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(1f, 2f);
					humanNPC.SetDucked(true);
					brain.Navigator.Stop();
				}
				else
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(2f, 3f);
					humanNPC.SetDucked(false);
					brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(this.combatStartPosition, 1f, 2f), BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x06004ED8 RID: 20184 RVA: 0x001A4250 File Offset: 0x001A2450
		private void FaceTarget()
		{
			BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
		}

		// Token: 0x04004378 RID: 17272
		private float nextActionTime;

		// Token: 0x04004379 RID: 17273
		private Vector3 combatStartPosition;
	}

	// Token: 0x02000C64 RID: 3172
	public class CombatStationaryState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004ED9 RID: 20185 RVA: 0x001A42B3 File Offset: 0x001A24B3
		public CombatStationaryState()
			: base(AIState.CombatStationary)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004EDA RID: 20186 RVA: 0x001A30E6 File Offset: 0x001A12E6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x001A42C4 File Offset: 0x001A24C4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C65 RID: 3173
	public class CoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EDC RID: 20188 RVA: 0x001A431F File Offset: 0x001A251F
		public CoverState()
			: base(AIState.Cover)
		{
		}

		// Token: 0x06004EDD RID: 20189 RVA: 0x001A4328 File Offset: 0x001A2528
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(true);
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.SetUsedBy(entity);
			}
			if (humanNPC.healthFraction <= brain.HealBelowHealthFraction && UnityEngine.Random.Range(0f, 1f) <= brain.HealChance)
			{
				Item item = humanNPC.FindHealingItem();
				if (item != null)
				{
					BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
					if (baseEntity == null || (!brain.Senses.Memory.IsLOS(baseEntity) && Vector3.Distance(entity.transform.position, baseEntity.transform.position) >= 5f))
					{
						humanNPC.UseHealingItem(item);
					}
				}
			}
		}

		// Token: 0x06004EDE RID: 20190 RVA: 0x001A440C File Offset: 0x001A260C
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004EDF RID: 20191 RVA: 0x001A4460 File Offset: 0x001A2660
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			float num = humanNPC.AmmoFractionRemaining();
			if (num == 0f || (baseEntity != null && !brain.Senses.Memory.IsLOS(baseEntity) && num < 0.25f))
			{
				humanNPC.AttemptReload();
			}
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C66 RID: 3174
	public class DismountedState : BaseAIBrain.BaseDismountedState
	{
		// Token: 0x06004EE0 RID: 20192 RVA: 0x001A44F8 File Offset: 0x001A26F8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return;
			}
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, entity.transform.position, 25f, 50f, entity, true);
			if (bestCoverPoint)
			{
				bestCoverPoint.SetUsedBy(entity, 10f);
			}
			Vector3 vector = ((bestCoverPoint == null) ? entity.transform.position : bestCoverPoint.transform.position);
			if (brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Fast, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
		}

		// Token: 0x06004EE1 RID: 20193 RVA: 0x001A45BD File Offset: 0x001A27BD
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

		// Token: 0x0400437A RID: 17274
		private StateStatus status = StateStatus.Error;
	}

	// Token: 0x02000C67 RID: 3175
	public class IdleState : BaseAIBrain.BaseIdleState
	{
	}

	// Token: 0x02000C68 RID: 3176
	public class MountedState : BaseAIBrain.BaseMountedState
	{
	}

	// Token: 0x02000C69 RID: 3177
	public class MoveToVector3State : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EE5 RID: 20197 RVA: 0x001A4608 File Offset: 0x001A2808
		public MoveToVector3State()
			: base(AIState.MoveToVector3)
		{
		}

		// Token: 0x06004EE6 RID: 20198 RVA: 0x001A4612 File Offset: 0x001A2812
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004EE7 RID: 20199 RVA: 0x001A3F24 File Offset: 0x001A2124
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004EE8 RID: 20200 RVA: 0x001A4624 File Offset: 0x001A2824
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 vector = brain.Events.Memory.Position.Get(7);
			if (!brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Fast, 0.5f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C6A RID: 3178
	public class RoamState : BaseAIBrain.BaseRoamState
	{
		// Token: 0x06004EE9 RID: 20201 RVA: 0x001A467D File Offset: 0x001A287D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
			this.ClearRoamPointUsage(entity);
		}

		// Token: 0x06004EEA RID: 20202 RVA: 0x001A4694 File Offset: 0x001A2894
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			this.ClearRoamPointUsage(entity);
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Error;
			this.roamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.transform.position, (entity as HumanNPC).eyes.BodyForward(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
			if (this.roamPoint != null)
			{
				if (brain.Navigator.SetDestination(this.roamPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
				{
					this.roamPoint.SetUsedBy(entity);
					this.status = StateStatus.Running;
					return;
				}
				this.roamPoint.SetUsedBy(entity, 600f);
			}
		}

		// Token: 0x06004EEB RID: 20203 RVA: 0x001A476A File Offset: 0x001A296A
		private void ClearRoamPointUsage(BaseEntity entity)
		{
			if (this.roamPoint != null)
			{
				this.roamPoint.ClearIfUsedBy(entity);
				this.roamPoint = null;
			}
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EED RID: 20205 RVA: 0x001A478D File Offset: 0x001A298D
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			this.PickGoodLookDirection();
			return StateStatus.Finished;
		}

		// Token: 0x06004EEE RID: 20206 RVA: 0x000063A5 File Offset: 0x000045A5
		private void PickGoodLookDirection()
		{
		}

		// Token: 0x0400437B RID: 17275
		private StateStatus status = StateStatus.Error;

		// Token: 0x0400437C RID: 17276
		private AIMovePoint roamPoint;
	}

	// Token: 0x02000C6B RID: 3179
	public class TakeCoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EF0 RID: 20208 RVA: 0x001A47C4 File Offset: 0x001A29C4
		public TakeCoverState()
			: base(AIState.TakeCover)
		{
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x001A47D5 File Offset: 0x001A29D5
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Running;
			if (!this.StartMovingToCover(entity as HumanNPC))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004EF2 RID: 20210 RVA: 0x001A47FB File Offset: 0x001A29FB
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			this.ClearCoverPointUsage(entity);
		}

		// Token: 0x06004EF3 RID: 20211 RVA: 0x001A4818 File Offset: 0x001A2A18
		private void ClearCoverPointUsage(BaseEntity entity)
		{
			AIPoint aipoint = this.brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004EF4 RID: 20212 RVA: 0x001A4854 File Offset: 0x001A2A54
		private bool StartMovingToCover(HumanNPC entity)
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			Vector3 vector = (this.coverFromEntity ? this.coverFromEntity.transform.position : (entity.transform.position + entity.LastAttackedDir * 30f));
			AIInformationZone informationZone = entity.GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return false;
			}
			float num = ((entity.SecondsSinceAttacked < 2f) ? 2f : 0f);
			float bestCoverPointMaxDistance = this.brain.Navigator.BestCoverPointMaxDistance;
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, vector, num, bestCoverPointMaxDistance, entity, true);
			if (bestCoverPoint == null)
			{
				return false;
			}
			Vector3 position = bestCoverPoint.transform.position;
			if (!this.brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				return false;
			}
			this.FaceCoverFromEntity();
			this.brain.Events.Memory.AIPoint.Set(bestCoverPoint, 4);
			bestCoverPoint.SetUsedBy(entity);
			return true;
		}

		// Token: 0x06004EF5 RID: 20213 RVA: 0x001A4998 File Offset: 0x001A2B98
		public override void DrawGizmos()
		{
			base.DrawGizmos();
		}

		// Token: 0x06004EF6 RID: 20214 RVA: 0x001A49A0 File Offset: 0x001A2BA0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			this.FaceCoverFromEntity();
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

		// Token: 0x06004EF7 RID: 20215 RVA: 0x001A49D4 File Offset: 0x001A2BD4
		private void FaceCoverFromEntity()
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (this.coverFromEntity == null)
			{
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(this.coverFromEntity);
		}

		// Token: 0x0400437D RID: 17277
		private StateStatus status = StateStatus.Error;

		// Token: 0x0400437E RID: 17278
		private BaseEntity coverFromEntity;
	}
}
