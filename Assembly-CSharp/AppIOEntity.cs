using System;
using System.Collections.Generic;
using System.Globalization;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000034 RID: 52
public abstract class AppIOEntity : global::IOEntity
{
	// Token: 0x06000154 RID: 340 RVA: 0x00022060 File Offset: 0x00020260
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AppIOEntity.OnRpcMessage", 0))
		{
			if (rpc == 3018927126U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PairWithApp ");
				}
				using (TimeWarning.New("PairWithApp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3018927126U, "PairWithApp", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3018927126U, "PairWithApp", this, player, 3f))
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
							this.PairWithApp(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in PairWithApp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000155 RID: 341
	public abstract AppEntityType Type { get; }

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000156 RID: 342 RVA: 0x00007A44 File Offset: 0x00005C44
	// (set) Token: 0x06000157 RID: 343 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual bool Value
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00022220 File Offset: 0x00020420
	protected void BroadcastValueChange()
	{
		if (!this.IsValid())
		{
			return;
		}
		EntityTarget target = this.GetTarget();
		AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
		appBroadcast.entityChanged = Facepunch.Pool.Get<AppEntityChanged>();
		appBroadcast.entityChanged.entityId = this.net.ID;
		appBroadcast.entityChanged.payload = Facepunch.Pool.Get<AppEntityPayload>();
		this.FillEntityPayload(appBroadcast.entityChanged.payload);
		CompanionServer.Server.Broadcast(target, appBroadcast);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0002228A File Offset: 0x0002048A
	internal virtual void FillEntityPayload(AppEntityPayload payload)
	{
		payload.value = this.Value;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00022298 File Offset: 0x00020498
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this._cacheTime > 5f)
		{
			this._cache = base.GetBuildingPrivilege();
			this._cacheTime = UnityEngine.Time.realtimeSinceStartup;
		}
		return this._cache;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x000222CA File Offset: 0x000204CA
	public EntityTarget GetTarget()
	{
		return new EntityTarget(this.net.ID);
	}

	// Token: 0x0600015C RID: 348 RVA: 0x000222DC File Offset: 0x000204DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public async void PairWithApp(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		Dictionary<string, string> playerPairingData = CompanionServer.Util.GetPlayerPairingData(player);
		playerPairingData.Add("entityId", this.net.ID.Value.ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityType", ((int)this.Type).ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityName", base.GetDisplayName());
		NotificationSendResult notificationSendResult = await CompanionServer.Util.SendPairNotification("entity", player, base.GetDisplayName(), "Tap to pair with this device.", playerPairingData);
		if (notificationSendResult == NotificationSendResult.Sent)
		{
			this.OnPairedWithPlayer(msg.player);
		}
		else
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", (int)notificationSendResult);
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnPairedWithPlayer(global::BasePlayer player)
	{
	}

	// Token: 0x0400019E RID: 414
	private float _cacheTime;

	// Token: 0x0400019F RID: 415
	private BuildingPrivlidge _cache;
}
