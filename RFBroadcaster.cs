using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C1 RID: 193
public class RFBroadcaster : IOEntity, IRFObject
{
	// Token: 0x0600114E RID: 4430 RVA: 0x0008E12C File Offset: 0x0008C32C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFBroadcaster.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
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

	// Token: 0x0600114F RID: 4431 RVA: 0x0008E294 File Offset: 0x0008C494
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0005F95E File Offset: 0x0005DB5E
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x000063A5 File Offset: 0x000045A5
	public void RFSignalUpdate(bool on)
	{
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0008E29C File Offset: 0x0008C49C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (!this.CanChangeFrequency(msg.player))
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int num = msg.read.Int32();
		if (RFManager.IsReserved(num))
		{
			RFManager.ReserveErrorPrint(msg.player);
			return;
		}
		RFManager.ChangeFrequency(this.frequency, num, this, false, this.IsPowered());
		this.frequency = num;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.Hurt(this.MaxHealth() * 0.01f, DamageType.Decay, this, true);
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0008E335 File Offset: 0x0008C535
	public override bool CanUseNetworkCache(Connection connection)
	{
		return !this.playerUsable && base.CanUseNetworkCache(connection);
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0008E348 File Offset: 0x0008C548
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

	// Token: 0x06001157 RID: 4439 RVA: 0x0008E39C File Offset: 0x0008C59C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputAmount > 0)
		{
			base.CancelInvoke(new Action(this.StopBroadcasting));
			RFManager.AddBroadcaster(this.frequency, this);
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.nextStopTime = UnityEngine.Time.time + 1f;
			return;
		}
		base.Invoke(new Action(this.StopBroadcasting), Mathf.Clamp01(this.nextStopTime - UnityEngine.Time.time));
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0008E410 File Offset: 0x0008C610
	public void StopBroadcasting()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0008E42C File Offset: 0x0008C62C
	internal override void DoServerDestroy()
	{
		RFManager.RemoveBroadcaster(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0008E440 File Offset: 0x0008C640
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0008E46C File Offset: 0x0008C66C
	private bool CanChangeFrequency(BasePlayer player)
	{
		return this.playerUsable && player != null && player.CanBuild();
	}

	// Token: 0x04000AD2 RID: 2770
	public int frequency;

	// Token: 0x04000AD3 RID: 2771
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04000AD4 RID: 2772
	public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;

	// Token: 0x04000AD5 RID: 2773
	public bool playerUsable = true;

	// Token: 0x04000AD6 RID: 2774
	private float nextChangeTime;

	// Token: 0x04000AD7 RID: 2775
	private float nextStopTime;
}
