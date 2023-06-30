using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000093 RID: 147
public class LiquidWeapon : BaseLiquidVessel
{
	// Token: 0x06000D8C RID: 3468 RVA: 0x0007342C File Offset: 0x0007162C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidWeapon.OnRpcMessage", 0))
		{
			if (rpc == 1600824953U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PumpWater ");
				}
				using (TimeWarning.New("PumpWater", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1600824953U, "PumpWater", this, player))
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
							this.PumpWater(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in PumpWater");
					}
				}
				return true;
			}
			if (rpc == 3724096303U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartFiring ");
				}
				using (TimeWarning.New("StartFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3724096303U, "StartFiring", this, player))
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
							this.StartFiring(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StartFiring");
					}
				}
				return true;
			}
			if (rpc == 789289044U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopFiring ");
				}
				using (TimeWarning.New("StopFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(789289044U, "StopFiring", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							this.StopFiring();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in StopFiring");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x00073844 File Offset: 0x00071A44
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StartFiring(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.OnCooldown())
		{
			return;
		}
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		if (!this.CanFire(player))
		{
			return;
		}
		base.CancelInvoke("FireTick");
		base.InvokeRepeating("FireTick", 0f, this.FireRate);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		this.StartCooldown(this.FireRate);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x000738C5 File Offset: 0x00071AC5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StopFiring()
	{
		base.CancelInvoke("FireTick");
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00073900 File Offset: 0x00071B00
	private bool CanFire(global::BasePlayer player)
	{
		if (this.RequiresPumping && this.pressure < this.PressureLossPerTick)
		{
			return false;
		}
		if (player == null)
		{
			return false;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Open))
		{
			return false;
		}
		if (base.AmountHeld() <= 0)
		{
			return false;
		}
		if (!player.CanInteract())
		{
			return false;
		}
		if (!player.CanAttack() || player.IsRunning())
		{
			return false;
		}
		global::Item item = this.GetItem();
		return item != null && item.contents != null;
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x00073977 File Offset: 0x00071B77
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void PumpWater(global::BaseEntity.RPCMessage msg)
	{
		this.PumpWater();
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00073980 File Offset: 0x00071B80
	private void PumpWater()
	{
		if (base.GetOwnerPlayer() == null)
		{
			return;
		}
		if (this.OnCooldown())
		{
			return;
		}
		if (this.Firing())
		{
			return;
		}
		this.pressure += this.PressureGainedPerPump;
		this.pressure = Mathf.Min(this.pressure, this.MaxPressure);
		this.StartCooldown(this.PumpingBlockDuration);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000739F8 File Offset: 0x00071BF8
	private void FireTick()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!this.CanFire(ownerPlayer))
		{
			this.StopFiring();
			return;
		}
		int num = Mathf.Min(this.FireAmountML, base.AmountHeld());
		if (num == 0)
		{
			this.StopFiring();
			return;
		}
		base.LoseWater(num);
		float currentRange = this.CurrentRange;
		this.pressure -= this.PressureLossPerTick;
		if (this.pressure <= 0)
		{
			this.StopFiring();
		}
		Ray ray = ownerPlayer.eyes.BodyRay();
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * currentRange, Color.blue, 1f);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(ray, out raycastHit, currentRange, 1218652417))
		{
			this.DoSplash(ownerPlayer, raycastHit.point, ray.direction, num);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00073AD4 File Offset: 0x00071CD4
	private void DoSplash(global::BasePlayer attacker, Vector3 position, Vector3 direction, int amount)
	{
		global::Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		global::Item slot = item.contents.GetSlot(0);
		if (slot == null || slot.amount <= 0)
		{
			return;
		}
		if (slot.info == null)
		{
			return;
		}
		WaterBall.DoSplash(position, this.SplashRadius, slot.info, amount);
		DamageUtil.RadiusDamage(attacker, base.LookupPrefab(), position, this.MinDmgRadius, this.MaxDmgRadius, this.Damage, 131072, true);
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00073B58 File Offset: 0x00071D58
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StopFiring();
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00073B66 File Offset: 0x00071D66
	public float PressureFraction
	{
		get
		{
			return (float)this.pressure / (float)this.MaxPressure;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D96 RID: 3478 RVA: 0x00073B77 File Offset: 0x00071D77
	public float MinimumPressureFraction
	{
		get
		{
			return (float)this.PressureGainedPerPump / (float)this.MaxPressure;
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000D97 RID: 3479 RVA: 0x00073B88 File Offset: 0x00071D88
	public float CurrentRange
	{
		get
		{
			if (!this.UseFalloffCurve)
			{
				return this.MaxRange;
			}
			return this.MaxRange * this.FalloffCurve.Evaluate((float)(this.MaxPressure - this.pressure) / (float)this.MaxPressure);
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00073BC1 File Offset: 0x00071DC1
	private void StartCooldown(float duration)
	{
		if (UnityEngine.Time.realtimeSinceStartup + duration > this.cooldownTime)
		{
			this.cooldownTime = UnityEngine.Time.realtimeSinceStartup + duration;
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00073BDF File Offset: 0x00071DDF
	private bool OnCooldown()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.cooldownTime;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0002A700 File Offset: 0x00028900
	private bool Firing()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x00073BF0 File Offset: 0x00071DF0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.pressure;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00073C44 File Offset: 0x00071E44
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.pressure = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x0400089B RID: 2203
	[Header("Liquid Weapon")]
	public float FireRate = 0.2f;

	// Token: 0x0400089C RID: 2204
	public float MaxRange = 10f;

	// Token: 0x0400089D RID: 2205
	public int FireAmountML = 100;

	// Token: 0x0400089E RID: 2206
	public int MaxPressure = 100;

	// Token: 0x0400089F RID: 2207
	public int PressureLossPerTick = 5;

	// Token: 0x040008A0 RID: 2208
	public int PressureGainedPerPump = 25;

	// Token: 0x040008A1 RID: 2209
	public float MinDmgRadius = 0.15f;

	// Token: 0x040008A2 RID: 2210
	public float MaxDmgRadius = 0.15f;

	// Token: 0x040008A3 RID: 2211
	public float SplashRadius = 2f;

	// Token: 0x040008A4 RID: 2212
	public GameObjectRef ImpactSplashEffect;

	// Token: 0x040008A5 RID: 2213
	public AnimationCurve PowerCurve;

	// Token: 0x040008A6 RID: 2214
	public List<DamageTypeEntry> Damage;

	// Token: 0x040008A7 RID: 2215
	public LiquidWeaponEffects EntityWeaponEffects;

	// Token: 0x040008A8 RID: 2216
	public bool RequiresPumping;

	// Token: 0x040008A9 RID: 2217
	public bool AutoPump;

	// Token: 0x040008AA RID: 2218
	public bool WaitForFillAnim;

	// Token: 0x040008AB RID: 2219
	public bool UseFalloffCurve;

	// Token: 0x040008AC RID: 2220
	public AnimationCurve FalloffCurve;

	// Token: 0x040008AD RID: 2221
	public float PumpingBlockDuration = 0.5f;

	// Token: 0x040008AE RID: 2222
	public float StartFillingBlockDuration = 2f;

	// Token: 0x040008AF RID: 2223
	public float StopFillingBlockDuration = 1f;

	// Token: 0x040008B0 RID: 2224
	private float cooldownTime;

	// Token: 0x040008B1 RID: 2225
	private int pressure;

	// Token: 0x040008B2 RID: 2226
	public const string RadiationFightAchievement = "SUMMER_RADICAL";

	// Token: 0x040008B3 RID: 2227
	public const string SoakedAchievement = "SUMMER_SOAKED";

	// Token: 0x040008B4 RID: 2228
	public const string LiquidatorAchievement = "SUMMER_LIQUIDATOR";

	// Token: 0x040008B5 RID: 2229
	public const string NoPressureAchievement = "SUMMER_NO_PRESSURE";
}
