using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004D RID: 77
public class BearTrap : BaseTrap
{
	// Token: 0x0600086C RID: 2156 RVA: 0x000519EC File Offset: 0x0004FBEC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BearTrap.OnRpcMessage", 0))
		{
			if (rpc == 547827602U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Arm ");
				}
				using (TimeWarning.New("RPC_Arm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(547827602U, "RPC_Arm", this, player, 3f))
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
							this.RPC_Arm(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Arm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0002A700 File Offset: 0x00028900
	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00051B54 File Offset: 0x0004FD54
	public override void InitShared()
	{
		this.animator = base.GetComponent<Animator>();
		base.InitShared();
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00051B68 File Offset: 0x0004FD68
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.Armed() && player.CanBuild();
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00051B83 File Offset: 0x0004FD83
	public override void ServerInit()
	{
		base.ServerInit();
		this.Arm();
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00051B91 File Offset: 0x0004FD91
	public override void Arm()
	{
		base.Arm();
		this.RadialResetCorpses(120f);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00051BA4 File Offset: 0x0004FDA4
	public void Fire()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00051BB7 File Offset: 0x0004FDB7
	public override void ObjectEntered(GameObject obj)
	{
		if (!this.Armed())
		{
			return;
		}
		this.hurtTarget = obj;
		base.Invoke(new Action(this.DelayedFire), 0.05f);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00051BE0 File Offset: 0x0004FDE0
	public void DelayedFire()
	{
		if (this.hurtTarget)
		{
			BaseEntity baseEntity = this.hurtTarget.ToBaseEntity();
			if (baseEntity != null)
			{
				HitInfo hitInfo = new HitInfo(this, baseEntity, DamageType.Bite, 50f, base.transform.position);
				hitInfo.damageTypes.Add(DamageType.Stab, 30f);
				baseEntity.OnAttacked(hitInfo);
			}
			this.hurtTarget = null;
		}
		this.RadialResetCorpses(1800f);
		this.Fire();
		base.Hurt(25f);
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00051C68 File Offset: 0x0004FE68
	public void RadialResetCorpses(float duration)
	{
		List<BaseCorpse> list = Facepunch.Pool.GetList<BaseCorpse>();
		global::Vis.Entities<BaseCorpse>(base.transform.position, 5f, list, 512, QueryTriggerInteraction.Collide);
		foreach (BaseCorpse baseCorpse in list)
		{
			baseCorpse.ResetRemovalTime(duration);
		}
		Facepunch.Pool.FreeList<BaseCorpse>(ref list);
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00051CE0 File Offset: 0x0004FEE0
	public override void OnAttacked(HitInfo info)
	{
		float num = info.damageTypes.Total();
		if ((info.damageTypes.IsMeleeType() && num > 20f) || num > 30f)
		{
			this.Fire();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00051D23 File Offset: 0x0004FF23
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Arm(BaseEntity.RPCMessage rpc)
	{
		if (this.Armed())
		{
			return;
		}
		this.Arm();
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00051D34 File Offset: 0x0004FF34
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && this.animator.isInitialized)
		{
			this.animator.SetBool("armed", this.Armed());
		}
	}

	// Token: 0x04000591 RID: 1425
	protected Animator animator;

	// Token: 0x04000592 RID: 1426
	private GameObject hurtTarget;
}
