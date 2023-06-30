using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000070 RID: 112
public class ElectricalBranch : IOEntity
{
	// Token: 0x06000AD5 RID: 2773 RVA: 0x00062700 File Offset: 0x00060900
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricalBranch.OnRpcMessage", 0))
		{
			if (rpc == 643124146U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetBranchOffPower ");
				}
				using (TimeWarning.New("SetBranchOffPower", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(643124146U, "SetBranchOffPower", this, player, 3f))
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
							this.SetBranchOffPower(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetBranchOffPower");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00062868 File Offset: 0x00060A68
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetBranchOffPower(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player == null || !player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 1f;
		int num = msg.read.Int32();
		num = Mathf.Clamp(num, 2, 10000000);
		this.branchAmount = num;
		base.MarkDirtyForceUpdateOutputs();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x000628DA File Offset: 0x00060ADA
	public override bool AllowDrainFrom(int outputSlot)
	{
		return outputSlot != 1;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000628E3 File Offset: 0x00060AE3
	public override int DesiredPower()
	{
		return this.branchAmount;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x000628EB File Offset: 0x00060AEB
	public void SetBranchAmount(int newAmount)
	{
		newAmount = Mathf.Clamp(newAmount, 2, 100000000);
		this.branchAmount = newAmount;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00062902 File Offset: 0x00060B02
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot == 0)
		{
			return Mathf.Clamp(this.GetCurrentEnergy() - this.branchAmount, 0, this.GetCurrentEnergy());
		}
		if (outputSlot == 1)
		{
			return Mathf.Min(this.GetCurrentEnergy(), this.branchAmount);
		}
		return 0;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00062938 File Offset: 0x00060B38
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.branchAmount;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00062957 File Offset: 0x00060B57
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.branchAmount = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x04000704 RID: 1796
	public int branchAmount = 2;

	// Token: 0x04000705 RID: 1797
	public GameObjectRef branchPanelPrefab;

	// Token: 0x04000706 RID: 1798
	private float nextChangeTime;
}
