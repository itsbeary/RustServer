using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008F RID: 143
public class Landmine : BaseTrap
{
	// Token: 0x06000D4D RID: 3405 RVA: 0x00071DB8 File Offset: 0x0006FFB8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Landmine.OnRpcMessage", 0))
		{
			if (rpc == 1552281787U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Disarm ");
				}
				using (TimeWarning.New("RPC_Disarm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1552281787U, "RPC_Disarm", this, player, 3f))
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
							this.RPC_Disarm(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Disarm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x0002A709 File Offset: 0x00028909
	public bool Triggered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0002A700 File Offset: 0x00028900
	public bool Armed()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x00071F20 File Offset: 0x00070120
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = this.triggerPlayerID;
		}
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00071F57 File Offset: 0x00070157
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			this.triggerPlayerID = info.msg.landmine.triggeredID;
		}
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00071F8B File Offset: 0x0007018B
	public override void ServerInit()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.Invoke(new Action(this.Arm), 1.5f);
		base.ServerInit();
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00071FB8 File Offset: 0x000701B8
	public override void ObjectEntered(GameObject obj)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.Armed())
		{
			base.CancelInvoke(new Action(this.Arm));
			this.blocked = true;
			return;
		}
		global::BasePlayer basePlayer = obj.ToBaseEntity() as global::BasePlayer;
		this.Trigger(basePlayer);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00072004 File Offset: 0x00070204
	public void Trigger(global::BasePlayer ply = null)
	{
		if (ply)
		{
			this.triggerPlayerID = ply.userID;
		}
		base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0007202B File Offset: 0x0007022B
	public override void OnEmpty()
	{
		if (this.blocked)
		{
			this.Arm();
			this.blocked = false;
			return;
		}
		if (this.Triggered())
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
		}
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00072064 File Offset: 0x00070264
	public virtual void Explode()
	{
		base.health = float.PositiveInfinity;
		Effect.server.Run(this.explosionEffect.resourcePath, base.PivotPoint(), base.transform.up, null, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 2263296, true);
		if (base.IsDestroyed)
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x000720D9 File Offset: 0x000702D9
	public override void OnKilled(HitInfo info)
	{
		base.Invoke(new Action(this.Explode), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x000720FD File Offset: 0x000702FD
	private void OnGroundMissing()
	{
		this.Explode();
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00072105 File Offset: 0x00070305
	private void TryExplode()
	{
		if (this.Armed())
		{
			this.Explode();
		}
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00072115 File Offset: 0x00070315
	public override void Arm()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00072128 File Offset: 0x00070328
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Disarm(global::BaseEntity.RPCMessage rpc)
	{
		if (rpc.player.userID == this.triggerPlayerID)
		{
			return;
		}
		if (!this.Armed())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (UnityEngine.Random.Range(0, 100) < 15)
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
			return;
		}
		rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, 0UL), global::BaseEntity.GiveItemReason.PickedUp);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0400087F RID: 2175
	public GameObjectRef explosionEffect;

	// Token: 0x04000880 RID: 2176
	public GameObjectRef triggeredEffect;

	// Token: 0x04000881 RID: 2177
	public float minExplosionRadius;

	// Token: 0x04000882 RID: 2178
	public float explosionRadius;

	// Token: 0x04000883 RID: 2179
	public bool blocked;

	// Token: 0x04000884 RID: 2180
	private ulong triggerPlayerID;

	// Token: 0x04000885 RID: 2181
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();
}
