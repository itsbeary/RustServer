using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200001E RID: 30
public class FogMachine : ContainerIOEntity
{
	// Token: 0x06000078 RID: 120 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsEmitting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool HasJuice()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00003FB8 File Offset: 0x000021B8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetFogOn(BaseEntity.RPCMessage msg)
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		if (!this.HasFuel())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0000401B File Offset: 0x0000221B
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetFogOff(BaseEntity.RPCMessage msg)
	{
		if (!base.IsOn())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.CancelInvoke(new Action(this.StartFogging));
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004050 File Offset: 0x00002250
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetMotionDetection(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved9, flag, false, true);
		if (flag)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
		this.UpdateMotionMode();
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00004098 File Offset: 0x00002298
	public void UpdateMotionMode()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved9))
		{
			base.InvokeRandomized(new Action(this.CheckTrigger), UnityEngine.Random.Range(0f, 0.5f), 0.5f, 0.1f);
			return;
		}
		base.CancelInvoke(new Action(this.CheckTrigger));
	}

	// Token: 0x0600007E RID: 126 RVA: 0x000040F0 File Offset: 0x000022F0
	public void CheckTrigger()
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (BasePlayer.AnyPlayersVisibleToEntity(base.transform.position + base.transform.forward * 3f, 3f, this, base.transform.position + Vector3.up * 0.1f, true))
		{
			this.StartFogging();
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004160 File Offset: 0x00002360
	public void StartFogging()
	{
		if (!this.UseFuel(1f))
		{
			base.CancelInvoke(new Action(this.StartFogging));
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
		base.Invoke(new Action(this.EnableFogField), 1f);
		base.Invoke(new Action(this.DisableNozzle), this.nozzleBlastDuration);
		base.Invoke(new Action(this.FinishFogging), this.fogLength);
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000041EE File Offset: 0x000023EE
	public virtual void EnableFogField()
	{
		base.SetFlag(BaseEntity.Flags.Reserved10, true, false, true);
	}

	// Token: 0x06000081 RID: 129 RVA: 0x000041FE File Offset: 0x000023FE
	public void DisableNozzle()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x06000082 RID: 130 RVA: 0x0000420E File Offset: 0x0000240E
	public virtual void FinishFogging()
	{
		base.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004220 File Offset: 0x00002420
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		if (base.IsOn())
		{
			base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
		}
		this.UpdateMotionMode();
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00004293 File Offset: 0x00002493
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06000085 RID: 133 RVA: 0x000042B0 File Offset: 0x000024B0
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000042DE File Offset: 0x000024DE
	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000042EC File Offset: 0x000024EC
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
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "fog", true, false);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00004374 File Offset: 0x00002574
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		bool flag = false;
		if (inputSlot == 0)
		{
			flag = inputAmount > 0;
		}
		else if (inputSlot == 1)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = true;
		}
		else if (inputSlot == 2)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = false;
		}
		if (flag)
		{
			if (this.IsEmitting())
			{
				return;
			}
			if (base.IsOn())
			{
				return;
			}
			if (!this.HasFuel())
			{
				return;
			}
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
			return;
		}
		else
		{
			if (!base.IsOn())
			{
				return;
			}
			base.CancelInvoke(new Action(this.StartFogging));
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00004420 File Offset: 0x00002620
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FogMachine.OnRpcMessage", 0))
		{
			if (rpc == 2788115565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFogOff ");
				}
				using (TimeWarning.New("SetFogOff", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2788115565U, "SetFogOff", this, player, 3f))
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
							this.SetFogOff(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetFogOff");
					}
				}
				return true;
			}
			if (rpc == 3905831928U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFogOn ");
				}
				using (TimeWarning.New("SetFogOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3905831928U, "SetFogOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFogOn(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetFogOn");
					}
				}
				return true;
			}
			if (rpc == 1773639087U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetMotionDetection ");
				}
				using (TimeWarning.New("SetMotionDetection", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1773639087U, "SetMotionDetection", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetMotionDetection(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SetMotionDetection");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0400006D RID: 109
	public const BaseEntity.Flags FogFieldOn = BaseEntity.Flags.Reserved10;

	// Token: 0x0400006E RID: 110
	public const BaseEntity.Flags MotionMode = BaseEntity.Flags.Reserved9;

	// Token: 0x0400006F RID: 111
	public const BaseEntity.Flags Emitting = BaseEntity.Flags.Reserved6;

	// Token: 0x04000070 RID: 112
	public const BaseEntity.Flags Flag_HasJuice = BaseEntity.Flags.Reserved5;

	// Token: 0x04000071 RID: 113
	public float fogLength = 60f;

	// Token: 0x04000072 RID: 114
	public float nozzleBlastDuration = 5f;

	// Token: 0x04000073 RID: 115
	public float fuelPerSec = 1f;

	// Token: 0x04000074 RID: 116
	private float pendingFuel;
}
