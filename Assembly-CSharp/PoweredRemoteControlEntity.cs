using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B5 RID: 181
public class PoweredRemoteControlEntity : global::IOEntity, IRemoteControllable
{
	// Token: 0x06001058 RID: 4184 RVA: 0x00088440 File Offset: 0x00086640
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PoweredRemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetID ");
				}
				using (TimeWarning.New("Server_SetID", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1053317251U, "Server_SetID", this, player, 3f))
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
							this.Server_SetID(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_SetID");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000885A8 File Offset: 0x000867A8
	public bool IsStatic()
	{
		return this.isStatic;
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x0600105A RID: 4186 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x0600105B RID: 4187 RVA: 0x000233FC File Offset: 0x000215FC
	public virtual float MaxRange
	{
		get
		{
			return 10000f;
		}
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x0600105C RID: 4188 RVA: 0x000885B0 File Offset: 0x000867B0
	public RemoteControllableControls RequiredControls
	{
		get
		{
			return this.rcControls;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x0600105D RID: 4189 RVA: 0x000885B8 File Offset: 0x000867B8
	public bool CanPing
	{
		get
		{
			return this.EntityCanPing;
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x0600105E RID: 4190 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool EntityCanPing
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x0600105F RID: 4191 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool CanAcceptInput
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001060 RID: 4192 RVA: 0x000885C0 File Offset: 0x000867C0
	// (set) Token: 0x06001061 RID: 4193 RVA: 0x000885C8 File Offset: 0x000867C8
	public int ViewerCount { get; private set; }

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001062 RID: 4194 RVA: 0x000885D1 File Offset: 0x000867D1
	// (set) Token: 0x06001063 RID: 4195 RVA: 0x000885D9 File Offset: 0x000867D9
	public CameraViewerId? ControllingViewerId { get; private set; }

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001064 RID: 4196 RVA: 0x000885E4 File Offset: 0x000867E4
	public bool IsBeingControlled
	{
		get
		{
			return this.ViewerCount > 0 && this.ControllingViewerId != null;
		}
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x0008860A File Offset: 0x0008680A
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.UpdateRCAccess(this.IsPowered());
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00088620 File Offset: 0x00086820
	public void UpdateRCAccess(bool isOnline)
	{
		if (isOnline)
		{
			RemoteControlEntity.InstallControllable(this);
			return;
		}
		RemoteControlEntity.RemoveControllable(this);
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00088634 File Offset: 0x00086834
	public override void Spawn()
	{
		base.Spawn();
		string text = "#ID";
		if (this.IsStatic() && this.rcIdentifier.Contains(text))
		{
			int num = this.rcIdentifier.IndexOf(text);
			int length = text.Length;
			string text2 = this.rcIdentifier.Substring(0, num);
			text2 += this.net.ID.ToString();
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x000886AC File Offset: 0x000868AC
	public virtual bool InitializeControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount + 1;
		if (this.CanAcceptInput && this.ControllingViewerId == null)
		{
			this.ControllingViewerId = new CameraViewerId?(viewerID);
			return true;
		}
		return !this.CanAcceptInput;
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x000886F8 File Offset: 0x000868F8
	public virtual void StopControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount - 1;
		CameraViewerId? controllingViewerId = this.ControllingViewerId;
		if (controllingViewerId != null && (controllingViewerId == null || controllingViewerId.GetValueOrDefault() == viewerID))
		{
			this.ControllingViewerId = null;
		}
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UserInput(InputState inputState, CameraViewerId viewerID)
	{
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00088754 File Offset: 0x00086954
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float GetFovScale()
	{
		return 1f;
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x0008875C File Offset: 0x0008695C
	public virtual bool CanControl(ulong playerID)
	{
		return this.IsPowered() || this.IsStatic();
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void RCSetup()
	{
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x00023790 File Offset: 0x00021990
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x00088770 File Offset: 0x00086970
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsStatic())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (!this.CanChangeID(player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!string.IsNullOrEmpty(text) && !global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		string text2 = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text2))
		{
			return;
		}
		if (text == this.GetIdentifier())
		{
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x000887E9 File Offset: 0x000869E9
	public override bool CanUseNetworkCache(Connection connection)
	{
		return this.IsStatic() && base.CanUseNetworkCache(connection);
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x000887FC File Offset: 0x000869FC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && !this.IsStatic())
		{
			Connection forConnection = info.forConnection;
			if (!this.CanChangeID(((forConnection != null) ? forConnection.player : null) as global::BasePlayer))
			{
				return;
			}
		}
		info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = this.GetIdentifier();
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00088868 File Offset: 0x00086A68
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x000888B7 File Offset: 0x00086AB7
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			if (!Rust.Application.isLoadingSave)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x000888E5 File Offset: 0x00086AE5
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x000888ED File Offset: 0x00086AED
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000888FB File Offset: 0x00086AFB
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x00088909 File Offset: 0x00086B09
	protected bool CanChangeID(global::BasePlayer player)
	{
		return player != null && player.CanBuild() && player.IsBuildingAuthed();
	}

	// Token: 0x04000A79 RID: 2681
	public string rcIdentifier = "";

	// Token: 0x04000A7A RID: 2682
	public Transform viewEyes;

	// Token: 0x04000A7B RID: 2683
	public GameObjectRef IDPanelPrefab;

	// Token: 0x04000A7C RID: 2684
	public RemoteControllableControls rcControls;

	// Token: 0x04000A7D RID: 2685
	public bool isStatic;

	// Token: 0x04000A7E RID: 2686
	public bool appendEntityIDToIdentifier;
}
