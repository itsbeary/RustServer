using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000071 RID: 113
public class ElectricSwitch : IOEntity
{
	// Token: 0x06000ADE RID: 2782 RVA: 0x00062994 File Offset: 0x00060B94
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricSwitch.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00007649 File Offset: 0x00005849
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00062AFC File Offset: 0x00060CFC
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00062B09 File Offset: 0x00060D09
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00062B15 File Offset: 0x00060D15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00062B27 File Offset: 0x00060D27
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

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00062B4F File Offset: 0x00060D4F
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00062B68 File Offset: 0x00060D68
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00062BBB File Offset: 0x00060DBB
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00062BCC File Offset: 0x00060DCC
	public void Unbusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x04000707 RID: 1799
	public bool isToggleSwitch;
}
