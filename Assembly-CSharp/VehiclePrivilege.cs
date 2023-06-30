using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000ED RID: 237
public class VehiclePrivilege : global::BaseEntity
{
	// Token: 0x060014C4 RID: 5316 RVA: 0x000A354C File Offset: 0x000A174C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehiclePrivilege.OnRpcMessage", 0))
		{
			if (rpc == 1092560690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddSelfAuthorize ");
				}
				using (TimeWarning.New("AddSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1092560690U, "AddSelfAuthorize", this, player, 3f))
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
							this.AddSelfAuthorize(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 253307592U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearList ");
				}
				using (TimeWarning.New("ClearList", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(253307592U, "ClearList", this, player, 3f))
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
							this.ClearList(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearList");
					}
				}
				return true;
			}
			if (rpc == 3617985969U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RemoveSelfAuthorize ");
				}
				using (TimeWarning.New("RemoveSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3617985969U, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RemoveSelfAuthorize(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x000A39A8 File Offset: 0x000A1BA8
	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x000A39BC File Offset: 0x000A1BBC
	public bool IsAuthed(global::BasePlayer player)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x000A39F0 File Offset: 0x000A1BF0
	public bool IsAuthed(ulong userID)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == userID);
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x000A3A21 File Offset: 0x000A1C21
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x000A3A31 File Offset: 0x000A1C31
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Facepunch.Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = this.authorizedPlayers;
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000577B5 File Offset: 0x000559B5
	public override void PostSave(global::BaseNetworkable.SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000A3A60 File Offset: 0x000A1C60
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			info.msg.buildingPrivilege.users = null;
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000A3AC8 File Offset: 0x000A1CC8
	public bool IsDriver(global::BasePlayer player)
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity == null)
		{
			return false;
		}
		global::BaseVehicle baseVehicle = parentEntity as global::BaseVehicle;
		return !(baseVehicle == null) && baseVehicle.IsDriver(player);
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool AtMaxAuthCapacity()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000A3B00 File Offset: 0x000A1D00
	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && activeGameMode.limitTeamAuths)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, this.authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize(), false, true);
		}
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000A3B47 File Offset: 0x000A1D47
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void AddSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.IsDriver(rpc.player))
		{
			return;
		}
		this.AddPlayer(rpc.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x000A3B7C File Offset: 0x000A1D7C
	public void AddPlayer(global::BasePlayer player)
	{
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = player.userID;
		playerNameID.username = player.displayName;
		this.authorizedPlayers.Add(playerNameID);
		Analytics.Azure.OnEntityAuthChanged(this, player, this.authorizedPlayers.Select((PlayerNameID x) => x.userid), "added", player.userID);
		this.UpdateMaxAuthCapacity();
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x000A3C38 File Offset: 0x000A1E38
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RemoveSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.IsDriver(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		Analytics.Azure.OnEntityAuthChanged(this, rpc.player, this.authorizedPlayers.Select((PlayerNameID x) => x.userid), "removed", rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x000A3CEE File Offset: 0x000A1EEE
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void ClearList(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.IsDriver(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.Clear();
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04000D12 RID: 3346
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x04000D13 RID: 3347
	public const global::BaseEntity.Flags Flag_MaxAuths = global::BaseEntity.Flags.Reserved5;
}
