using System;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D0 RID: 208
public class SmartAlarm : AppIOEntity, ISubscribable
{
	// Token: 0x06001289 RID: 4745 RVA: 0x00095C5C File Offset: 0x00093E5C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartAlarm.OnRpcMessage", 0))
		{
			if (rpc == 3292290572U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetNotificationTextImpl ");
				}
				using (TimeWarning.New("SetNotificationTextImpl", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3292290572U, "SetNotificationTextImpl", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3292290572U, "SetNotificationTextImpl", this, player, 3f))
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
							this.SetNotificationTextImpl(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetNotificationTextImpl");
					}
				}
				return true;
			}
			if (rpc == 4207149767U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartSetupNotification ");
				}
				using (TimeWarning.New("StartSetupNotification", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4207149767U, "StartSetupNotification", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4207149767U, "StartSetupNotification", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.StartSetupNotification(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StartSetupNotification");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x0600128A RID: 4746 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Alarm;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x0600128B RID: 4747 RVA: 0x00095F94 File Offset: 0x00094194
	// (set) Token: 0x0600128C RID: 4748 RVA: 0x00095F9C File Offset: 0x0009419C
	public override bool Value { get; set; }

	// Token: 0x0600128D RID: 4749 RVA: 0x00095FA5 File Offset: 0x000941A5
	public bool AddSubscription(ulong steamId)
	{
		return this._subscriptions.AddSubscription(steamId);
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00095FB3 File Offset: 0x000941B3
	public bool RemoveSubscription(ulong steamId)
	{
		return this._subscriptions.RemoveSubscription(steamId);
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00095FC1 File Offset: 0x000941C1
	public bool HasSubscription(ulong steamId)
	{
		return this._subscriptions.HasSubscription(steamId);
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00095FCF File Offset: 0x000941CF
	public override void InitShared()
	{
		base.InitShared();
		this._notificationTitle = global::SmartAlarm.DefaultNotificationTitle.translated;
		this._notificationBody = global::SmartAlarm.DefaultNotificationBody.translated;
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x00095FF8 File Offset: 0x000941F8
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		this.Value = inputAmount > 0;
		if (this.Value == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, this.Value, false, true);
		base.BroadcastValueChange();
		float num = Mathf.Max(App.alarmcooldown, 15f);
		if (this.Value && UnityEngine.Time.realtimeSinceStartup - this._lastSentTime >= num)
		{
			BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
			if (buildingPrivilege != null)
			{
				this._subscriptions.IntersectWith(buildingPrivilege.authorizedPlayers);
			}
			this._subscriptions.SendNotification(NotificationChannel.SmartAlarm, this._notificationTitle, this._notificationBody, "alarm");
			this._lastSentTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x000960A8 File Offset: 0x000942A8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.smartAlarm = Facepunch.Pool.Get<ProtoBuf.SmartAlarm>();
			info.msg.smartAlarm.notificationTitle = this._notificationTitle;
			info.msg.smartAlarm.notificationBody = this._notificationBody;
			info.msg.smartAlarm.subscriptions = this._subscriptions.ToList();
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0009611C File Offset: 0x0009431C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.smartAlarm != null)
		{
			this._notificationTitle = info.msg.smartAlarm.notificationTitle;
			this._notificationBody = info.msg.smartAlarm.notificationBody;
			this._subscriptions.LoadFrom(info.msg.smartAlarm.subscriptions);
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x0009618C File Offset: 0x0009438C
	protected override void OnPairedWithPlayer(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!this.AddSubscription(player.userID))
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", 7);
		}
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x000961B4 File Offset: 0x000943B4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void StartSetupNotification(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		base.ClientRPCPlayer<string, string>(null, rpc.player, "SetupNotification", this._notificationTitle, this._notificationBody);
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x0009620C File Offset: 0x0009440C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void SetNotificationTextImpl(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		string text = rpc.read.String(128);
		string text2 = rpc.read.String(512);
		if (!string.IsNullOrWhiteSpace(text))
		{
			this._notificationTitle = text;
		}
		if (!string.IsNullOrWhiteSpace(text2))
		{
			this._notificationBody = text2;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}

	// Token: 0x04000B8D RID: 2957
	public const global::BaseEntity.Flags Flag_HasCustomMessage = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000B8E RID: 2958
	public static readonly Translate.Phrase DefaultNotificationTitle = new Translate.Phrase("app.alarm.title", "Alarm");

	// Token: 0x04000B8F RID: 2959
	public static readonly Translate.Phrase DefaultNotificationBody = new Translate.Phrase("app.alarm.body", "Your base is under attack!");

	// Token: 0x04000B90 RID: 2960
	[Header("Smart Alarm")]
	public GameObjectRef SetupNotificationDialog;

	// Token: 0x04000B91 RID: 2961
	public Animator Animator;

	// Token: 0x04000B93 RID: 2963
	private readonly NotificationList _subscriptions = new NotificationList();

	// Token: 0x04000B94 RID: 2964
	private string _notificationTitle = "";

	// Token: 0x04000B95 RID: 2965
	private string _notificationBody = "";

	// Token: 0x04000B96 RID: 2966
	private float _lastSentTime;
}
