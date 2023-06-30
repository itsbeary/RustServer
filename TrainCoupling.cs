using System;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class TrainCoupling
{
	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06002793 RID: 10131 RVA: 0x000F7503 File Offset: 0x000F5703
	public bool IsCoupled
	{
		get
		{
			return this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06002794 RID: 10132 RVA: 0x000F7516 File Offset: 0x000F5716
	public bool IsUncoupled
	{
		get
		{
			return !this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000F752C File Offset: 0x000F572C
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller)
		: this(owner, isFrontCoupling, controller, null, null, BaseEntity.Flags.Placeholder)
	{
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000F753C File Offset: 0x000F573C
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller, Transform couplingPoint, Transform couplingPivot, BaseEntity.Flags flag)
	{
		this.owner = owner;
		this.isFrontCoupling = isFrontCoupling;
		this.controller = controller;
		this.couplingPoint = couplingPoint;
		this.couplingPivot = couplingPivot;
		this.flag = flag;
		this.isValid = couplingPoint != null;
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06002797 RID: 10135 RVA: 0x000F758A File Offset: 0x000F578A
	// (set) Token: 0x06002798 RID: 10136 RVA: 0x000F7592 File Offset: 0x000F5792
	public TrainCoupling CoupledTo { get; private set; }

	// Token: 0x06002799 RID: 10137 RVA: 0x000F759B File Offset: 0x000F579B
	public bool IsCoupledTo(TrainCar them)
	{
		return this.CoupledTo != null && this.CoupledTo.owner == them;
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000F75B8 File Offset: 0x000F57B8
	public bool IsCoupledTo(TrainCoupling them)
	{
		return this.CoupledTo != null && this.CoupledTo == them;
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000F75D0 File Offset: 0x000F57D0
	public bool TryCouple(TrainCoupling theirCoupling, bool reflect)
	{
		if (!this.isValid)
		{
			return false;
		}
		if (this.CoupledTo == theirCoupling)
		{
			return true;
		}
		if (this.IsCoupled)
		{
			return false;
		}
		if (reflect && !theirCoupling.TryCouple(this, false))
		{
			return false;
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = theirCoupling;
		this.owner.SetFlag(this.flag, true, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000F7640 File Offset: 0x000F5840
	public void Uncouple(bool reflect)
	{
		if (this.IsUncoupled)
		{
			return;
		}
		if (reflect && this.CoupledTo != null)
		{
			this.CoupledTo.Uncouple(false);
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = null;
		this.owner.SetFlag(this.flag, false, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.timeSinceCouplingBlock = 0f;
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000F76AF File Offset: 0x000F58AF
	public TrainCoupling GetOppositeCoupling()
	{
		if (!this.isFrontCoupling)
		{
			return this.controller.frontCoupling;
		}
		return this.controller.rearCoupling;
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000F76D0 File Offset: 0x000F58D0
	public bool TryGetCoupledToID(out NetworkableId id)
	{
		if (this.CoupledTo != null && this.CoupledTo.owner != null && this.CoupledTo.owner.IsValid())
		{
			id = this.CoupledTo.owner.net.ID;
			return true;
		}
		id = default(NetworkableId);
		return false;
	}

	// Token: 0x04002030 RID: 8240
	public readonly TrainCar owner;

	// Token: 0x04002031 RID: 8241
	public readonly bool isFrontCoupling;

	// Token: 0x04002032 RID: 8242
	public readonly TrainCouplingController controller;

	// Token: 0x04002033 RID: 8243
	public readonly Transform couplingPoint;

	// Token: 0x04002034 RID: 8244
	public readonly Transform couplingPivot;

	// Token: 0x04002035 RID: 8245
	public readonly BaseEntity.Flags flag;

	// Token: 0x04002036 RID: 8246
	public readonly bool isValid;

	// Token: 0x04002038 RID: 8248
	public TimeSince timeSinceCouplingBlock;
}
