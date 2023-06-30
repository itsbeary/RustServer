using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D1 RID: 209
public class SmartSwitch : AppIOEntity
{
	// Token: 0x06001299 RID: 4761 RVA: 0x000962E8 File Offset: 0x000944E8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2810053005U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleSwitch ");
				}
				using (TimeWarning.New("ToggleSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2810053005U, "ToggleSwitch", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2810053005U, "ToggleSwitch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ToggleSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x00007649 File Offset: 0x00005849
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x0600129B RID: 4763 RVA: 0x0000441C File Offset: 0x0000261C
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Switch;
		}
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00062B4F File Offset: 0x00060D4F
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00062AFC File Offset: 0x00060CFC
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x00062B09 File Offset: 0x00060D09
	public override void ResetIOState()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00062B15 File Offset: 0x00060D15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000964A8 File Offset: 0x000946A8
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x000964D0 File Offset: 0x000946D0
	public void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00096529 File Offset: 0x00094729
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void ToggleSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (!SmartSwitch.PlayerCanToggle(msg.player))
		{
			return;
		}
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x060012A3 RID: 4771 RVA: 0x00007649 File Offset: 0x00005849
	// (set) Token: 0x060012A4 RID: 4772 RVA: 0x00096548 File Offset: 0x00094748
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
			this.SetSwitch(value);
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x00062BCC File Offset: 0x00060DCC
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x00096551 File Offset: 0x00094751
	private static bool PlayerCanToggle(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x04000B97 RID: 2967
	[Header("Smart Switch")]
	public Animator ReceiverAnimator;
}
