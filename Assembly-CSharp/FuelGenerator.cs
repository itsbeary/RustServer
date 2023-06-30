using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007C RID: 124
public class FuelGenerator : ContainerIOEntity
{
	// Token: 0x06000B87 RID: 2951 RVA: 0x000666D0 File Offset: 0x000648D0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FuelGenerator.OnRpcMessage", 0))
		{
			if (rpc == 1401355317U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EngineSwitch ");
				}
				using (TimeWarning.New("RPC_EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1401355317U, "RPC_EngineSwitch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_EngineSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00066838 File Offset: 0x00064A38
	public override int MaximalPowerOutput()
	{
		return this.outputEnergy;
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00066840 File Offset: 0x00064A40
	public override void Init()
	{
		if (base.IsOn())
		{
			this.UpdateCurrentEnergy();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
		base.Init();
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x00066874 File Offset: 0x00064A74
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0 && inputAmount > 0)
		{
			this.TurnOn();
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.TurnOff();
		}
		base.UpdateFromInput(inputAmount, inputSlot);
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00066899 File Offset: 0x00064A99
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.outputEnergy;
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x000668AB File Offset: 0x00064AAB
	public void UpdateCurrentEnergy()
	{
		this.currentEnergy = this.CalculateCurrentEnergy(0, 0);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x000668BB File Offset: 0x00064ABB
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x000668C8 File Offset: 0x00064AC8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_EngineSwitch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		this.SetGeneratorState(flag);
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x000668E8 File Offset: 0x00064AE8
	public void SetGeneratorState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x000668FC File Offset: 0x00064AFC
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0006692A File Offset: 0x00064B2A
	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00066938 File Offset: 0x00064B38
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "generator", true, false);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x000669C0 File Offset: 0x00064BC0
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		if (this.UseFuel(1f))
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateCurrentEnergy();
			this.MarkDirty();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00066A17 File Offset: 0x00064C17
	public void FuelConsumption()
	{
		if (!this.UseFuel(this.fuelTickRate))
		{
			this.TurnOff();
		}
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00066A2D File Offset: 0x00064C2D
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.UpdateCurrentEnergy();
		this.MarkDirty();
		base.CancelInvoke(new Action(this.FuelConsumption));
	}

	// Token: 0x04000780 RID: 1920
	public int outputEnergy = 35;

	// Token: 0x04000781 RID: 1921
	public float fuelPerSec = 1f;

	// Token: 0x04000782 RID: 1922
	protected float fuelTickRate = 3f;

	// Token: 0x04000783 RID: 1923
	private float pendingFuel;
}
