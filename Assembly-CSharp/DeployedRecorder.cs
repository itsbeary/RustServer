using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000066 RID: 102
public class DeployedRecorder : StorageContainer, ICassettePlayer
{
	// Token: 0x06000A47 RID: 2631 RVA: 0x0005EC54 File Offset: 0x0005CE54
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployedRecorder.OnRpcMessage", 0))
		{
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
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
							this.ServerTogglePlay(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000A48 RID: 2632 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0005EDBC File Offset: 0x0005CFBC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(flag);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0005EDDF File Offset: 0x0005CFDF
	private void ServerTogglePlay(bool play)
	{
		base.SetFlag(BaseEntity.Flags.On, play, false, true);
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0005EDEB File Offset: 0x0005CFEB
	public void OnCassetteInserted(Cassette c)
	{
		base.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0005EE0B File Offset: 0x0005D00B
	public void OnCassetteRemoved(Cassette c)
	{
		base.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0005EE20 File Offset: 0x0005D020
	public override bool ItemFilter(Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = this.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0005EE55 File Offset: 0x0005D055
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (base.isServer)
		{
			this.DoCollisionStick(collision, hitEntity);
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0005EE68 File Offset: 0x0005D068
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0005EE98 File Offset: 0x0005D098
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (this.initialCollisionDetectionMode == null)
			{
				this.initialCollisionDetectionMode = new CollisionDetectionMode?(component.collisionDetectionMode);
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = this.initialCollisionDetectionMode.Value;
			}
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0005EF04 File Offset: 0x0005D104
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider hitCollider)
	{
		if (ent != null && ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (ent != null && base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (hitCollider != null && ent != null)
		{
			base.SetParent(ent, ent.FindBoneID(hitCollider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0005EFD1 File Offset: 0x0005D1D1
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0005EFFF File Offset: 0x0005D1FF
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0005F008 File Offset: 0x0005D208
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0005F034 File Offset: 0x0005D234
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.initialCollisionDetectionMode = null;
		}
	}

	// Token: 0x040006BF RID: 1727
	public AudioSource SoundSource;

	// Token: 0x040006C0 RID: 1728
	public ItemDefinition[] ValidCassettes;

	// Token: 0x040006C1 RID: 1729
	public SoundDefinition PlaySfx;

	// Token: 0x040006C2 RID: 1730
	public SoundDefinition StopSfx;

	// Token: 0x040006C3 RID: 1731
	public SwapKeycard TapeSwapper;

	// Token: 0x040006C4 RID: 1732
	private CollisionDetectionMode? initialCollisionDetectionMode;
}
