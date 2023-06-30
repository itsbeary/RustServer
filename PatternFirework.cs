using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200001A RID: 26
public class PatternFirework : MortarFirework, IUGCBrowserEntity
{
	// Token: 0x06000060 RID: 96 RVA: 0x000034E6 File Offset: 0x000016E6
	public override void DestroyShared()
	{
		base.DestroyShared();
		ProtoBuf.PatternFirework.Design design = this.Design;
		if (design != null)
		{
			design.Dispose();
		}
		this.Design = null;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00003506 File Offset: 0x00001706
	public override void ServerInit()
	{
		base.ServerInit();
		this.ShellFuseLength = global::PatternFirework.FuseLength.Medium;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003515 File Offset: 0x00001715
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void StartOpenDesigner(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		base.ClientRPCPlayer(null, rpc.player, "OpenDesigner");
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003538 File Offset: 0x00001738
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void ServerSetFireworkDesign(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		ProtoBuf.PatternFirework.Design design = ProtoBuf.PatternFirework.Design.Deserialize(rpc.read);
		if (((design != null) ? design.stars : null) != null)
		{
			while (design.stars.Count > this.MaxStars)
			{
				int num = design.stars.Count - 1;
				design.stars[num].Dispose();
				design.stars.RemoveAt(num);
			}
			foreach (ProtoBuf.PatternFirework.Star star in design.stars)
			{
				star.position = new Vector2(Mathf.Clamp(star.position.x, -1f, 1f), Mathf.Clamp(star.position.y, -1f, 1f));
				star.color = new Color(Mathf.Clamp01(star.color.r), Mathf.Clamp01(star.color.g), Mathf.Clamp01(star.color.b), 1f);
			}
			design.editedBy = rpc.player.userID;
		}
		ProtoBuf.PatternFirework.Design design2 = this.Design;
		if (design2 != null)
		{
			design2.Dispose();
		}
		this.Design = design;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x000036A8 File Offset: 0x000018A8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void SetShellFuseLength(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		this.ShellFuseLength = (global::PatternFirework.FuseLength)Mathf.Clamp(rpc.read.Int32(), 0, 2);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000065 RID: 101 RVA: 0x000036D8 File Offset: 0x000018D8
	private bool PlayerCanModify(global::BasePlayer player)
	{
		if (player == null || !player.CanInteract())
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege != null) || buildingPrivilege.CanAdministrate(player);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003714 File Offset: 0x00001914
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.patternFirework = Facepunch.Pool.Get<ProtoBuf.PatternFirework>();
		ProtoBuf.PatternFirework patternFirework = info.msg.patternFirework;
		ProtoBuf.PatternFirework.Design design = this.Design;
		patternFirework.design = ((design != null) ? design.Copy() : null);
		info.msg.patternFirework.shellFuseLength = (int)this.ShellFuseLength;
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000067 RID: 103 RVA: 0x00003770 File Offset: 0x00001970
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.Design == null || this.Design.stars.Count <= 0)
			{
				return Array.Empty<uint>();
			}
			return new uint[] { 1U };
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000379D File Offset: 0x0000199D
	public void ClearContent()
	{
		ProtoBuf.PatternFirework.Design design = this.Design;
		if (design != null)
		{
			design.Dispose();
		}
		this.Design = null;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000069 RID: 105 RVA: 0x000037BE File Offset: 0x000019BE
	public UGCType ContentType
	{
		get
		{
			return UGCType.PatternBoomer;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600006A RID: 106 RVA: 0x000037C1 File Offset: 0x000019C1
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.Design == null)
			{
				return new List<ulong>();
			}
			return new List<ulong> { this.Design.editedBy };
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600006B RID: 107 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000037EC File Offset: 0x000019EC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.patternFirework != null)
		{
			ProtoBuf.PatternFirework.Design design = this.Design;
			if (design != null)
			{
				design.Dispose();
			}
			ProtoBuf.PatternFirework.Design design2 = info.msg.patternFirework.design;
			this.Design = ((design2 != null) ? design2.Copy() : null);
			this.ShellFuseLength = (global::PatternFirework.FuseLength)info.msg.patternFirework.shellFuseLength;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00003858 File Offset: 0x00001A58
	private float GetShellFuseLength()
	{
		switch (this.ShellFuseLength)
		{
		case global::PatternFirework.FuseLength.Short:
			return this.ShellFuseLengthShort;
		case global::PatternFirework.FuseLength.Medium:
			return this.ShellFuseLengthMed;
		case global::PatternFirework.FuseLength.Long:
			return this.ShellFuseLengthLong;
		default:
			return this.ShellFuseLengthMed;
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x0000389C File Offset: 0x00001A9C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PatternFirework.OnRpcMessage", 0))
		{
			if (rpc == 3850129568U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFireworkDesign ");
				}
				using (TimeWarning.New("ServerSetFireworkDesign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3850129568U, "ServerSetFireworkDesign", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3850129568U, "ServerSetFireworkDesign", this, player, 3f))
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
							this.ServerSetFireworkDesign(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerSetFireworkDesign");
					}
				}
				return true;
			}
			if (rpc == 2132764204U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetShellFuseLength ");
				}
				using (TimeWarning.New("SetShellFuseLength", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2132764204U, "SetShellFuseLength", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2132764204U, "SetShellFuseLength", this, player, 3f))
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
							this.SetShellFuseLength(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetShellFuseLength");
					}
				}
				return true;
			}
			if (rpc == 2760408151U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartOpenDesigner ");
				}
				using (TimeWarning.New("StartOpenDesigner", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760408151U, "StartOpenDesigner", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2760408151U, "StartOpenDesigner", this, player, 3f))
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
							this.StartOpenDesigner(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in StartOpenDesigner");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000058 RID: 88
	public const int CurrentVersion = 1;

	// Token: 0x04000059 RID: 89
	[Header("PatternFirework")]
	public GameObjectRef FireworkDesignerDialog;

	// Token: 0x0400005A RID: 90
	public int MaxStars = 25;

	// Token: 0x0400005B RID: 91
	public float ShellFuseLengthShort = 3f;

	// Token: 0x0400005C RID: 92
	public float ShellFuseLengthMed = 5.5f;

	// Token: 0x0400005D RID: 93
	public float ShellFuseLengthLong = 8f;

	// Token: 0x0400005E RID: 94
	[NonSerialized]
	public ProtoBuf.PatternFirework.Design Design;

	// Token: 0x0400005F RID: 95
	[NonSerialized]
	public global::PatternFirework.FuseLength ShellFuseLength;

	// Token: 0x02000B5B RID: 2907
	public enum FuseLength
	{
		// Token: 0x04003F48 RID: 16200
		Short,
		// Token: 0x04003F49 RID: 16201
		Medium,
		// Token: 0x04003F4A RID: 16202
		Long,
		// Token: 0x04003F4B RID: 16203
		Max = 2
	}
}
