using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006E RID: 110
public class DudTimedExplosive : global::TimedExplosive, IIgniteable, ISplashable
{
	// Token: 0x06000ABD RID: 2749 RVA: 0x00061E10 File Offset: 0x00060010
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pickup ");
				}
				using (TimeWarning.New("RPC_Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2436818324U, "RPC_Pickup", this, player, 3f))
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
							this.RPC_Pickup(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0002A700 File Offset: 0x00028900
	private bool IsWickBurning()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000ABF RID: 2751 RVA: 0x00061F78 File Offset: 0x00060178
	protected override bool AlwaysRunWaterCheck
	{
		get
		{
			return this.becomeDudInWater;
		}
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00061F80 File Offset: 0x00060180
	public override void WaterCheck()
	{
		if (!this.becomeDudInWater || this.WaterFactor() < 0.5f)
		{
			base.WaterCheck();
			return;
		}
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.WaterCheck)))
		{
			base.CancelInvoke(new Action(this.WaterCheck));
		}
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00062024 File Offset: 0x00060224
	public override float GetRandomTimerTime()
	{
		float randomTimerTime = base.GetRandomTimerTime();
		float num = 1f;
		if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 0.334f;
		}
		else if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 3f;
		}
		return randomTimerTime * num;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0006207C File Offset: 0x0006027C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsWickBurning())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (UnityEngine.Random.Range(0f, 1f) >= 0.5f && base.HasParent())
		{
			this.SetFuse(UnityEngine.Random.Range(2.5f, 3f));
			return;
		}
		player.GiveItem(ItemManager.Create(this.itemToGive, 1, this.skinID), global::BaseEntity.GiveItemReason.Generic);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000620ED File Offset: 0x000602ED
	public override void SetFuse(float fuseLength)
	{
		base.SetFuse(fuseLength);
		this.explodeTime = UnityEngine.Time.realtimeSinceStartup + fuseLength;
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00062128 File Offset: 0x00060328
	public override void Explode()
	{
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		if (UnityEngine.Random.Range(0f, 1f) < this.dudChance)
		{
			this.BecomeDud();
			return;
		}
		base.Explode();
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0006217B File Offset: 0x0006037B
	public override bool CanStickTo(global::BaseEntity entity)
	{
		return base.CanStickTo(entity) && this.IsWickBurning();
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00062190 File Offset: 0x00060390
	public virtual void BecomeDud()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		bool flag = false;
		EntityRef entityRef = this.parentEntity;
		while (entityRef.IsValid(base.isServer) && !flag)
		{
			global::BaseEntity baseEntity = entityRef.Get(base.isServer);
			if (baseEntity.syncPosition)
			{
				flag = true;
			}
			entityRef = baseEntity.parentEntity;
		}
		if (flag)
		{
			base.SetParent(null, false, false);
		}
		base.transform.SetPositionAndRotation(position, rotation);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (flag)
		{
			this.SetMotionEnabled(true);
		}
		Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
		base.Invoke(new Action(base.KillMessage), 1200f);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00062264 File Offset: 0x00060464
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.dudExplosive = Facepunch.Pool.Get<DudExplosive>();
		info.msg.dudExplosive.fuseTimeLeft = this.explodeTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00062299 File Offset: 0x00060499
	public void Ignite(Vector3 fromPos)
	{
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x000622D3 File Offset: 0x000604D3
	public bool CanIgnite()
	{
		return !base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x000622DF File Offset: 0x000604DF
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x000622F2 File Offset: 0x000604F2
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
		return 1;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00062323 File Offset: 0x00060523
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.dudExplosive != null)
		{
			this.explodeTime = UnityEngine.Time.realtimeSinceStartup + info.msg.dudExplosive.fuseTimeLeft;
		}
	}

	// Token: 0x040006FB RID: 1787
	public GameObjectRef fizzleEffect;

	// Token: 0x040006FC RID: 1788
	public GameObject wickSpark;

	// Token: 0x040006FD RID: 1789
	public AudioSource wickSound;

	// Token: 0x040006FE RID: 1790
	public float dudChance = 0.4f;

	// Token: 0x040006FF RID: 1791
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	// Token: 0x04000700 RID: 1792
	[NonSerialized]
	private float explodeTime;

	// Token: 0x04000701 RID: 1793
	public bool becomeDudInWater;
}
