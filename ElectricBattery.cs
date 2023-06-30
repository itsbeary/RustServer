using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class ElectricBattery : global::IOEntity, IInstanceDataReceiver
{
	// Token: 0x0600164C RID: 5708 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x000ADCE8 File Offset: 0x000ABEE8
	public override int MaximalPowerOutput()
	{
		return this.maxOutput;
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x000ADCF0 File Offset: 0x000ABEF0
	public int GetActiveDrain()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.activeDrain;
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x000ADD02 File Offset: 0x000ABF02
	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		this.rustWattSeconds = (float)data.dataInt;
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x000ADD11 File Offset: 0x000ABF11
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.wasLoaded = true;
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x000ADD20 File Offset: 0x000ABF20
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem.instanceData == null)
		{
			createdItem.instanceData = new ProtoBuf.Item.InstanceData();
		}
		createdItem.instanceData.ShouldPool = false;
		createdItem.instanceData.dataInt = Mathf.FloorToInt(this.rustWattSeconds);
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x000ADD5F File Offset: 0x000ABF5F
	public override int GetCurrentEnergy()
	{
		return this.currentEnergy;
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x000ADD67 File Offset: 0x000ABF67
	public override int DesiredPower()
	{
		if (this.rustWattSeconds >= this.maxCapactiySeconds)
		{
			return 0;
		}
		return Mathf.FloorToInt((float)this.maxOutput * this.maximumInboundEnergyRatio);
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x000ADD8C File Offset: 0x000ABF8C
	public override void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = base.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, this.rustWattSeconds, (float)this.activeDrain);
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x000ADDC3 File Offset: 0x000ABFC3
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.CheckDischarge), UnityEngine.Random.Range(0f, 1f), 1f, 0.1f);
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x00007A44 File Offset: 0x00005C44
	public int GetDrainFor(global::IOEntity ent)
	{
		return 0;
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x000ADDF8 File Offset: 0x000ABFF8
	public void AddConnectedRecursive(global::IOEntity root, ref HashSet<global::IOEntity> listToUse)
	{
		listToUse.Add(root);
		if (root.WantsPassthroughPower())
		{
			for (int i = 0; i < root.outputs.Length; i++)
			{
				if (root.AllowDrainFrom(i))
				{
					global::IOEntity.IOSlot ioslot = root.outputs[i];
					if (ioslot.type == global::IOEntity.IOType.Electric)
					{
						global::IOEntity ioentity = ioslot.connectedTo.Get(true);
						if (ioentity != null)
						{
							bool flag = ioentity.WantsPower();
							if (!listToUse.Contains(ioentity))
							{
								if (flag)
								{
									this.AddConnectedRecursive(ioentity, ref listToUse);
								}
								else
								{
									listToUse.Add(ioentity);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x000ADE80 File Offset: 0x000AC080
	public int GetDrain()
	{
		this.connectedList.Clear();
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		if (ioentity)
		{
			this.AddConnectedRecursive(ioentity, ref this.connectedList);
		}
		int num = 0;
		foreach (global::IOEntity ioentity2 in this.connectedList)
		{
			if (ioentity2.ShouldDrainBattery(this))
			{
				num += ioentity2.DesiredPower();
				if (num >= this.maxOutput)
				{
					num = this.maxOutput;
					break;
				}
			}
		}
		return num;
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x000ADF28 File Offset: 0x000AC128
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		int drain = this.GetDrain();
		this.activeDrain = drain;
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x000ADF4C File Offset: 0x000AC14C
	public void CheckDischarge()
	{
		if (this.rustWattSeconds < 5f)
		{
			this.SetDischarging(false);
			return;
		}
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		int drain = this.GetDrain();
		this.activeDrain = drain;
		if (ioentity)
		{
			this.SetDischarging(ioentity.WantsPower());
			return;
		}
		this.SetDischarging(false);
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x000ADFAC File Offset: 0x000AC1AC
	public void SetDischarging(bool wantsOn)
	{
		this.SetPassthroughOn(wantsOn);
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x000ADFB5 File Offset: 0x000AC1B5
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.IsOn())
		{
			return Mathf.FloorToInt((float)this.maxOutput * ((this.rustWattSeconds >= 1f) ? 1f : 0f));
		}
		return 0;
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x000ADFE7 File Offset: 0x000AC1E7
	public override bool WantsPower()
	{
		return this.rustWattSeconds < this.maxCapactiySeconds;
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x000ADFF8 File Offset: 0x000AC1F8
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			if (!this.IsPowered())
			{
				if (this.rechargable)
				{
					base.CancelInvoke(new Action(this.AddCharge));
					return;
				}
			}
			else if (this.rechargable && !base.IsInvoking(new Action(this.AddCharge)))
			{
				base.InvokeRandomized(new Action(this.AddCharge), 1f, 1f, 0.1f);
			}
		}
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x000AE070 File Offset: 0x000AC270
	public void TickUsage()
	{
		float num = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > 0f;
		if (this.rustWattSeconds >= 1f)
		{
			float num2 = 1f * (float)this.activeDrain;
			this.rustWattSeconds -= num2;
		}
		if (this.rustWattSeconds <= 0f)
		{
			this.rustWattSeconds = 0f;
		}
		bool flag2 = this.rustWattSeconds > 0f;
		this.ChargeChanged(num);
		if (flag != flag2)
		{
			this.MarkDirty();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x000AE0F8 File Offset: 0x000AC2F8
	public virtual void ChargeChanged(float oldCharge)
	{
		float num = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > this.maxCapactiySeconds * 0.25f;
		bool flag2 = this.rustWattSeconds > this.maxCapactiySeconds * 0.75f;
		if (base.HasFlag(global::BaseEntity.Flags.Reserved5) != flag || base.HasFlag(global::BaseEntity.Flags.Reserved6) != flag2)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, flag, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, flag2, false, false);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x000AE178 File Offset: 0x000AC378
	public void AddCharge()
	{
		float num = this.rustWattSeconds;
		float num2 = (float)Mathf.Min(this.currentEnergy, this.DesiredPower()) * 1f * this.chargeRatio;
		this.rustWattSeconds += num2;
		this.rustWattSeconds = Mathf.Clamp(this.rustWattSeconds, 0f, this.maxCapactiySeconds);
		this.ChargeChanged(num);
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x000AE1E0 File Offset: 0x000AC3E0
	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == base.IsOn() && !this.wasLoaded)
		{
			return;
		}
		this.wasLoaded = false;
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		if (base.IsOn())
		{
			if (!base.IsInvoking(new Action(this.TickUsage)))
			{
				base.InvokeRandomized(new Action(this.TickUsage), 1f, 1f, 0.1f);
			}
		}
		else
		{
			base.CancelInvoke(new Action(this.TickUsage));
		}
		this.MarkDirty();
	}

	// Token: 0x06001664 RID: 5732 RVA: 0x00062BCC File Offset: 0x00060DCC
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x000AE267 File Offset: 0x000AC467
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericFloat1 = this.rustWattSeconds;
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000AE2A3 File Offset: 0x000AC4A3
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.rustWattSeconds = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000E8D RID: 3725
	public int maxOutput;

	// Token: 0x04000E8E RID: 3726
	public float maxCapactiySeconds;

	// Token: 0x04000E8F RID: 3727
	public float rustWattSeconds;

	// Token: 0x04000E90 RID: 3728
	private int activeDrain;

	// Token: 0x04000E91 RID: 3729
	public bool rechargable;

	// Token: 0x04000E92 RID: 3730
	[Tooltip("How much energy we can request from power sources for charging is this value * our maxOutput")]
	public float maximumInboundEnergyRatio = 4f;

	// Token: 0x04000E93 RID: 3731
	public float chargeRatio = 0.25f;

	// Token: 0x04000E94 RID: 3732
	private const float tickRateSeconds = 1f;

	// Token: 0x04000E95 RID: 3733
	public const global::BaseEntity.Flags Flag_HalfFull = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000E96 RID: 3734
	public const global::BaseEntity.Flags Flag_VeryFull = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000E97 RID: 3735
	private bool wasLoaded;

	// Token: 0x04000E98 RID: 3736
	private HashSet<global::IOEntity> connectedList = new HashSet<global::IOEntity>();
}
