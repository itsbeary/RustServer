using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class TrainCouplingController
{
	// Token: 0x17000353 RID: 851
	// (get) Token: 0x0600279F RID: 10143 RVA: 0x000F772F File Offset: 0x000F592F
	public bool IsCoupled
	{
		get
		{
			return this.IsFrontCoupled || this.IsRearCoupled;
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x060027A0 RID: 10144 RVA: 0x000F7741 File Offset: 0x000F5941
	public bool IsFrontCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x060027A1 RID: 10145 RVA: 0x000F7753 File Offset: 0x000F5953
	public bool IsRearCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved3);
		}
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x060027A2 RID: 10146 RVA: 0x000F7765 File Offset: 0x000F5965
	// (set) Token: 0x060027A3 RID: 10147 RVA: 0x000F776D File Offset: 0x000F596D
	public float PreChangeTrackSpeed { get; private set; }

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x060027A4 RID: 10148 RVA: 0x000F7776 File Offset: 0x000F5976
	// (set) Token: 0x060027A5 RID: 10149 RVA: 0x000F777E File Offset: 0x000F597E
	public bool PreChangeCoupledBackwards { get; private set; }

	// Token: 0x060027A6 RID: 10150 RVA: 0x000F7788 File Offset: 0x000F5988
	public TrainCouplingController(TrainCar owner)
	{
		this.owner = owner;
		this.frontCoupling = new TrainCoupling(owner, true, this, owner.frontCoupling, owner.frontCouplingPivot, BaseEntity.Flags.Reserved2);
		this.rearCoupling = new TrainCoupling(owner, false, this, owner.rearCoupling, owner.rearCouplingPivot, BaseEntity.Flags.Reserved3);
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000F77E0 File Offset: 0x000F59E0
	public bool IsCoupledTo(TrainCar them)
	{
		return this.frontCoupling.IsCoupledTo(them) || this.rearCoupling.IsCoupledTo(them);
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000F7800 File Offset: 0x000F5A00
	public bool TryCouple(TrainCar them, TriggerTrainCollisions.Location ourLocation)
	{
		TrainCoupling trainCoupling = ((ourLocation == TriggerTrainCollisions.Location.Front) ? this.frontCoupling : this.rearCoupling);
		if (!trainCoupling.isValid)
		{
			return false;
		}
		if (trainCoupling.IsCoupled)
		{
			return false;
		}
		if (trainCoupling.timeSinceCouplingBlock < 1.5f)
		{
			return false;
		}
		float num = Vector3.Angle(this.owner.transform.forward, them.transform.forward);
		if (num > 25f && num < 155f)
		{
			return false;
		}
		bool flag = num < 90f;
		TrainCoupling trainCoupling2;
		if (flag)
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.rearCoupling : them.coupling.frontCoupling);
		}
		else
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.frontCoupling : them.coupling.rearCoupling);
		}
		float num2 = them.GetTrackSpeed();
		if (!flag)
		{
			num2 = -num2;
		}
		if (Mathf.Abs(num2 - this.owner.GetTrackSpeed()) > TrainCouplingController.max_couple_speed)
		{
			trainCoupling.timeSinceCouplingBlock = 0f;
			trainCoupling2.timeSinceCouplingBlock = 0f;
			return false;
		}
		if (!trainCoupling2.isValid)
		{
			return false;
		}
		if (Vector3.SqrMagnitude(trainCoupling.couplingPoint.position - trainCoupling2.couplingPoint.position) > 0.5f)
		{
			return false;
		}
		TrainTrackSpline frontTrackSection = this.owner.FrontTrackSection;
		TrainTrackSpline frontTrackSection2 = them.FrontTrackSection;
		return (!(frontTrackSection2 != frontTrackSection) || frontTrackSection.HasConnectedTrack(frontTrackSection2)) && trainCoupling.TryCouple(trainCoupling2, true);
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x000F796E File Offset: 0x000F5B6E
	public void Uncouple(bool front)
	{
		if (front)
		{
			this.frontCoupling.Uncouple(true);
			return;
		}
		this.rearCoupling.Uncouple(true);
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000F798C File Offset: 0x000F5B8C
	public void GetAll(ref List<TrainCar> result)
	{
		result.Add(this.owner);
		TrainCoupling trainCoupling = this.rearCoupling.CoupledTo;
		while (trainCoupling != null && trainCoupling.IsCoupled && !result.Contains(trainCoupling.owner))
		{
			result.Insert(0, trainCoupling.owner);
			trainCoupling = trainCoupling.GetOppositeCoupling();
			trainCoupling = trainCoupling.CoupledTo;
		}
		TrainCoupling trainCoupling2 = this.frontCoupling.CoupledTo;
		while (trainCoupling2 != null && trainCoupling2.IsCoupled && !result.Contains(trainCoupling2.owner))
		{
			result.Add(trainCoupling2.owner);
			trainCoupling2 = trainCoupling2.GetOppositeCoupling();
			trainCoupling2 = trainCoupling2.CoupledTo;
		}
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x000F7A2D File Offset: 0x000F5C2D
	public void OnPreCouplingChange()
	{
		this.PreChangeCoupledBackwards = this.owner.IsCoupledBackwards();
		this.PreChangeTrackSpeed = this.owner.GetTrackSpeed();
		if (this.PreChangeCoupledBackwards)
		{
			this.PreChangeTrackSpeed = -this.PreChangeTrackSpeed;
		}
	}

	// Token: 0x0400203B RID: 8251
	public const BaseEntity.Flags Flag_CouplingFront = BaseEntity.Flags.Reserved2;

	// Token: 0x0400203C RID: 8252
	public const BaseEntity.Flags Flag_CouplingRear = BaseEntity.Flags.Reserved3;

	// Token: 0x0400203D RID: 8253
	public readonly TrainCoupling frontCoupling;

	// Token: 0x0400203E RID: 8254
	public readonly TrainCoupling rearCoupling;

	// Token: 0x0400203F RID: 8255
	private readonly TrainCar owner;

	// Token: 0x04002040 RID: 8256
	[ServerVar(Help = "Maximum difference in velocity for train cars to couple")]
	public static float max_couple_speed = 9f;
}
