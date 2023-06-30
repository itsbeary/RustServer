using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class FrankensteinBrain : PetBrain
{
	// Token: 0x06001ABB RID: 6843 RVA: 0x000BFD7C File Offset: 0x000BDF7C
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new FrankensteinBrain.MoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new FrankensteinBrain.MoveToPointState());
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000BFDBB File Offset: 0x000BDFBB
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x000BFDF6 File Offset: 0x000BDFF6
	public FrankensteinPet GetEntity()
	{
		return this.GetBaseEntity() as FrankensteinPet;
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x000BFE03 File Offset: 0x000BE003
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x040012E9 RID: 4841
	[ServerVar]
	public static float MoveTowardsRate = 1f;

	// Token: 0x02000C6E RID: 3182
	public class MoveToPointState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EFE RID: 20222 RVA: 0x001A31C3 File Offset: 0x001A13C3
		public MoveToPointState()
			: base(AIState.MoveToPoint)
		{
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x001A4B18 File Offset: 0x001A2D18
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			this.originalStopDistance = navigator.StoppingDistance;
			navigator.StoppingDistance = 0.5f;
		}

		// Token: 0x06004F00 RID: 20224 RVA: 0x001A4B4B File Offset: 0x001A2D4B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.StoppingDistance = this.originalStopDistance;
			this.Stop();
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004F02 RID: 20226 RVA: 0x001A4B6C File Offset: 0x001A2D6C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 vector = brain.Events.Memory.Position.Get(6);
			if (!brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				brain.LoadDefaultAIDesign();
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004388 RID: 17288
		private float originalStopDistance;
	}

	// Token: 0x02000C6F RID: 3183
	public class MoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004F03 RID: 20227 RVA: 0x0019FDA4 File Offset: 0x0019DFA4
		public MoveTorwardsState()
			: base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004F04 RID: 20228 RVA: 0x001A4BD9 File Offset: 0x001A2DD9
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004F05 RID: 20229 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x001A4BEC File Offset: 0x001A2DEC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
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
}
