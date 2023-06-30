using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C2 RID: 194
public class RFReceiver : IOEntity, IRFObject
{
	// Token: 0x0600115D RID: 4445 RVA: 0x0008E498 File Offset: 0x0008C698
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFReceiver.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFrequency ");
				}
				using (TimeWarning.New("ServerSetFrequency", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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
							this.ServerSetFrequency(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerSetFrequency");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0008E600 File Offset: 0x0008C800
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x00007649 File Offset: 0x00005849
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x00062B09 File Offset: 0x00060D09
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x00062B15 File Offset: 0x00060D15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0005F95E File Offset: 0x0005DB5E
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0008E608 File Offset: 0x0008C808
	public override void Init()
	{
		base.Init();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0008E61C File Offset: 0x0008C81C
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0008E630 File Offset: 0x0008C830
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.IsOn() == on)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, on, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0008E65C File Offset: 0x0008C85C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		int num = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, num, this, true, true);
		this.frequency = num;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0008E6B4 File Offset: 0x0008C8B4
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			Connection forConnection = info.forConnection;
			if (!this.CanChangeFrequency(((forConnection != null) ? forConnection.player : null) as BasePlayer))
			{
				return;
			}
		}
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0008E705 File Offset: 0x0008C905
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x00087C33 File Offset: 0x00085E33
	private bool CanChangeFrequency(BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x04000AD8 RID: 2776
	public int frequency;

	// Token: 0x04000AD9 RID: 2777
	public GameObjectRef frequencyPanelPrefab;
}
