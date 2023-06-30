using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BD RID: 189
public class RemoteControlEntity : BaseCombatEntity, IRemoteControllable
{
	// Token: 0x06001103 RID: 4355 RVA: 0x0008C5EC File Offset: 0x0008A7EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06001104 RID: 4356 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanPing
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x0008C754 File Offset: 0x0008A954
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetFovScale()
	{
		return 1f;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0008C75C File Offset: 0x0008A95C
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001109 RID: 4361 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool CanAcceptInput
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x0600110A RID: 4362 RVA: 0x0008C764 File Offset: 0x0008A964
	// (set) Token: 0x0600110B RID: 4363 RVA: 0x0008C76C File Offset: 0x0008A96C
	public int ViewerCount { get; private set; }

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x0600110C RID: 4364 RVA: 0x0008C775 File Offset: 0x0008A975
	// (set) Token: 0x0600110D RID: 4365 RVA: 0x0008C77D File Offset: 0x0008A97D
	public CameraViewerId? ControllingViewerId { get; private set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x0600110E RID: 4366 RVA: 0x0008C788 File Offset: 0x0008A988
	public bool IsBeingControlled
	{
		get
		{
			return this.ViewerCount > 0 && this.ControllingViewerId != null;
		}
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x0008C7B0 File Offset: 0x0008A9B0
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

	// Token: 0x06001110 RID: 4368 RVA: 0x0008C7FC File Offset: 0x0008A9FC
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

	// Token: 0x06001111 RID: 4369 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UserInput(InputState inputState, CameraViewerId viewerID)
	{
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0008C858 File Offset: 0x0008AA58
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0002377F File Offset: 0x0002197F
	public virtual void RCSetup()
	{
		if (base.isServer)
		{
			RemoteControlEntity.InstallControllable(this);
		}
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00023790 File Offset: 0x00021990
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0008C87F File Offset: 0x0008AA7F
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x0008C88D File Offset: 0x0008AA8D
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanControl(ulong playerID)
	{
		return true;
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001118 RID: 4376 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06001119 RID: 4377 RVA: 0x000233FC File Offset: 0x000215FC
	public virtual float MaxRange
	{
		get
		{
			return 10000f;
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x0600111A RID: 4378 RVA: 0x0008C89B File Offset: 0x0008AA9B
	public RemoteControllableControls RequiredControls
	{
		get
		{
			return this.rcControls;
		}
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x0008C8A4 File Offset: 0x0008AAA4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !this.CanControl(msg.player.userID) || !this.CanChangeID(msg.player))
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
			Debug.Log("SetID success!");
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x0008C940 File Offset: 0x0008AB40
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
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

	// Token: 0x0600111E RID: 4382 RVA: 0x0008C9A4 File Offset: 0x0008ABA4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x0008C9F3 File Offset: 0x0008ABF3
	protected virtual bool CanChangeID(global::BasePlayer player)
	{
		return player != null && player.CanBuild() && player.IsBuildingAuthed() && player.IsHoldingEntity<Hammer>();
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x0008CA16 File Offset: 0x0008AC16
	public static bool IDInUse(string id)
	{
		return RemoteControlEntity.FindByID(id) != null;
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0008CA24 File Offset: 0x0008AC24
	public static IRemoteControllable FindByID(string id)
	{
		foreach (IRemoteControllable remoteControllable in RemoteControlEntity.allControllables)
		{
			if (remoteControllable != null && remoteControllable.GetIdentifier() == id)
			{
				return remoteControllable;
			}
		}
		return null;
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x0008CA88 File Offset: 0x0008AC88
	public static bool InstallControllable(IRemoteControllable newControllable)
	{
		if (RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Add(newControllable);
		return true;
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x0008CAA5 File Offset: 0x0008ACA5
	public static bool RemoveControllable(IRemoteControllable newControllable)
	{
		if (!RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Remove(newControllable);
		return true;
	}

	// Token: 0x04000ABB RID: 2747
	public static List<IRemoteControllable> allControllables = new List<IRemoteControllable>();

	// Token: 0x04000ABC RID: 2748
	[Header("RC Entity")]
	public string rcIdentifier = "";

	// Token: 0x04000ABD RID: 2749
	public Transform viewEyes;

	// Token: 0x04000ABE RID: 2750
	public GameObjectRef IDPanelPrefab;

	// Token: 0x04000ABF RID: 2751
	public RemoteControllableControls rcControls;
}
