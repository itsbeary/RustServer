using System;
using UnityEngine;

// Token: 0x02000483 RID: 1155
public class CH47AIBrain : BaseAIBrain
{
	// Token: 0x06002639 RID: 9785 RVA: 0x000F1874 File Offset: 0x000EFA74
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new CH47AIBrain.IdleState());
		base.AddState(new CH47AIBrain.PatrolState());
		base.AddState(new CH47AIBrain.OrbitState());
		base.AddState(new CH47AIBrain.EgressState());
		base.AddState(new CH47AIBrain.DropCrate());
		base.AddState(new CH47AIBrain.LandState());
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000F18C9 File Offset: 0x000EFAC9
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.FixedUpdate;
		base.PathFinder = new CH47PathFinder();
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000F18E3 File Offset: 0x000EFAE3
	public void FixedUpdate()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		this.Think(Time.fixedDeltaTime);
	}

	// Token: 0x02000D17 RID: 3351
	public class DropCrate : BaseAIBrain.BasicAIState
	{
		// Token: 0x0600504B RID: 20555 RVA: 0x001A860D File Offset: 0x001A680D
		public DropCrate()
			: base(AIState.DropCrate)
		{
		}

		// Token: 0x0600504C RID: 20556 RVA: 0x001A8617 File Offset: 0x001A6817
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && !this.CanDrop();
		}

		// Token: 0x0600504D RID: 20557 RVA: 0x001A862C File Offset: 0x001A682C
		public bool CanDrop()
		{
			return Time.time > this.nextDropTime && (this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).CanDropCrate();
		}

		// Token: 0x0600504E RID: 20558 RVA: 0x001A8654 File Offset: 0x001A6854
		public override float GetWeight()
		{
			if (!this.CanDrop())
			{
				return 0f;
			}
			if (base.IsInState())
			{
				return 10000f;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Orbit && this.brain.CurrentState.TimeInState > 60f)
			{
				CH47DropZone closest = CH47DropZone.GetClosest(this.brain.mainInterestPoint);
				if (closest && Vector3Ex.Distance2D(closest.transform.position, this.brain.mainInterestPoint) < 200f)
				{
					CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
					if (component != null)
					{
						float num = Mathf.InverseLerp(300f, 600f, component.Age);
						return 1000f * num;
					}
				}
			}
			return 0f;
		}

		// Token: 0x0600504F RID: 20559 RVA: 0x001A8730 File Offset: 0x001A6930
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetDropDoorOpen(true);
			ch47HelicopterAIController.EnableFacingOverride(false);
			CH47DropZone closest = CH47DropZone.GetClosest(ch47HelicopterAIController.transform.position);
			if (closest == null)
			{
				this.nextDropTime = Time.time + 60f;
			}
			brain.mainInterestPoint = closest.transform.position;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06005050 RID: 20560 RVA: 0x001A87A0 File Offset: 0x001A69A0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			if (this.CanDrop() && Vector3Ex.Distance2D(brain.mainInterestPoint, ch47HelicopterAIController.transform.position) < 5f && ch47HelicopterAIController.rigidBody.velocity.magnitude < 5f)
			{
				ch47HelicopterAIController.DropCrate();
				this.nextDropTime = Time.time + 120f;
			}
			return StateStatus.Running;
		}

		// Token: 0x06005051 RID: 20561 RVA: 0x001A8815 File Offset: 0x001A6A15
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			(entity as CH47HelicopterAIController).SetDropDoorOpen(false);
			this.nextDropTime = Time.time + 60f;
			base.StateLeave(brain, entity);
		}

		// Token: 0x040046BF RID: 18111
		private float nextDropTime;
	}

	// Token: 0x02000D18 RID: 3352
	public class EgressState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06005052 RID: 20562 RVA: 0x001A883C File Offset: 0x001A6A3C
		public EgressState()
			: base(AIState.Egress)
		{
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x00007A44 File Offset: 0x00005C44
		public override bool CanInterrupt()
		{
			return false;
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x001A8848 File Offset: 0x001A6A48
		public override float GetWeight()
		{
			CH47HelicopterAIController ch47HelicopterAIController = this.brain.GetBrainBaseEntity() as CH47HelicopterAIController;
			if (ch47HelicopterAIController.OutOfCrates() && !ch47HelicopterAIController.ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
			if (!(component != null))
			{
				return 0f;
			}
			if (component.Age <= 1800f)
			{
				return 0f;
			}
			return 10000f;
		}

		// Token: 0x06005055 RID: 20565 RVA: 0x001A88B0 File Offset: 0x001A6AB0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			Transform transform = ch47HelicopterAIController.transform;
			Rigidbody rigidBody = ch47HelicopterAIController.rigidBody;
			Vector3 vector = ((rigidBody.velocity.magnitude < 0.1f) ? transform.forward : rigidBody.velocity.normalized);
			Vector3 vector2 = Vector3.Cross(Vector3.Cross(transform.up, vector), Vector3.up);
			brain.mainInterestPoint = transform.position + vector2 * 8000f;
			brain.mainInterestPoint.y = 100f;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06005056 RID: 20566 RVA: 0x001A895C File Offset: 0x001A6B5C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.killing)
			{
				return StateStatus.Running;
			}
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			if (position.y < 85f && !this.egressAltitueAchieved)
			{
				CH47LandingZone closest = CH47LandingZone.GetClosest(position);
				if (closest != null && Vector3Ex.Distance2D(closest.transform.position, position) < 20f)
				{
					float num = 0f;
					if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
					{
						num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(position), TerrainMeta.HeightMap.GetHeight(position));
					}
					num += 100f;
					Vector3 vector = position;
					vector.y = num;
					ch47HelicopterAIController.SetMoveTarget(vector);
					return StateStatus.Running;
				}
			}
			this.egressAltitueAchieved = true;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			if (base.TimeInState > 300f)
			{
				ch47HelicopterAIController.Invoke("DelayedKill", 2f);
				this.killing = true;
			}
			return StateStatus.Running;
		}

		// Token: 0x040046C0 RID: 18112
		private bool killing;

		// Token: 0x040046C1 RID: 18113
		private bool egressAltitueAchieved;
	}

	// Token: 0x02000D19 RID: 3353
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06005057 RID: 20567 RVA: 0x0002C198 File Offset: 0x0002A398
		public override float GetWeight()
		{
			return 0.1f;
		}

		// Token: 0x06005058 RID: 20568 RVA: 0x001A8A64 File Offset: 0x001A6C64
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetMoveTarget(ch47HelicopterAIController.GetPosition() + ch47HelicopterAIController.rigidBody.velocity.normalized * 10f);
			base.StateEnter(brain, entity);
		}
	}

	// Token: 0x02000D1A RID: 3354
	public class LandState : BaseAIBrain.BasicAIState
	{
		// Token: 0x0600505A RID: 20570 RVA: 0x001A8AAE File Offset: 0x001A6CAE
		public LandState()
			: base(AIState.Land)
		{
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x001A8AC4 File Offset: 0x001A6CC4
		public override float GetWeight()
		{
			if (!(this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).ShouldLand())
			{
				return 0f;
			}
			float num = Time.time - this.lastLandtime;
			if (base.IsInState() && this.landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!base.IsInState() && num > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x001A8B34 File Offset: 0x001A6D34
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			Vector3 forward = ch47HelicopterAIController.transform.forward;
			CH47LandingZone closest = CH47LandingZone.GetClosest(ch47HelicopterAIController.landingTarget);
			if (!closest)
			{
				return StateStatus.Error;
			}
			float magnitude = ch47HelicopterAIController.rigidBody.velocity.magnitude;
			float num = Vector3Ex.Distance2D(closest.transform.position, position);
			bool flag = num < 40f;
			bool flag2 = num > 15f && position.y < closest.transform.position.y + 10f;
			ch47HelicopterAIController.EnableFacingOverride(flag);
			ch47HelicopterAIController.SetAltitudeProtection(flag2);
			bool flag3 = Mathf.Abs(closest.transform.position.y - position.y) < 3f && num <= 5f && magnitude < 1f;
			if (flag3)
			{
				this.landedForSeconds += delta;
				if (this.lastLandtime == 0f)
				{
					this.lastLandtime = Time.time;
				}
			}
			float num2 = 1f - Mathf.InverseLerp(0f, 7f, num);
			this.landingHeight -= 4f * num2 * Time.deltaTime;
			if (this.landingHeight < -5f)
			{
				this.landingHeight = -5f;
			}
			ch47HelicopterAIController.SetAimDirection(closest.transform.forward);
			Vector3 vector = brain.mainInterestPoint + new Vector3(0f, this.landingHeight, 0f);
			if (num < 100f && num > 15f)
			{
				Vector3 vector2 = Vector3Ex.Direction2D(closest.transform.position, position);
				RaycastHit raycastHit;
				if (Physics.SphereCast(position, 15f, vector2, out raycastHit, num, 1218511105))
				{
					Vector3 vector3 = Vector3.Cross(vector2, Vector3.up);
					vector = raycastHit.point + vector3 * 50f;
				}
			}
			ch47HelicopterAIController.SetMoveTarget(vector);
			if (flag3)
			{
				if (this.landedForSeconds > 1f && Time.time > this.nextDismountTime)
				{
					foreach (BaseVehicle.MountPointInfo mountPointInfo in ch47HelicopterAIController.mountPoints)
					{
						if (mountPointInfo.mountable && mountPointInfo.mountable.AnyMounted())
						{
							this.nextDismountTime = Time.time + 0.5f;
							mountPointInfo.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (this.landedForSeconds > 8f)
				{
					brain.GetComponent<CH47AIBrain>().ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x0600505D RID: 20573 RVA: 0x001A8DF4 File Offset: 0x001A6FF4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			brain.mainInterestPoint = (entity as CH47HelicopterAIController).landingTarget;
			this.landingHeight = 15f;
			base.StateEnter(brain, entity);
		}

		// Token: 0x0600505E RID: 20574 RVA: 0x001A8E1A File Offset: 0x001A701A
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.SetAltitudeProtection(true);
			ch47HelicopterAIController.SetMinHoverHeight(30f);
			this.landedForSeconds = 0f;
			base.StateLeave(brain, entity);
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x040046C2 RID: 18114
		private float landedForSeconds;

		// Token: 0x040046C3 RID: 18115
		private float lastLandtime;

		// Token: 0x040046C4 RID: 18116
		private float landingHeight = 20f;

		// Token: 0x040046C5 RID: 18117
		private float nextDismountTime;
	}

	// Token: 0x02000D1B RID: 3355
	public class OrbitState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06005060 RID: 20576 RVA: 0x001A8E4D File Offset: 0x001A704D
		public OrbitState()
			: base(AIState.Orbit)
		{
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x001A8E57 File Offset: 0x001A7057
		public Vector3 GetOrbitCenter()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x001A8E64 File Offset: 0x001A7064
		public override float GetWeight()
		{
			if (base.IsInState())
			{
				float num = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState);
				return 5f * num;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Patrol)
			{
				CH47AIBrain.PatrolState patrolState = this.brain.CurrentState as CH47AIBrain.PatrolState;
				if (patrolState != null && patrolState.AtPatrolDestination())
				{
					return 5f;
				}
			}
			return 0f;
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x001A8EE4 File Offset: 0x001A70E4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(true);
			ch47HelicopterAIController.InitiateAnger();
			base.StateEnter(brain, entity);
		}

		// Token: 0x06005064 RID: 20580 RVA: 0x001A8F00 File Offset: 0x001A7100
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			Vector3 orbitCenter = this.GetOrbitCenter();
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.GetPosition();
			Vector3 vector = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
			float num = ((Vector3.Dot(Vector3.Cross(ch47HelicopterAIController.transform.right, Vector3.up), vector2) < 0f) ? (-1f) : 1f);
			float num2 = 75f;
			Vector3 normalized = (-vector + vector2 * num * 0.6f).normalized;
			Vector3 vector3 = orbitCenter + normalized * num2;
			ch47HelicopterAIController.SetMoveTarget(vector3);
			ch47HelicopterAIController.SetAimDirection(Vector3Ex.Direction2D(vector3, position));
			base.StateThink(delta, brain, entity);
			return StateStatus.Running;
		}

		// Token: 0x06005065 RID: 20581 RVA: 0x001A8FC5 File Offset: 0x001A71C5
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.CancelAnger();
			base.StateLeave(brain, entity);
		}
	}

	// Token: 0x02000D1C RID: 3356
	public class PatrolState : BaseAIBrain.BasePatrolState
	{
		// Token: 0x06005066 RID: 20582 RVA: 0x001A8FE1 File Offset: 0x001A71E1
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		// Token: 0x06005067 RID: 20583 RVA: 0x001A8FFC File Offset: 0x001A71FC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			(entity as CH47HelicopterAIController).SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x001A901A File Offset: 0x001A721A
		public bool AtPatrolDestination()
		{
			return Vector3Ex.Distance2D(this.GetDestination(), this.brain.transform.position) < this.patrolApproachDist;
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x001A8E57 File Offset: 0x001A7057
		public Vector3 GetDestination()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x0600506A RID: 20586 RVA: 0x001A903F File Offset: 0x001A723F
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && this.AtPatrolDestination();
		}

		// Token: 0x0600506B RID: 20587 RVA: 0x001A9054 File Offset: 0x001A7254
		public override float GetWeight()
		{
			if (!base.IsInState())
			{
				float num = Mathf.InverseLerp(70f, 120f, base.TimeSinceState()) * 5f;
				return 1f + num;
			}
			if (this.AtPatrolDestination() && base.TimeInState > 2f)
			{
				return 0f;
			}
			return 3f;
		}

		// Token: 0x040046C6 RID: 18118
		protected float patrolApproachDist = 75f;
	}
}
