using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007B RID: 123
public class FreeableLootContainer : LootContainer
{
	// Token: 0x06000B7F RID: 2943 RVA: 0x00066398 File Offset: 0x00064598
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0))
		{
			if (rpc == 2202685945U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_FreeCrate ");
				}
				using (TimeWarning.New("RPC_FreeCrate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2202685945U, "RPC_FreeCrate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_FreeCrate(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_FreeCrate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00066500 File Offset: 0x00064700
	public Rigidbody GetRB()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		return this.rb;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsTiedDown()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00066524 File Offset: 0x00064724
	public override void ServerInit()
	{
		this.GetRB().isKinematic = true;
		this.buoyancy.buoyancyScale = 0f;
		this.buoyancy.enabled = false;
		base.ServerInit();
		if (this.skinOverride != 0U)
		{
			this.skinID = (ulong)this.skinOverride;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0006657C File Offset: 0x0006477C
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.Weapon != null)
		{
			BaseMelee component = info.Weapon.GetComponent<BaseMelee>();
			if (component && component.canUntieCrates && this.IsTiedDown())
			{
				base.health -= 1f;
				info.DidGather = true;
				if (base.health <= 0f)
				{
					base.health = this.MaxHealth();
					this.Release();
				}
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00066604 File Offset: 0x00064804
	public void Release()
	{
		this.GetRB().isKinematic = false;
		this.buoyancy.enabled = true;
		this.buoyancy.buoyancyScale = 1f;
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		if (this.freedEffect.isValid)
		{
			Effect.server.Run(this.freedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00066678 File Offset: 0x00064878
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_FreeCrate(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTiedDown())
		{
			return;
		}
		this.Release();
		BasePlayer player = msg.player;
		if (player)
		{
			player.ProcessMissionEvent(BaseMission.MissionEventType.FREE_CRATE, "", 1f);
			Analytics.Server.FreeUnderwaterCrate();
			Analytics.Azure.OnFreeUnderwaterCrate(msg.player, this);
		}
	}

	// Token: 0x0400077B RID: 1915
	private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;

	// Token: 0x0400077C RID: 1916
	public Buoyancy buoyancy;

	// Token: 0x0400077D RID: 1917
	public GameObjectRef freedEffect;

	// Token: 0x0400077E RID: 1918
	private Rigidbody rb;

	// Token: 0x0400077F RID: 1919
	public uint skinOverride;
}
