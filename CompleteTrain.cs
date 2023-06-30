using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class CompleteTrain : IDisposable
{
	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06002743 RID: 10051 RVA: 0x000F5090 File Offset: 0x000F3290
	// (set) Token: 0x06002744 RID: 10052 RVA: 0x000F5098 File Offset: 0x000F3298
	public TrainCar PrimaryTrainCar { get; private set; }

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06002745 RID: 10053 RVA: 0x000F50A1 File Offset: 0x000F32A1
	public bool TrainIsReversing
	{
		get
		{
			return this.PrimaryTrainCar != this.trainCars[0];
		}
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06002746 RID: 10054 RVA: 0x000F50BA File Offset: 0x000F32BA
	// (set) Token: 0x06002747 RID: 10055 RVA: 0x000F50C2 File Offset: 0x000F32C2
	public float TotalForces { get; private set; }

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06002748 RID: 10056 RVA: 0x000F50CB File Offset: 0x000F32CB
	// (set) Token: 0x06002749 RID: 10057 RVA: 0x000F50D3 File Offset: 0x000F32D3
	public float TotalMass { get; private set; }

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x0600274A RID: 10058 RVA: 0x000F50DC File Offset: 0x000F32DC
	public int NumTrainCars
	{
		get
		{
			return this.trainCars.Count;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x0600274B RID: 10059 RVA: 0x000F50E9 File Offset: 0x000F32E9
	// (set) Token: 0x0600274C RID: 10060 RVA: 0x000F50F1 File Offset: 0x000F32F1
	public int LinedUpToUnload { get; private set; } = -1;

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x0600274D RID: 10061 RVA: 0x000F50FA File Offset: 0x000F32FA
	public bool IsLinedUpToUnload
	{
		get
		{
			return this.LinedUpToUnload >= 0;
		}
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000F5108 File Offset: 0x000F3308
	public CompleteTrain(TrainCar trainCar)
	{
		List<TrainCar> list = Facepunch.Pool.GetList<TrainCar>();
		list.Add(trainCar);
		this.Init(list);
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000F5178 File Offset: 0x000F3378
	public CompleteTrain(List<TrainCar> allTrainCars)
	{
		this.Init(allTrainCars);
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000F51DC File Offset: 0x000F33DC
	private void Init(List<TrainCar> allTrainCars)
	{
		this.trainCars = allTrainCars;
		this.timeSinceLastChange = 0f;
		this.lastMovingTime = UnityEngine.Time.time;
		float num = 0f;
		this.PrimaryTrainCar = this.trainCars[0];
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.completeTrain != this)
			{
				if (trainCar.completeTrain != null)
				{
					bool flag = this.IsCoupledBackwards(i);
					bool preChangeCoupledBackwards = trainCar.coupling.PreChangeCoupledBackwards;
					float preChangeTrackSpeed = trainCar.coupling.PreChangeTrackSpeed;
					if (flag != preChangeCoupledBackwards)
					{
						num -= preChangeTrackSpeed;
					}
					else
					{
						num += preChangeTrackSpeed;
					}
				}
				trainCar.SetNewCompleteTrain(this);
			}
		}
		this.trackSpeed = num / (float)this.trainCars.Count;
		this.prevTrackSpeed = this.trackSpeed;
		this.ParamsTick();
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000F52B8 File Offset: 0x000F34B8
	~CompleteTrain()
	{
		this.Cleanup();
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000F52E4 File Offset: 0x000F34E4
	public void Dispose()
	{
		this.Cleanup();
		System.GC.SuppressFinalize(this);
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000F52F2 File Offset: 0x000F34F2
	private void Cleanup()
	{
		if (this.disposed)
		{
			return;
		}
		this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		this.disposed = true;
		Facepunch.Pool.FreeList<TrainCar>(ref this.trainCars);
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000F5318 File Offset: 0x000F3518
	public void RemoveTrainCar(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return;
		}
		if (this.trainCars.Count <= 1)
		{
			Debug.LogWarning(base.GetType().Name + ": Can't remove car from CompleteTrain of length one.");
			return;
		}
		int num = this.IndexOf(trainCar);
		bool flag;
		if (num == 0)
		{
			flag = this.IsCoupledBackwards(1);
		}
		else
		{
			flag = this.IsCoupledBackwards(0);
		}
		this.trainCars.RemoveAt(num);
		this.timeSinceLastChange = 0f;
		this.LinedUpToUnload = -1;
		if (this.IsCoupledBackwards(0) != flag)
		{
			this.trackSpeed *= -1f;
		}
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000F53B4 File Offset: 0x000F35B4
	public float GetTrackSpeedFor(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return 0f;
		}
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.trackSpeed;
		}
		return this.trackSpeed;
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000F5418 File Offset: 0x000F3618
	public float GetPrevTrackSpeedFor(TrainCar trainCar)
	{
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.prevTrackSpeed;
		}
		return this.prevTrackSpeed;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000F546C File Offset: 0x000F366C
	public void UpdateTick(float dt)
	{
		if (this.ranUpdateTick || this.disposed)
		{
			return;
		}
		this.ranUpdateTick = true;
		if (this.IsAllAsleep() && !this.HasAnyEnginesOn() && !this.HasAnyCollisions() && !this.isShunting)
		{
			this.trackSpeed = 0f;
			return;
		}
		this.ParamsTick();
		this.MovementTick(dt);
		this.LinedUpToUnload = this.CheckLinedUpToUnload(out this.unloaderPos);
		if (this.disposed)
		{
			return;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			this.lastMovingTime = UnityEngine.Time.time;
		}
		if (!this.HasAnyEnginesOn() && !this.HasAnyCollisions() && UnityEngine.Time.time > this.lastMovingTime + 10f)
		{
			this.trackSpeed = 0f;
			this.SleepAll();
		}
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000F553C File Offset: 0x000F373C
	public bool IncludesAnEngine()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == TrainCar.TrainCarType.Engine)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000F55A0 File Offset: 0x000F37A0
	protected bool HasAnyCollisions()
	{
		return this.frontCollisionTrigger.HasAnyContents || this.rearCollisionTrigger.HasAnyContents;
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000F55BC File Offset: 0x000F37BC
	private bool HasAnyEnginesOn()
	{
		if (this.disposed)
		{
			return false;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.CarType == TrainCar.TrainCarType.Engine && trainCar.IsOn())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000F562C File Offset: 0x000F382C
	private bool IsAllAsleep()
	{
		if (this.disposed)
		{
			return true;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.rigidBody.IsSleeping())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000F5694 File Offset: 0x000F3894
	private void SleepAll()
	{
		if (this.disposed)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			trainCar.rigidBody.Sleep();
		}
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000F56F4 File Offset: 0x000F38F4
	public bool TryShuntCarTo(Vector3 shuntDirection, float shuntDistance, TrainCar shuntTarget, Action<CoalingTower.ActionAttemptStatus> shuntEndCallback, out CoalingTower.ActionAttemptStatus status)
	{
		if (this.disposed)
		{
			status = CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (this.isShunting)
		{
			status = CoalingTower.ActionAttemptStatus.AlreadyShunting;
			return false;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			status = CoalingTower.ActionAttemptStatus.TrainIsMoving;
			return false;
		}
		if (this.HasThrottleInput())
		{
			status = CoalingTower.ActionAttemptStatus.TrainHasThrottle;
			return false;
		}
		this.shuntDirection = shuntDirection;
		this.shuntDistance = shuntDistance;
		this.shuntTarget = shuntTarget;
		this.timeSinceShuntStart = 0f;
		this.shuntStartPos2D.x = shuntTarget.transform.position.x;
		this.shuntStartPos2D.y = shuntTarget.transform.position.z;
		this.isShunting = true;
		this.shuntEndCallback = shuntEndCallback;
		status = CoalingTower.ActionAttemptStatus.NoError;
		return true;
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000F57B2 File Offset: 0x000F39B2
	private void EndShunting(CoalingTower.ActionAttemptStatus status)
	{
		this.isShunting = false;
		if (this.shuntEndCallback != null)
		{
			this.shuntEndCallback(status);
			this.shuntEndCallback = null;
		}
		this.shuntTarget = null;
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000F57DD File Offset: 0x000F39DD
	public bool ContainsOnly(TrainCar trainCar)
	{
		return !this.disposed && this.trainCars.Count == 1 && this.trainCars[0] == trainCar;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000F580B File Offset: 0x000F3A0B
	public int IndexOf(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return -1;
		}
		return this.trainCars.IndexOf(trainCar);
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000F5824 File Offset: 0x000F3A24
	public bool TryGetAdjacentTrainCar(TrainCar trainCar, bool next, Vector3 forwardDir, out TrainCar result)
	{
		int num = this.trainCars.IndexOf(trainCar);
		Vector3 vector;
		if (this.IsCoupledBackwards(num))
		{
			vector = -trainCar.transform.forward;
		}
		else
		{
			vector = trainCar.transform.forward;
		}
		if (Vector3.Dot(vector, forwardDir) < 0f)
		{
			next = !next;
		}
		if (num >= 0)
		{
			if (next)
			{
				num++;
			}
			else
			{
				num--;
			}
			if (num >= 0 && num < this.trainCars.Count)
			{
				result = this.trainCars[num];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000F58B4 File Offset: 0x000F3AB4
	private void ParamsTick()
	{
		this.TotalForces = 0f;
		this.TotalMass = 0f;
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.rigidBody.mass > num2)
			{
				num2 = trainCar.rigidBody.mass;
				num = i;
			}
		}
		bool flag = false;
		for (int j = 0; j < this.trainCars.Count; j++)
		{
			TrainCar trainCar2 = this.trainCars[j];
			float forces = trainCar2.GetForces();
			this.TotalForces += (this.IsCoupledBackwards(trainCar2) ? (-forces) : forces);
			flag |= trainCar2.HasThrottleInput();
			if (j == num)
			{
				this.TotalMass += trainCar2.rigidBody.mass;
			}
			else
			{
				this.TotalMass += trainCar2.rigidBody.mass * 0.4f;
			}
		}
		if (this.isShunting && flag)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.TrainHasThrottle);
		}
		if (this.trainCars.Count == 1)
		{
			this.frontCollisionTrigger = this.trainCars[0].FrontCollisionTrigger;
			this.rearCollisionTrigger = this.trainCars[0].RearCollisionTrigger;
			return;
		}
		this.frontCollisionTrigger = (this.trainCars[0].coupling.IsRearCoupled ? this.trainCars[0].FrontCollisionTrigger : this.trainCars[0].RearCollisionTrigger);
		this.rearCollisionTrigger = (this.trainCars[this.trainCars.Count - 1].coupling.IsRearCoupled ? this.trainCars[this.trainCars.Count - 1].FrontCollisionTrigger : this.trainCars[this.trainCars.Count - 1].RearCollisionTrigger);
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000F5AB8 File Offset: 0x000F3CB8
	private void MovementTick(float dt)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (!this.isShunting)
		{
			this.trackSpeed += this.TotalForces * dt / this.TotalMass;
		}
		else
		{
			bool flag = Vector3.Dot(this.shuntDirection, this.PrimaryTrainCar.transform.forward) >= 0f;
			if (this.IsCoupledBackwards(this.PrimaryTrainCar))
			{
				flag = !flag;
			}
			if (this.shuntTarget == null || this.shuntTarget.IsDead() || this.shuntTarget.IsDestroyed)
			{
				this.EndShunting(CoalingTower.ActionAttemptStatus.NoTrainCar);
			}
			else
			{
				float num = 4f;
				this.shuntTargetPos2D.x = this.shuntTarget.transform.position.x;
				this.shuntTargetPos2D.y = this.shuntTarget.transform.position.z;
				float num2 = this.shuntDistance - Vector3.Distance(this.shuntStartPos2D, this.shuntTargetPos2D);
				if (num2 < 2f)
				{
					float num3 = Mathf.InverseLerp(0f, 2f, num2);
					num *= Mathf.Lerp(0.1f, 1f, num3);
				}
				this.trackSpeed = Mathf.MoveTowards(this.trackSpeed, flag ? num : (-num), dt * 10f);
				if (this.timeSinceShuntStart > 20f || num2 <= 0f)
				{
					this.EndShunting(CoalingTower.ActionAttemptStatus.NoError);
					this.trackSpeed = 0f;
				}
			}
		}
		float num4 = this.trainCars[0].rigidBody.drag;
		if (this.IsLinedUpToUnload)
		{
			float num5 = Mathf.Abs(this.trackSpeed);
			if (num5 > 1f)
			{
				TrainCarUnloadable trainCarUnloadable = this.trainCars[this.LinedUpToUnload] as TrainCarUnloadable;
				if (trainCarUnloadable != null)
				{
					float num6 = trainCarUnloadable.MinDistToUnloadingArea(this.unloaderPos);
					float num7 = Mathf.InverseLerp(2f, 0f, num6);
					if (num5 < 2f)
					{
						float num8 = (num5 - 1f) / 1f;
						num7 *= num8;
					}
					num4 = Mathf.Lerp(num4, 3.5f, num7);
				}
			}
		}
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed -= num4 * 4f * dt;
			if (this.trackSpeed < 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		else if (this.trackSpeed < 0f)
		{
			this.trackSpeed += num4 * 4f * dt;
			if (this.trackSpeed > 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		float num9 = this.trackSpeed;
		this.trackSpeed = this.ApplyCollisionsToTrackSpeed(this.trackSpeed, this.TotalMass, dt);
		if (this.isShunting && this.trackSpeed != num9)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		}
		if (this.disposed)
		{
			return;
		}
		this.trackSpeed = Mathf.Clamp(this.trackSpeed, -(TrainCar.TRAINCAR_MAX_SPEED - 1f), TrainCar.TRAINCAR_MAX_SPEED - 1f);
		if (this.trackSpeed > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.trackSpeed < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else if (this.TotalForces > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.TotalForces < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		if (this.trackSpeed != 0f || this.TotalForces != 0f)
		{
			this.PrimaryTrainCar.FrontTrainCarTick(this.GetTrackSelection(), dt);
			if (this.trainCars.Count > 1)
			{
				if (this.PrimaryTrainCar == this.trainCars[0])
				{
					for (int i = 1; i < this.trainCars.Count; i++)
					{
						this.MoveOtherTrainCar(this.trainCars[i], this.trainCars[i - 1]);
					}
					return;
				}
				for (int j = this.trainCars.Count - 2; j >= 0; j--)
				{
					this.MoveOtherTrainCar(this.trainCars[j], this.trainCars[j + 1]);
				}
			}
		}
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000F5F60 File Offset: 0x000F4160
	private void MoveOtherTrainCar(TrainCar trainCar, TrainCar prevTrainCar)
	{
		TrainTrackSpline frontTrackSection = prevTrainCar.FrontTrackSection;
		float frontWheelSplineDist = prevTrainCar.FrontWheelSplineDist;
		float num = 0f;
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		TrainCoupling coupledTo2 = trainCar.coupling.rearCoupling.CoupledTo;
		if (coupledTo == prevTrainCar.coupling.frontCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.rearCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo == prevTrainCar.coupling.rearCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.frontCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Uncoupled!");
		}
		trainCar.OtherTrainCarTick(frontTrackSection, frontWheelSplineDist, -num);
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000F6055 File Offset: 0x000F4255
	public void ResetUpdateTick()
	{
		this.ranUpdateTick = false;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000F6060 File Offset: 0x000F4260
	public bool Matches(List<TrainCar> listToCompare)
	{
		if (this.disposed)
		{
			return false;
		}
		if (listToCompare.Count != this.trainCars.Count)
		{
			return false;
		}
		for (int i = 0; i < listToCompare.Count; i++)
		{
			if (this.trainCars[i] != listToCompare[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000F60BC File Offset: 0x000F42BC
	public void ReduceSpeedBy(float velChange)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed = Mathf.Max(0f, this.trackSpeed - velChange);
			return;
		}
		if (this.trackSpeed < 0f)
		{
			this.trackSpeed = Mathf.Min(0f, this.trackSpeed + velChange);
		}
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000F6120 File Offset: 0x000F4320
	public bool AnyPlayersOnTrain()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.AnyPlayersOnTrainCar())
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000F6184 File Offset: 0x000F4384
	private int CheckLinedUpToUnload(out Vector3 unloaderPos)
	{
		if (this.disposed)
		{
			unloaderPos = Vector3.zero;
			return -1;
		}
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			bool flag;
			if (CoalingTower.IsUnderAnUnloader(trainCar, out flag, out unloaderPos))
			{
				trainCar.SetFlag(BaseEntity.Flags.Reserved4, flag, false, true);
				if (flag)
				{
					return i;
				}
			}
		}
		unloaderPos = Vector3.zero;
		return -1;
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000F61F3 File Offset: 0x000F43F3
	public bool IsCoupledBackwards(TrainCar trainCar)
	{
		return !this.disposed && this.IsCoupledBackwards(this.trainCars.IndexOf(trainCar));
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000F6214 File Offset: 0x000F4414
	private bool IsCoupledBackwards(int trainCarIndex)
	{
		if (this.disposed || this.trainCars.Count == 1 || trainCarIndex < 0 || trainCarIndex > this.trainCars.Count - 1)
		{
			return false;
		}
		TrainCar trainCar = this.trainCars[trainCarIndex];
		if (trainCarIndex == 0)
		{
			return trainCar.coupling.IsFrontCoupled;
		}
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		return coupledTo == null || coupledTo.owner != this.trainCars[trainCarIndex - 1];
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000F6298 File Offset: 0x000F4498
	private bool HasThrottleInput()
	{
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			if (this.trainCars[i].HasThrottleInput())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000F62D4 File Offset: 0x000F44D4
	private TrainTrackSpline.TrackSelection GetTrackSelection()
	{
		TrainTrackSpline.TrackSelection trackSelection = TrainTrackSpline.TrackSelection.Default;
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.localTrackSelection != TrainTrackSpline.TrackSelection.Default)
			{
				if (this.IsCoupledBackwards(trainCar) != this.IsCoupledBackwards(this.PrimaryTrainCar))
				{
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Left)
					{
						return TrainTrackSpline.TrackSelection.Right;
					}
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Right)
					{
						return TrainTrackSpline.TrackSelection.Left;
					}
				}
				return trainCar.localTrackSelection;
			}
		}
		return trackSelection;
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000F6368 File Offset: 0x000F4568
	public void FreeStaticCollision()
	{
		this.staticCollidingAtFront = CompleteTrain.StaticCollisionState.Free;
		this.staticCollidingAtRear = CompleteTrain.StaticCollisionState.Free;
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000F6378 File Offset: 0x000F4578
	private float ApplyCollisionsToTrackSpeed(float trackSpeed, float totalMass, float deltaTime)
	{
		TrainCar trainCar = this.frontCollisionTrigger.owner;
		Vector3 vector = (this.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward);
		trackSpeed = this.ApplyCollisions(trackSpeed, trainCar, vector, true, this.frontCollisionTrigger, totalMass, ref this.staticCollidingAtFront, this.staticCollidingAtRear, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		trainCar = this.rearCollisionTrigger.owner;
		vector = (this.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward);
		trackSpeed = this.ApplyCollisions(trackSpeed, trainCar, vector, false, this.rearCollisionTrigger, totalMass, ref this.staticCollidingAtRear, this.staticCollidingAtFront, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		Rigidbody rigidbody = null;
		foreach (KeyValuePair<Rigidbody, float> keyValuePair in this.prevTrackSpeeds)
		{
			if (keyValuePair.Key == null || (!this.frontCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key) && !this.rearCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key)))
			{
				rigidbody = keyValuePair.Key;
				break;
			}
		}
		if (rigidbody != null)
		{
			this.prevTrackSpeeds.Remove(rigidbody);
		}
		return trackSpeed;
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000F64E4 File Offset: 0x000F46E4
	private float ApplyCollisions(float trackSpeed, TrainCar ourTrainCar, Vector3 forwardVector, bool atOurFront, TriggerTrainCollisions trigger, float ourTotalMass, ref CompleteTrain.StaticCollisionState wasStaticColliding, CompleteTrain.StaticCollisionState otherEndStaticColliding, float deltaTime)
	{
		Vector3 vector = forwardVector * trackSpeed;
		bool flag = trigger.HasAnyStaticContents;
		if (atOurFront && ourTrainCar.FrontAtEndOfLine)
		{
			flag = true;
		}
		else if (!atOurFront && ourTrainCar.RearAtEndOfLine)
		{
			flag = true;
		}
		float num = (flag ? (vector.magnitude * Mathf.Clamp(ourTotalMass, 1f, 13000f)) : 0f);
		trackSpeed = this.HandleStaticCollisions(flag, atOurFront, trackSpeed, ref wasStaticColliding, trigger);
		if (!flag && otherEndStaticColliding == CompleteTrain.StaticCollisionState.Free)
		{
			foreach (TrainCar trainCar in trigger.trainContents)
			{
				Vector3 vector2 = trainCar.transform.forward * trainCar.GetPrevTrackSpeed();
				trackSpeed = this.HandleTrainCollision(atOurFront, forwardVector, trackSpeed, ourTrainCar.transform, trainCar, deltaTime, ref wasStaticColliding);
				num += Vector3.Magnitude(vector2 - vector) * Mathf.Clamp(trainCar.rigidBody.mass, 1f, 13000f);
			}
			foreach (Rigidbody rigidbody in trigger.otherRigidbodyContents)
			{
				trackSpeed = this.HandleRigidbodyCollision(atOurFront, trackSpeed, forwardVector, ourTotalMass, rigidbody, rigidbody.mass, deltaTime, true);
				num += Vector3.Magnitude(rigidbody.velocity - vector) * Mathf.Clamp(rigidbody.mass, 1f, 13000f);
			}
		}
		if (num >= 70000f && this.timeSinceLastChange > 1f && trigger.owner.ApplyCollisionDamage(num) > 5f)
		{
			foreach (Collider collider in trigger.colliderContents)
			{
				Vector3 vector3 = collider.ClosestPointOnBounds(trigger.owner.transform.position);
				trigger.owner.TryShowCollisionFX(vector3, trigger.owner.collisionEffect);
			}
		}
		return trackSpeed;
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000F6724 File Offset: 0x000F4924
	private float HandleStaticCollisions(bool staticColliding, bool front, float trackSpeed, ref CompleteTrain.StaticCollisionState wasStaticColliding, TriggerTrainCollisions trigger = null)
	{
		float num = (front ? (-5f) : 5f);
		if (staticColliding && (front ? (trackSpeed > num) : (trackSpeed < num)))
		{
			trackSpeed = num;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StaticColliding;
			HashSet<GameObject> hashSet = (front ? this.monitoredStaticContentF : this.monitoredStaticContentR);
			hashSet.Clear();
			if (!(trigger != null))
			{
				return trackSpeed;
			}
			using (HashSet<GameObject>.Enumerator enumerator = trigger.staticContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject gameObject = enumerator.Current;
					hashSet.Add(gameObject);
				}
				return trackSpeed;
			}
		}
		if (wasStaticColliding == CompleteTrain.StaticCollisionState.StaticColliding)
		{
			trackSpeed = 0f;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StayingStill;
		}
		else if (wasStaticColliding == CompleteTrain.StaticCollisionState.StayingStill)
		{
			bool flag = (front ? (trackSpeed > 0.01f) : (trackSpeed < -0.01f));
			bool flag2 = false;
			if (!flag)
			{
				flag2 = (front ? (trackSpeed < -0.01f) : (trackSpeed > 0.01f));
			}
			if (flag)
			{
				HashSet<GameObject> hashSet2 = (front ? this.monitoredStaticContentF : this.monitoredStaticContentR);
				if (hashSet2.Count > 0)
				{
					bool flag3 = true;
					using (HashSet<GameObject>.Enumerator enumerator = hashSet2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current != null)
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				trackSpeed = 0f;
			}
			else if (flag2)
			{
				wasStaticColliding = CompleteTrain.StaticCollisionState.Free;
			}
		}
		else if (front)
		{
			this.monitoredStaticContentF.Clear();
		}
		else
		{
			this.monitoredStaticContentR.Clear();
		}
		return trackSpeed;
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000F68C4 File Offset: 0x000F4AC4
	private float HandleTrainCollision(bool front, Vector3 forwardVector, float trackSpeed, Transform ourTransform, TrainCar theirTrain, float deltaTime, ref CompleteTrain.StaticCollisionState wasStaticColliding)
	{
		Vector3 vector = (front ? forwardVector : (-forwardVector));
		float num = Vector3.Angle(vector, theirTrain.transform.forward);
		float num2 = Vector3.Dot(vector, (theirTrain.transform.position - ourTransform.position).normalized);
		if ((num > 30f && num < 150f) || Mathf.Abs(num2) < 0.975f)
		{
			trackSpeed = (front ? (-0.5f) : 0.5f);
		}
		else
		{
			List<CompleteTrain> list = Facepunch.Pool.GetList<CompleteTrain>();
			float totalPushingMass = this.GetTotalPushingMass(vector, forwardVector, ref list);
			if (totalPushingMass < 0f)
			{
				trackSpeed = this.HandleStaticCollisions(true, front, trackSpeed, ref wasStaticColliding, null);
			}
			else
			{
				trackSpeed = this.HandleRigidbodyCollision(front, trackSpeed, forwardVector, this.TotalMass, theirTrain.rigidBody, totalPushingMass, deltaTime, false);
			}
			list.Clear();
			float num3 = this.GetTotalPushingForces(vector, forwardVector, ref list);
			if (!front)
			{
				num3 *= -1f;
			}
			if ((front && num3 <= 0f) || (!front && num3 >= 0f))
			{
				trackSpeed += num3 / this.TotalMass * deltaTime;
			}
			Facepunch.Pool.FreeList<CompleteTrain>(ref list);
		}
		return trackSpeed;
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000F69EC File Offset: 0x000F4BEC
	private float HandleRigidbodyCollision(bool atOurFront, float trackSpeed, Vector3 forwardVector, float ourTotalMass, Rigidbody theirRB, float theirTotalMass, float deltaTime, bool calcSecondaryForces)
	{
		float num = Vector3.Dot(forwardVector, theirRB.velocity);
		float num2 = trackSpeed - num;
		if ((atOurFront && num2 <= 0f) || (!atOurFront && num2 >= 0f))
		{
			return trackSpeed;
		}
		float num3 = num2 / deltaTime * theirTotalMass * 0.75f;
		if (calcSecondaryForces)
		{
			if (this.prevTrackSpeeds.ContainsKey(theirRB))
			{
				float num4 = num2 / deltaTime * ourTotalMass * 0.75f / theirTotalMass * deltaTime;
				float num5 = this.prevTrackSpeeds[theirRB] - num;
				num3 -= Mathf.Clamp((num5 - num4) * ourTotalMass, 0f, 1000000f);
				this.prevTrackSpeeds[theirRB] = num;
			}
			else if (num != 0f)
			{
				this.prevTrackSpeeds.Add(theirRB, num);
			}
		}
		float num6 = num3 / ourTotalMass * deltaTime;
		num6 = Mathf.Clamp(num6, -Mathf.Abs(num - trackSpeed) - 0.5f, Mathf.Abs(num - trackSpeed) + 0.5f);
		trackSpeed -= num6;
		return trackSpeed;
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000F6ADC File Offset: 0x000F4CDC
	private float GetTotalPushingMass(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingMass: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalMass;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		if ((flag ? this.staticCollidingAtFront : this.staticCollidingAtRear) != CompleteTrain.StaticCollisionState.Free)
		{
			return -1f;
		}
		TriggerTrainCollisions triggerTrainCollisions = (flag ? this.frontCollisionTrigger : this.rearCollisionTrigger);
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 vector = (trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward);
				float totalPushingMass = trainCar.completeTrain.GetTotalPushingMass(pushDirection, vector, ref prevTrains);
				if (totalPushingMass < 0f)
				{
					return -1f;
				}
				num += totalPushingMass;
			}
		}
		foreach (Rigidbody rigidbody in triggerTrainCollisions.otherRigidbodyContents)
		{
			num += rigidbody.mass;
		}
		return num;
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000F6C60 File Offset: 0x000F4E60
	private float GetTotalPushingForces(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingForces: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalForces;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		TriggerTrainCollisions triggerTrainCollisions = (flag ? this.frontCollisionTrigger : this.rearCollisionTrigger);
		if (!flag)
		{
			num *= -1f;
		}
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 vector = (trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward);
				num += trainCar.completeTrain.GetTotalPushingForces(pushDirection, vector, ref prevTrains);
			}
		}
		return num;
	}

	// Token: 0x04001FD7 RID: 8151
	private Vector3 unloaderPos;

	// Token: 0x04001FD8 RID: 8152
	private float trackSpeed;

	// Token: 0x04001FD9 RID: 8153
	private float prevTrackSpeed;

	// Token: 0x04001FDA RID: 8154
	private List<TrainCar> trainCars;

	// Token: 0x04001FDB RID: 8155
	private TriggerTrainCollisions frontCollisionTrigger;

	// Token: 0x04001FDC RID: 8156
	private TriggerTrainCollisions rearCollisionTrigger;

	// Token: 0x04001FDD RID: 8157
	private bool ranUpdateTick;

	// Token: 0x04001FDE RID: 8158
	private bool disposed;

	// Token: 0x04001FDF RID: 8159
	private const float IMPACT_ENERGY_FRACTION = 0.75f;

	// Token: 0x04001FE0 RID: 8160
	private const float MIN_COLLISION_FORCE = 70000f;

	// Token: 0x04001FE1 RID: 8161
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001FE2 RID: 8162
	private const float SLEEP_SPEED = 0.1f;

	// Token: 0x04001FE3 RID: 8163
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001FE4 RID: 8164
	private TimeSince timeSinceLastChange;

	// Token: 0x04001FE5 RID: 8165
	private bool isShunting;

	// Token: 0x04001FE6 RID: 8166
	private TimeSince timeSinceShuntStart;

	// Token: 0x04001FE7 RID: 8167
	private const float MAX_SHUNT_TIME = 20f;

	// Token: 0x04001FE8 RID: 8168
	private const float SHUNT_SPEED = 4f;

	// Token: 0x04001FE9 RID: 8169
	private const float SHUNT_SPEED_CHANGE_RATE = 10f;

	// Token: 0x04001FEA RID: 8170
	private Action<CoalingTower.ActionAttemptStatus> shuntEndCallback;

	// Token: 0x04001FEB RID: 8171
	private float shuntDistance;

	// Token: 0x04001FEC RID: 8172
	private Vector3 shuntDirection;

	// Token: 0x04001FED RID: 8173
	private Vector2 shuntStartPos2D = Vector2.zero;

	// Token: 0x04001FEE RID: 8174
	private Vector2 shuntTargetPos2D = Vector2.zero;

	// Token: 0x04001FEF RID: 8175
	private TrainCar shuntTarget;

	// Token: 0x04001FF0 RID: 8176
	private CompleteTrain.StaticCollisionState staticCollidingAtFront;

	// Token: 0x04001FF1 RID: 8177
	private HashSet<GameObject> monitoredStaticContentF = new HashSet<GameObject>();

	// Token: 0x04001FF2 RID: 8178
	private CompleteTrain.StaticCollisionState staticCollidingAtRear;

	// Token: 0x04001FF3 RID: 8179
	private HashSet<GameObject> monitoredStaticContentR = new HashSet<GameObject>();

	// Token: 0x04001FF4 RID: 8180
	private Dictionary<Rigidbody, float> prevTrackSpeeds = new Dictionary<Rigidbody, float>();

	// Token: 0x02000D25 RID: 3365
	private enum ShuntState
	{
		// Token: 0x040046E3 RID: 18147
		None,
		// Token: 0x040046E4 RID: 18148
		Forwards,
		// Token: 0x040046E5 RID: 18149
		Backwards
	}

	// Token: 0x02000D26 RID: 3366
	private enum StaticCollisionState
	{
		// Token: 0x040046E7 RID: 18151
		Free,
		// Token: 0x040046E8 RID: 18152
		StaticColliding,
		// Token: 0x040046E9 RID: 18153
		StayingStill
	}
}
