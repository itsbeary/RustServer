using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000060 RID: 96
public class ComputerStation : BaseMountable
{
	// Token: 0x060009E5 RID: 2533 RVA: 0x0005CBD0 File Offset: 0x0005ADD0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ComputerStation.OnRpcMessage", 0))
		{
			if (rpc == 481778085U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddBookmark ");
				}
				using (TimeWarning.New("AddBookmark", 0))
				{
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
							this.AddBookmark(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddBookmark");
					}
				}
				return true;
			}
			if (rpc == 552248427U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginControllingBookmark ");
				}
				using (TimeWarning.New("BeginControllingBookmark", 0))
				{
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
							this.BeginControllingBookmark(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in BeginControllingBookmark");
					}
				}
				return true;
			}
			if (rpc == 2498687923U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DeleteBookmark ");
				}
				using (TimeWarning.New("DeleteBookmark", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DeleteBookmark(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in DeleteBookmark");
					}
				}
				return true;
			}
			if (rpc == 2139261430U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_DisconnectControl ");
				}
				using (TimeWarning.New("Server_DisconnectControl", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_DisconnectControl(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_DisconnectControl");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x0005D048 File Offset: 0x0005B248
	public bool AllowPings()
	{
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(base.isServer);
		IRemoteControllable remoteControllable;
		return baseEntity != null && (remoteControllable = baseEntity as IRemoteControllable) != null && remoteControllable.CanPing;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0005D085 File Offset: 0x0005B285
	public static bool IsValidIdentifier(string str)
	{
		return !string.IsNullOrEmpty(str) && str.Length <= 32 && str.IsAlphaNumeric();
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0005D0A3 File Offset: 0x0005B2A3
	public override void DestroyShared()
	{
		if (base.isServer && base.GetMounted())
		{
			this.StopControl(base.GetMounted());
		}
		base.DestroyShared();
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0005D0CC File Offset: 0x0005B2CC
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.GatherStaticCameras), 5f);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0005D0EC File Offset: 0x0005B2EC
	public void GatherStaticCameras()
	{
		if (Rust.Application.isLoadingSave)
		{
			base.Invoke(new Action(this.GatherStaticCameras), 1f);
			return;
		}
		if (this.isStatic && this.autoGatherRadius > 0f)
		{
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(base.transform.position, this.autoGatherRadius, list, 256, QueryTriggerInteraction.Ignore);
			foreach (global::BaseEntity baseEntity in list)
			{
				IRemoteControllable component = baseEntity.GetComponent<IRemoteControllable>();
				if (component != null)
				{
					CCTV_RC component2 = baseEntity.GetComponent<CCTV_RC>();
					if (!(component2 == null) && component2.IsStatic() && !this.controlBookmarks.Contains(component.GetIdentifier()))
					{
						this.ForceAddBookmark(component.GetIdentifier());
					}
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0005D1DC File Offset: 0x0005B3DC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.GatherStaticCameras();
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0005D1EC File Offset: 0x0005B3EC
	public void StopControl(global::BasePlayer ply)
	{
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
		if (baseEntity)
		{
			baseEntity.GetComponent<IRemoteControllable>().StopControl(new CameraViewerId(this.currentPlayerID, 0L));
		}
		if (ply)
		{
			ply.net.SwitchSecondaryGroup(null);
		}
		this.currentlyControllingEnt.uid = default(NetworkableId);
		this.currentPlayerID = 0UL;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.SendControlBookmarks(ply);
		base.CancelInvoke(new Action(this.ControlCheck));
		base.CancelInvoke(new Action(this.CheckCCTVAchievement));
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0005D297 File Offset: 0x0005B497
	public bool IsPlayerAdmin(global::BasePlayer player)
	{
		return player == this._mounted;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0005D2A8 File Offset: 0x0005B4A8
	[global::BaseEntity.RPC_Server]
	public void DeleteBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		if (this.isStatic)
		{
			return;
		}
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		if (this.controlBookmarks.Contains(text))
		{
			this.controlBookmarks.Remove(text);
			this.SendControlBookmarks(player);
			global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
			IRemoteControllable remoteControllable;
			if (baseEntity != null && baseEntity.TryGetComponent<IRemoteControllable>(out remoteControllable) && remoteControllable.GetIdentifier() == text)
			{
				this.StopControl(player);
			}
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0005D340 File Offset: 0x0005B540
	[global::BaseEntity.RPC_Server]
	public void Server_DisconnectControl(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		this.StopControl(player);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0005D368 File Offset: 0x0005B568
	[global::BaseEntity.RPC_Server]
	public void BeginControllingBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		if (!this.controlBookmarks.Contains(text))
		{
			return;
		}
		IRemoteControllable remoteControllable = RemoteControlEntity.FindByID(text);
		if (remoteControllable == null)
		{
			return;
		}
		global::BaseEntity ent = remoteControllable.GetEnt();
		if (ent == null)
		{
			Debug.LogWarning("RC identifier " + text + " was found but has a null or destroyed entity, this should never happen");
			return;
		}
		if (!remoteControllable.CanControl(player.userID))
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, ent.transform.position) >= remoteControllable.MaxRange)
		{
			return;
		}
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
		if (baseEntity)
		{
			IRemoteControllable component = baseEntity.GetComponent<IRemoteControllable>();
			if (component != null)
			{
				component.StopControl(new CameraViewerId(this.currentPlayerID, 0L));
			}
		}
		player.net.SwitchSecondaryGroup(ent.net.group);
		this.currentlyControllingEnt.uid = ent.net.ID;
		this.currentPlayerID = player.userID;
		bool flag = remoteControllable.InitializeControl(new CameraViewerId(this.currentPlayerID, 0L));
		base.SetFlag(global::BaseEntity.Flags.Reserved2, flag, false, false);
		base.SendNetworkUpdateImmediate(false);
		this.SendControlBookmarks(player);
		if (GameInfo.HasAchievements && remoteControllable.GetEnt() is CCTV_RC)
		{
			base.InvokeRepeating(new Action(this.CheckCCTVAchievement), 1f, 3f);
		}
		base.InvokeRepeating(new Action(this.ControlCheck), 0f, 0f);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0005D504 File Offset: 0x0005B704
	private void CheckCCTVAchievement()
	{
		if (this._mounted != null)
		{
			global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
			CCTV_RC cctv_RC;
			if (baseEntity != null && (cctv_RC = baseEntity as CCTV_RC) != null)
			{
				foreach (Connection connection in this._mounted.net.secondaryGroup.subscribers)
				{
					if (connection.active)
					{
						global::BasePlayer basePlayer = connection.player as global::BasePlayer;
						if (!(basePlayer == null))
						{
							Vector3 vector = basePlayer.CenterPoint();
							float num = Vector3.Dot((vector - cctv_RC.pitch.position).normalized, cctv_RC.pitch.forward);
							Vector3 vector2 = cctv_RC.pitch.InverseTransformPoint(vector);
							if (num > 0.6f && vector2.magnitude < 10f)
							{
								this._mounted.GiveAchievement("BIG_BROTHER");
								base.CancelInvoke(new Action(this.CheckCCTVAchievement));
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0005D63C File Offset: 0x0005B83C
	public bool CanAddBookmark(global::BasePlayer player)
	{
		if (!this.IsPlayerAdmin(player))
		{
			return false;
		}
		if (this.isStatic)
		{
			return false;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextAddTime)
		{
			return false;
		}
		if (this.controlBookmarks.Count > 3)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return false;
		}
		return true;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0005D68C File Offset: 0x0005B88C
	public void ForceAddBookmark(string identifier)
	{
		if (this.controlBookmarks.Count >= 128)
		{
			return;
		}
		if (!global::ComputerStation.IsValidIdentifier(identifier))
		{
			return;
		}
		if (this.controlBookmarks.Contains(identifier))
		{
			return;
		}
		IRemoteControllable remoteControllable = RemoteControlEntity.FindByID(identifier);
		if (remoteControllable == null)
		{
			return;
		}
		if (remoteControllable.GetEnt() == null)
		{
			Debug.LogWarning("RC identifier " + identifier + " was found but has a null or destroyed entity, this should never happen");
			return;
		}
		this.controlBookmarks.Add(identifier);
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0005D700 File Offset: 0x0005B900
	[global::BaseEntity.RPC_Server]
	public void AddBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		if (this.isStatic)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextAddTime)
		{
			player.ChatMessage("Slow down...");
			return;
		}
		if (this.controlBookmarks.Count >= 128)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return;
		}
		this.nextAddTime = UnityEngine.Time.realtimeSinceStartup + 1f;
		string text = msg.read.String(256);
		this.ForceAddBookmark(text);
		this.SendControlBookmarks(player);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0005D790 File Offset: 0x0005B990
	public void ControlCheck()
	{
		bool flag = false;
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(base.isServer);
		if (baseEntity && this._mounted)
		{
			IRemoteControllable component = baseEntity.GetComponent<IRemoteControllable>();
			if (component != null && component.CanControl(this._mounted.userID) && Vector3.Distance(base.transform.position, baseEntity.transform.position) < component.MaxRange)
			{
				flag = true;
				this._mounted.net.SwitchSecondaryGroup(baseEntity.net.group);
			}
		}
		if (!flag)
		{
			this.StopControl(this._mounted);
		}
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0005D833 File Offset: 0x0005BA33
	public string GenerateControlBookmarkString()
	{
		return string.Join(";", this.controlBookmarks);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0005D848 File Offset: 0x0005BA48
	public void SendControlBookmarks(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		string text = this.GenerateControlBookmarkString();
		base.ClientRPCPlayer<string>(null, player, "ReceiveBookmarks", text);
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0005D874 File Offset: 0x0005BA74
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		global::BasePlayer mounted = this._mounted;
		if (mounted)
		{
			this.SendControlBookmarks(mounted);
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0005D8A7 File Offset: 0x0005BAA7
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		this.StopControl(player);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0005D8C4 File Offset: 0x0005BAC4
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2) && this.currentlyControllingEnt.IsValid(true))
		{
			this.currentlyControllingEnt.Get(true).GetComponent<IRemoteControllable>().UserInput(inputState, new CameraViewerId(player.userID, 0L));
		}
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0005D918 File Offset: 0x0005BB18
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
			info.msg.ioEntity.genericEntRef1 = this.currentlyControllingEnt.uid;
			return;
		}
		info.msg.computerStation = Facepunch.Pool.Get<ProtoBuf.ComputerStation>();
		info.msg.computerStation.bookmarks = this.GenerateControlBookmarkString();
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0005D988 File Offset: 0x0005BB88
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk)
		{
			if (info.msg.ioEntity != null)
			{
				this.currentlyControllingEnt.uid = info.msg.ioEntity.genericEntRef1;
				return;
			}
		}
		else if (info.msg.computerStation != null)
		{
			foreach (string text in info.msg.computerStation.bookmarks.Split(global::ComputerStation.BookmarkSplit, StringSplitOptions.RemoveEmptyEntries))
			{
				if (global::ComputerStation.IsValidIdentifier(text))
				{
					this.controlBookmarks.Add(text);
				}
			}
		}
	}

	// Token: 0x04000699 RID: 1689
	public const global::BaseEntity.Flags Flag_HasFullControl = global::BaseEntity.Flags.Reserved2;

	// Token: 0x0400069A RID: 1690
	[Header("Computer")]
	public GameObjectRef menuPrefab;

	// Token: 0x0400069B RID: 1691
	public ComputerMenu computerMenu;

	// Token: 0x0400069C RID: 1692
	public EntityRef currentlyControllingEnt;

	// Token: 0x0400069D RID: 1693
	public List<string> controlBookmarks = new List<string>();

	// Token: 0x0400069E RID: 1694
	public Transform leftHandIKPosition;

	// Token: 0x0400069F RID: 1695
	public Transform rightHandIKPosition;

	// Token: 0x040006A0 RID: 1696
	public SoundDefinition turnOnSoundDef;

	// Token: 0x040006A1 RID: 1697
	public SoundDefinition turnOffSoundDef;

	// Token: 0x040006A2 RID: 1698
	public SoundDefinition onLoopSoundDef;

	// Token: 0x040006A3 RID: 1699
	public bool isStatic;

	// Token: 0x040006A4 RID: 1700
	public float autoGatherRadius;

	// Token: 0x040006A5 RID: 1701
	private ulong currentPlayerID;

	// Token: 0x040006A6 RID: 1702
	private float nextAddTime;

	// Token: 0x040006A7 RID: 1703
	private static readonly char[] BookmarkSplit = new char[] { ';' };
}
