using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EB RID: 491
public class FishBrain : BaseAIBrain
{
	// Token: 0x060019E7 RID: 6631 RVA: 0x000BC9D4 File Offset: 0x000BABD4
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new FishBrain.IdleState());
		base.AddState(new FishBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000BCA34 File Offset: 0x000BAC34
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new UnderwaterPathFinder();
		((UnderwaterPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		FishBrain.Count++;
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000BCA86 File Offset: 0x000BAC86
	public override void OnDestroy()
	{
		base.OnDestroy();
		FishBrain.Count--;
	}

	// Token: 0x04001275 RID: 4725
	public static int Count;

	// Token: 0x02000C58 RID: 3160
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004E9B RID: 20123 RVA: 0x001A33C2 File Offset: 0x001A15C2
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x001A33D4 File Offset: 0x001A15D4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.idleRootPos = brain.Navigator.transform.position;
			this.GenerateIdlePoints(20f, 0f);
			this.currentPointIndex = 0;
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[0], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x001A3464 File Offset: 0x001A1664
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Vector3.Distance(brain.Navigator.transform.position, this.idleRootPos + this.idlePoints[this.currentPointIndex]) < 4f)
			{
				this.currentPointIndex++;
			}
			if (this.currentPointIndex >= this.idlePoints.Count)
			{
				this.currentPointIndex = 0;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[this.currentPointIndex], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
			else
			{
				this.status = StateStatus.Error;
			}
			return this.status;
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x001A3528 File Offset: 0x001A1728
		private void GenerateIdlePoints(float radius, float heightOffset)
		{
			if (this.idlePoints != null)
			{
				return;
			}
			this.idlePoints = new List<Vector3>();
			float num = 0f;
			int num2 = 32;
			float height = TerrainMeta.WaterMap.GetHeight(this.brain.Navigator.transform.position);
			float height2 = TerrainMeta.HeightMap.GetHeight(this.brain.Navigator.transform.position);
			for (int i = 0; i < num2; i++)
			{
				num += 360f / (float)num2;
				Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(Vector3.zero, radius, num);
				pointOnCircle.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
				pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
				this.idlePoints.Add(pointOnCircle);
			}
		}

		// Token: 0x04004362 RID: 17250
		private StateStatus status = StateStatus.Error;

		// Token: 0x04004363 RID: 17251
		private List<Vector3> idlePoints;

		// Token: 0x04004364 RID: 17252
		private int currentPointIndex;

		// Token: 0x04004365 RID: 17253
		private Vector3 idleRootPos;
	}

	// Token: 0x02000C59 RID: 3161
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EA1 RID: 20129 RVA: 0x001A35FF File Offset: 0x001A17FF
		public RoamState()
			: base(AIState.Roam)
		{
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x001A360F File Offset: 0x001A180F
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x001A3620 File Offset: 0x001A1820
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 vector = brain.Events.Memory.Position.Get(4);
			Vector3 bestRoamPosition = brain.PathFinder.GetBestRoamPosition(brain.Navigator, vector, 5f, brain.Navigator.MaxRoamDistanceFromHome);
			if (brain.Navigator.SetDestination(bestRoamPosition, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x001A36A7 File Offset: 0x001A18A7
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

		// Token: 0x04004366 RID: 17254
		private StateStatus status = StateStatus.Error;
	}
}
