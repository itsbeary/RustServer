using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000047 RID: 71
public class BasePortal : BaseCombatEntity
{
	// Token: 0x060006E9 RID: 1769 RVA: 0x00047F2C File Offset: 0x0004612C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BasePortal.OnRpcMessage", 0))
		{
			if (rpc == 561762999U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UsePortal ");
				}
				using (TimeWarning.New("RPC_UsePortal", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(561762999U, "RPC_UsePortal", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(561762999U, "RPC_UsePortal", this, player, 3f))
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
							this.RPC_UsePortal(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UsePortal");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x000480EC File Offset: 0x000462EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericEntRef1 = this.targetID;
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0004811B File Offset: 0x0004631B
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.targetID = info.msg.ioEntity.genericEntRef1;
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00048147 File Offset: 0x00046347
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00048150 File Offset: 0x00046350
	public void LinkPortal()
	{
		if (this.targetPortal != null)
		{
			this.targetID = this.targetPortal.net.ID;
		}
		if (this.targetPortal == null && this.targetID.IsValid)
		{
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(this.targetID);
			if (baseNetworkable != null)
			{
				this.targetPortal = baseNetworkable.GetComponent<BasePortal>();
			}
		}
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x000481C2 File Offset: 0x000463C2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Debug.Log("Post server load");
		this.LinkPortal();
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x000481DA File Offset: 0x000463DA
	public void SetDestination(Vector3 destPos, Quaternion destRot)
	{
		this.destination_pos = destPos;
		this.destination_rot = destRot;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x000481EA File Offset: 0x000463EA
	public Vector3 GetLocalEntryExitPosition()
	{
		return this.localEntryExitPos.transform.position;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000481FC File Offset: 0x000463FC
	public Quaternion GetLocalEntryExitRotation()
	{
		return this.localEntryExitPos.transform.rotation;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0004820E File Offset: 0x0004640E
	public BasePortal GetPortal()
	{
		this.LinkPortal();
		return this.targetPortal;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0004821C File Offset: 0x0004641C
	public virtual void UsePortal(global::BasePlayer player)
	{
		this.LinkPortal();
		if (this.targetPortal != null)
		{
			player.PauseFlyHackDetection(1f);
			player.PauseSpeedHackDetection(1f);
			Vector3 position = player.transform.position;
			Vector3 vector = this.targetPortal.GetLocalEntryExitPosition();
			Vector3 vector2 = base.transform.InverseTransformDirection(player.eyes.BodyForward());
			Vector3 vector4;
			if (this.isMirrored)
			{
				Vector3 vector3 = base.transform.InverseTransformPoint(player.transform.position);
				vector = this.targetPortal.relativeAnchor.transform.TransformPoint(vector3);
				vector4 = this.targetPortal.relativeAnchor.transform.TransformDirection(vector2);
			}
			else
			{
				vector4 = this.targetPortal.GetLocalEntryExitRotation() * Vector3.forward;
			}
			if (this.disappearEffect.isValid)
			{
				Effect.server.Run(this.disappearEffect.resourcePath, position, Vector3.up, null, false);
			}
			if (this.appearEffect.isValid)
			{
				Effect.server.Run(this.appearEffect.resourcePath, vector, Vector3.up, null, false);
			}
			player.SetParent(null, true, false);
			player.Teleport(vector);
			player.ForceUpdateTriggers(true, true, true);
			player.ClientRPCPlayer<Vector3>(null, player, "ForceViewAnglesTo", vector4);
			if (this.transitionSoundEffect.isValid)
			{
				Effect.server.Run(this.transitionSoundEffect.resourcePath, this.targetPortal.relativeAnchor.transform.position, Vector3.up, null, false);
			}
			player.UpdateNetworkGroup();
			player.SetPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot, true);
			base.SendNetworkUpdateImmediate(false);
			player.ClientRPCPlayer<bool>(null, player, "StartLoading_Quick", true);
			return;
		}
		Debug.Log("No portal...");
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x000483C8 File Offset: 0x000465C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_UsePortal(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsActive())
		{
			return;
		}
		this.UsePortal(player);
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0000441C File Offset: 0x0000261C
	public bool IsActive()
	{
		return true;
	}

	// Token: 0x0400046A RID: 1130
	public bool isUsablePortal = true;

	// Token: 0x0400046B RID: 1131
	private Vector3 destination_pos;

	// Token: 0x0400046C RID: 1132
	private Quaternion destination_rot;

	// Token: 0x0400046D RID: 1133
	public BasePortal targetPortal;

	// Token: 0x0400046E RID: 1134
	public NetworkableId targetID;

	// Token: 0x0400046F RID: 1135
	public Transform localEntryExitPos;

	// Token: 0x04000470 RID: 1136
	public Transform relativeAnchor;

	// Token: 0x04000471 RID: 1137
	public bool isMirrored = true;

	// Token: 0x04000472 RID: 1138
	public GameObjectRef appearEffect;

	// Token: 0x04000473 RID: 1139
	public GameObjectRef disappearEffect;

	// Token: 0x04000474 RID: 1140
	public GameObjectRef transitionSoundEffect;

	// Token: 0x04000475 RID: 1141
	public string useTagString = "";
}
