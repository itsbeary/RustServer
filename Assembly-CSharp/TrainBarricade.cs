using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004B6 RID: 1206
public class TrainBarricade : BaseCombatEntity, ITrainCollidable, TrainTrackSpline.ITrainTrackUser
{
	// Token: 0x1700034D RID: 845
	// (get) Token: 0x0600277E RID: 10110 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x0600277F RID: 10111 RVA: 0x000F6EB3 File Offset: 0x000F50B3
	// (set) Token: 0x06002780 RID: 10112 RVA: 0x000F6EBB File Offset: 0x000F50BB
	public float FrontWheelSplineDist { get; private set; }

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06002781 RID: 10113 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
	public TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Other;
		}
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000F6EC4 File Offset: 0x000F50C4
	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		bool flag = false;
		if (base.isServer)
		{
			float num = Mathf.Abs(train.GetTrackSpeed());
			this.SetHitTrain(train, trainTrigger);
			if (num < this.minVelToDestroy && !vehicle.cinematictrains)
			{
				base.InvokeRandomized(new Action(this.PushForceTick), 0f, 0.25f, 0.025f);
			}
			else
			{
				flag = true;
				base.Invoke(new Action(this.DestroyThisBarrier), 0f);
			}
		}
		return flag;
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000F6F3C File Offset: 0x000F513C
	public override void ServerInit()
	{
		base.ServerInit();
		TrainTrackSpline trainTrackSpline;
		float num;
		if (TrainTrackSpline.TryFindTrackNear(base.transform.position, 3f, out trainTrackSpline, out num))
		{
			this.track = trainTrackSpline;
			this.FrontWheelSplineDist = num;
			this.track.RegisterTrackUser(this);
		}
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000F6F84 File Offset: 0x000F5184
	internal override void DoServerDestroy()
	{
		if (this.track != null)
		{
			this.track.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000F6FA6 File Offset: 0x000F51A6
	private void SetHitTrain(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		this.hitTrain = train;
		this.hitTrainTrigger = trainTrigger;
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000F6FB6 File Offset: 0x000F51B6
	private void ClearHitTrain()
	{
		this.SetHitTrain(null, null);
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000F6FC0 File Offset: 0x000F51C0
	private void DestroyThisBarrier()
	{
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		if (this.hitTrain != null)
		{
			this.hitTrain.completeTrain.ReduceSpeedBy(this.velReduction);
			if (vehicle.cinematictrains)
			{
				this.hitTrain.Hurt(9999f, DamageType.Collision, this, false);
			}
			else
			{
				float num = Mathf.Abs(this.hitTrain.GetTrackSpeed()) * this.trainDamagePerMPS;
				this.hitTrain.Hurt(num, DamageType.Collision, this, false);
			}
		}
		this.ClearHitTrain();
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000F7054 File Offset: 0x000F5254
	private void PushForceTick()
	{
		if (this.hitTrain == null || this.hitTrainTrigger == null || this.hitTrain.IsDead() || this.hitTrain.IsDestroyed || this.IsDead())
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
			return;
		}
		bool flag = true;
		if (!this.hitTrainTrigger.triggerCollider.bounds.Intersects(this.bounds))
		{
			Vector3 vector;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Front)
			{
				vector = this.hitTrainTrigger.owner.GetFrontOfTrainPos();
			}
			else
			{
				vector = this.hitTrainTrigger.owner.GetRearOfTrainPos();
			}
			Vector3 vector2 = base.transform.position + this.bounds.ClosestPoint(vector - base.transform.position);
			Debug.DrawRay(vector2, Vector3.up, Color.red, 10f);
			flag = Vector3.SqrMagnitude(vector2 - vector) < 1f;
		}
		if (flag)
		{
			float num = this.hitTrainTrigger.owner.completeTrain.TotalForces;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Rear)
			{
				num *= -1f;
			}
			num = Mathf.Max(0f, num);
			base.Hurt(0.002f * num);
			if (this.IsDead())
			{
				this.hitTrain.completeTrain.FreeStaticCollision();
				return;
			}
		}
		else
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
		}
	}

	// Token: 0x04002005 RID: 8197
	[FormerlySerializedAs("damagePerMPS")]
	[SerializeField]
	private float trainDamagePerMPS = 10f;

	// Token: 0x04002006 RID: 8198
	[SerializeField]
	private float minVelToDestroy = 6f;

	// Token: 0x04002007 RID: 8199
	[SerializeField]
	private float velReduction = 2f;

	// Token: 0x04002008 RID: 8200
	[SerializeField]
	private GameObjectRef barricadeDamageEffect;

	// Token: 0x0400200A RID: 8202
	private TrainCar hitTrain;

	// Token: 0x0400200B RID: 8203
	private TriggerTrainCollisions hitTrainTrigger;

	// Token: 0x0400200C RID: 8204
	private TrainTrackSpline track;
}
