using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CD RID: 205
public class SleepingBagCamper : global::SleepingBag
{
	// Token: 0x06001264 RID: 4708 RVA: 0x00094914 File Offset: 0x00092B14
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBagCamper.OnRpcMessage", 0))
		{
			if (rpc == 2177887503U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerClearBed ");
				}
				using (TimeWarning.New("ServerClearBed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2177887503U, "ServerClearBed", this, player, 3f))
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
							this.ServerClearBed(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerClearBed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x00094A7C File Offset: 0x00092C7C
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x00094A94 File Offset: 0x00092C94
	protected override void PostPlayerSpawn(global::BasePlayer p)
	{
		base.PostPlayerSpawn(p);
		BaseVehicleSeat baseVehicleSeat = this.AssociatedSeat.Get(base.isServer);
		if (baseVehicleSeat != null)
		{
			p.EndSleeping();
			baseVehicleSeat.MountPlayer(p);
		}
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x00094AD0 File Offset: 0x00092CD0
	public void SetSeat(BaseVehicleSeat seat, bool sendNetworkUpdate = false)
	{
		this.AssociatedSeat.Set(seat);
		if (sendNetworkUpdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x00094AE8 File Offset: 0x00092CE8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.sleepingBagCamper = Facepunch.Pool.Get<ProtoBuf.SleepingBagCamper>();
			info.msg.sleepingBagCamper.seatID = this.AssociatedSeat.uid;
		}
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00094B24 File Offset: 0x00092D24
	public override bool IsOccupied()
	{
		return (this.AssociatedSeat.IsValid(base.isServer) && this.AssociatedSeat.Get(base.isServer).AnyMounted()) || WaterLevel.Test(base.transform.position, true, false, null);
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00094B74 File Offset: 0x00092D74
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void ServerClearBed(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || !this.AssociatedSeat.IsValid(base.isServer) || this.AssociatedSeat.Get(base.isServer).GetMounted() != player)
		{
			return;
		}
		global::SleepingBag.RemoveBagForPlayer(this, this.deployerUserID);
		this.deployerUserID = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04000B69 RID: 2921
	public EntityRef<BaseVehicleSeat> AssociatedSeat;
}
