using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x02000038 RID: 56
public class BaseArcadeMachine : global::BaseVehicle
{
	// Token: 0x06000233 RID: 563 RVA: 0x000271C0 File Offset: 0x000253C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseArcadeMachine.OnRpcMessage", 0))
		{
			if (rpc == 271542211U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BroadcastEntityMessage ");
				}
				using (TimeWarning.New("BroadcastEntityMessage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(271542211U, "BroadcastEntityMessage", this, player, 7UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(271542211U, "BroadcastEntityMessage", this, player, 3f))
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
							this.BroadcastEntityMessage(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BroadcastEntityMessage");
					}
				}
				return true;
			}
			if (rpc == 1365277306U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DestroyMessageFromHost ");
				}
				using (TimeWarning.New("DestroyMessageFromHost", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1365277306U, "DestroyMessageFromHost", this, player, 3f))
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
							this.DestroyMessageFromHost(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DestroyMessageFromHost");
					}
				}
				return true;
			}
			if (rpc == 2467852388U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - GetSnapshotFromClient ");
				}
				using (TimeWarning.New("GetSnapshotFromClient", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2467852388U, "GetSnapshotFromClient", this, player, 30UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2467852388U, "GetSnapshotFromClient", this, player, 3f))
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
							this.GetSnapshotFromClient(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in GetSnapshotFromClient");
					}
				}
				return true;
			}
			if (rpc == 2990871635U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestAddScore ");
				}
				using (TimeWarning.New("RequestAddScore", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2990871635U, "RequestAddScore", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RequestAddScore(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RequestAddScore");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x000277B0 File Offset: 0x000259B0
	public void AddScore(global::BasePlayer player, int score)
	{
		BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry();
		scoreEntry.displayName = player.displayName;
		scoreEntry.score = score;
		scoreEntry.playerID = player.userID;
		this.scores.Add(scoreEntry);
		this.scores.Sort((BaseArcadeMachine.ScoreEntry a, BaseArcadeMachine.ScoreEntry b) => b.score.CompareTo(a.score));
		this.scores.TrimExcess();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0002782C File Offset: 0x00025A2C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RequestAddScore(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		int num = msg.read.Int32();
		this.AddScore(player, num);
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00027868 File Offset: 0x00025A68
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.ClientRPCPlayer(null, player, "BeginHosting");
		base.SetFlag(global::BaseEntity.Flags.Reserved7, true, true, true);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0002788D File Offset: 0x00025A8D
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.ClientRPCPlayer(null, player, "EndHosting");
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, true, true);
		if (!this.AnyMounted())
		{
			this.NearbyClientMessage("NoHost");
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000278C8 File Offset: 0x00025AC8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.arcadeMachine = Facepunch.Pool.Get<ArcadeMachine>();
		info.msg.arcadeMachine.scores = Facepunch.Pool.GetList<ArcadeMachine.ScoreEntry>();
		for (int i = 0; i < this.scores.Count; i++)
		{
			ArcadeMachine.ScoreEntry scoreEntry = Facepunch.Pool.Get<ArcadeMachine.ScoreEntry>();
			scoreEntry.displayName = this.scores[i].displayName;
			scoreEntry.playerID = this.scores[i].playerID;
			scoreEntry.score = this.scores[i].score;
			info.msg.arcadeMachine.scores.Add(scoreEntry);
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00027978 File Offset: 0x00025B78
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.arcadeMachine != null && info.msg.arcadeMachine.scores != null)
		{
			this.scores.Clear();
			for (int i = 0; i < info.msg.arcadeMachine.scores.Count; i++)
			{
				BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry();
				scoreEntry.displayName = info.msg.arcadeMachine.scores[i].displayName;
				scoreEntry.score = info.msg.arcadeMachine.scores[i].score;
				scoreEntry.playerID = info.msg.arcadeMachine.scores[i].playerID;
				this.scores.Add(scoreEntry);
			}
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00027A54 File Offset: 0x00025C54
	public void NearbyClientMessage(string msg)
	{
		if (this.networkTrigger.entityContents != null)
		{
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer(null, component, msg);
			}
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00027AC0 File Offset: 0x00025CC0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void DestroyMessageFromHost(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || base.GetDriver() != player)
		{
			return;
		}
		if (this.networkTrigger.entityContents != null)
		{
			uint num = msg.read.UInt32();
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<uint>(null, component, "DestroyEntity", num);
			}
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00027B5C File Offset: 0x00025D5C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(7UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void BroadcastEntityMessage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || base.GetDriver() != player)
		{
			return;
		}
		if (this.networkTrigger.entityContents != null)
		{
			uint num = msg.read.UInt32();
			string text = msg.read.String(256);
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<uint, string>(null, component, "GetEntityMessage", num, text);
			}
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00027C0C File Offset: 0x00025E0C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(30UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void GetSnapshotFromClient(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || player != base.GetDriver())
		{
			return;
		}
		ArcadeGame arcadeGame = Facepunch.Pool.Get<ArcadeGame>();
		arcadeGame = ArcadeGame.Deserialize(msg.read);
		Connection connection = null;
		if (this.networkTrigger.entityContents != null)
		{
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<ArcadeGame>(connection, component, "GetSnapshotFromServer", arcadeGame);
			}
		}
	}

	// Token: 0x04000219 RID: 537
	public BaseArcadeGame arcadeGamePrefab;

	// Token: 0x0400021A RID: 538
	public BaseArcadeGame activeGame;

	// Token: 0x0400021B RID: 539
	public ArcadeNetworkTrigger networkTrigger;

	// Token: 0x0400021C RID: 540
	public float broadcastRadius = 8f;

	// Token: 0x0400021D RID: 541
	public Transform gameScreen;

	// Token: 0x0400021E RID: 542
	public RawImage RTImage;

	// Token: 0x0400021F RID: 543
	public Transform leftJoystick;

	// Token: 0x04000220 RID: 544
	public Transform rightJoystick;

	// Token: 0x04000221 RID: 545
	public SoundPlayer musicPlayer;

	// Token: 0x04000222 RID: 546
	public const global::BaseEntity.Flags Flag_P1 = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000223 RID: 547
	public const global::BaseEntity.Flags Flag_P2 = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000224 RID: 548
	public List<BaseArcadeMachine.ScoreEntry> scores = new List<BaseArcadeMachine.ScoreEntry>(10);

	// Token: 0x04000225 RID: 549
	private const int inputFrameRate = 60;

	// Token: 0x04000226 RID: 550
	private const int snapshotFrameRate = 15;

	// Token: 0x02000B79 RID: 2937
	public class ScoreEntry
	{
		// Token: 0x04003F8E RID: 16270
		public ulong playerID;

		// Token: 0x04003F8F RID: 16271
		public int score;

		// Token: 0x04003F90 RID: 16272
		public string displayName;
	}
}
