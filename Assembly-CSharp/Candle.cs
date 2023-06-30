using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000054 RID: 84
public class Candle : BaseCombatEntity, ISplashable, IIgniteable
{
	// Token: 0x0600092F RID: 2351 RVA: 0x00057E70 File Offset: 0x00056070
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Candle.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
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
							this.SetWantsOn(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00057FD8 File Offset: 0x000561D8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00058001 File Offset: 0x00056201
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0005800F File Offset: 0x0005620F
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.Burn), this.burnRate, this.burnRate, 1f);
			return;
		}
		base.CancelInvoke(new Action(this.Burn));
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00058050 File Offset: 0x00056250
	public void Burn()
	{
		float num = this.burnRate / this.lifeTimeSeconds;
		base.Hurt(num * this.MaxHealth(), DamageType.Decay, this, false);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0005807D File Offset: 0x0005627D
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.damageTypes.Get(DamageType.Heat) > 0f && !base.IsOn())
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateInvokes();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x000580BB File Offset: 0x000562BB
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && amount > 1 && base.IsOn();
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x000580D1 File Offset: 0x000562D1
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (amount > 1)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			this.UpdateInvokes();
			amount--;
		}
		return amount;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x000580ED File Offset: 0x000562ED
	public void Ignite(Vector3 fromPos)
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00050C19 File Offset: 0x0004EE19
	public bool CanIgnite()
	{
		return !base.IsOn();
	}

	// Token: 0x0400061C RID: 1564
	private float lifeTimeSeconds = 7200f;

	// Token: 0x0400061D RID: 1565
	private float burnRate = 10f;
}
