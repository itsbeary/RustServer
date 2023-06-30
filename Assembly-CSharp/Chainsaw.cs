using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005A RID: 90
public class Chainsaw : BaseMelee
{
	// Token: 0x0600099B RID: 2459 RVA: 0x0005A3B4 File Offset: 0x000585B4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Chainsaw.OnRpcMessage", 0))
		{
			if (rpc == 3381353917U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoReload ");
				}
				using (TimeWarning.New("DoReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3381353917U, "DoReload", this, player))
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
							this.DoReload(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoReload");
					}
				}
				return true;
			}
			if (rpc == 706698034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetAttacking ");
				}
				using (TimeWarning.New("Server_SetAttacking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(706698034U, "Server_SetAttacking", this, player))
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
							this.Server_SetAttacking(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_SetAttacking");
					}
				}
				return true;
			}
			if (rpc == 3881794867U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StartEngine ");
				}
				using (TimeWarning.New("Server_StartEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3881794867U, "Server_StartEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StartEngine(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_StartEngine");
					}
				}
				return true;
			}
			if (rpc == 841093980U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopEngine ");
				}
				using (TimeWarning.New("Server_StopEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(841093980U, "Server_StopEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopEngine(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_StopEngine");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002A700 File Offset: 0x00028900
	public bool EngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0002A749 File Offset: 0x00028949
	public bool IsAttacking()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0005A958 File Offset: 0x00058B58
	public void ServerNPCStart()
	{
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			this.DoReload(default(global::BaseEntity.RPCMessage));
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0005A9A4 File Offset: 0x00058BA4
	public override void ServerUse()
	{
		base.ServerUse();
		this.SetAttackStatus(true);
		base.Invoke(new Action(this.DelayedStopAttack), this.attackSpacing + 0.5f);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0005A9D1 File Offset: 0x00058BD1
	public override void ServerUse_OnHit(HitInfo info)
	{
		this.EnableHitEffect(info.HitMaterial);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0005A9DF File Offset: 0x00058BDF
	private void DelayedStopAttack()
	{
		this.SetAttackStatus(false);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0005A9E8 File Offset: 0x00058BE8
	protected override bool VerifyClientAttack(global::BasePlayer player)
	{
		return this.EngineOn() && this.IsAttacking() && base.VerifyClientAttack(player);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0005AA03 File Offset: 0x00058C03
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0005AA12 File Offset: 0x00058C12
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0005AA28 File Offset: 0x00058C28
	public void ReduceAmmo(float firingTime)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			return;
		}
		this.ammoRemainder += firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
		if ((float)this.ammo <= 0f)
		{
			this.SetEngineStatus(false);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0005AAE4 File Offset: 0x00058CE4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void DoReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (this.IsAttacking())
		{
			return;
		}
		global::Item item;
		while (this.ammo < this.maxAmmo && (item = this.GetAmmo()) != null && item.amount > 0)
		{
			int num = Mathf.Min(this.maxAmmo - this.ammo, item.amount);
			this.ammo += num;
			item.UseItem(num);
		}
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0005AB78 File Offset: 0x00058D78
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0005ABCC File Offset: 0x00058DCC
	public void SetEngineStatus(bool status)
	{
		base.SetFlag(global::BaseEntity.Flags.On, status, false, true);
		if (!status)
		{
			this.SetAttackStatus(false);
		}
		base.CancelInvoke(new Action(this.EngineTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.EngineTick), 0f, 1f);
		}
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0005AC20 File Offset: 0x00058E20
	public void SetAttackStatus(bool status)
	{
		if (!this.EngineOn())
		{
			status = false;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, status, false, true);
		base.CancelInvoke(new Action(this.AttackTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.AttackTick), 0f, 1f);
		}
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0005AC77 File Offset: 0x00058E77
	public void EngineTick()
	{
		this.ReduceAmmo(0.05f);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0005AC84 File Offset: 0x00058E84
	public void AttackTick()
	{
		this.ReduceAmmo(this.fuelPerSec);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0005AC94 File Offset: 0x00058E94
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StartEngine(global::BaseEntity.RPCMessage msg)
	{
		if (this.ammo <= 0 || this.EngineOn())
		{
			return;
		}
		this.ReduceAmmo(0.25f);
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.engineStartChance;
		if (!flag)
		{
			this.failedAttempts++;
		}
		if (flag || this.failedAttempts >= 3)
		{
			this.failedAttempts = 0;
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0005AD09 File Offset: 0x00058F09
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StopEngine(global::BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0005AD1C File Offset: 0x00058F1C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_SetAttacking(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (this.IsAttacking() == flag)
		{
			return;
		}
		if (!this.EngineOn())
		{
			return;
		}
		this.SetAttackStatus(flag);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0005AD58 File Offset: 0x00058F58
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo")
		{
			int num = this.ammo;
			if (num > 0)
			{
				this.ammo = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(this.fuelType, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
			}
		}
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0005ADD2 File Offset: 0x00058FD2
	public void DisableHitEffects()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0005AE08 File Offset: 0x00059008
	public void EnableHitEffect(uint hitMaterial)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		if (hitMaterial == StringPool.Get("Flesh"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
		}
		else if (hitMaterial == StringPool.Get("Wood"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
		}
		base.SendNetworkUpdateImmediate(false);
		base.CancelInvoke(new Action(this.DisableHitEffects));
		base.Invoke(new Action(this.DisableHitEffects), 0.5f);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0005AEB7 File Offset: 0x000590B7
	public override void DoAttackShared(HitInfo info)
	{
		base.DoAttackShared(info);
		if (base.isServer)
		{
			this.EnableHitEffect(info.HitMaterial);
		}
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0005AED4 File Offset: 0x000590D4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0005AF22 File Offset: 0x00059122
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0005AF30 File Offset: 0x00059130
	public global::Item GetAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}

	// Token: 0x04000654 RID: 1620
	public float attackFadeInTime = 0.1f;

	// Token: 0x04000655 RID: 1621
	public float attackFadeInDelay = 0.1f;

	// Token: 0x04000656 RID: 1622
	public float attackFadeOutTime = 0.1f;

	// Token: 0x04000657 RID: 1623
	public float idleFadeInTimeFromOff = 0.1f;

	// Token: 0x04000658 RID: 1624
	public float idleFadeInTimeFromAttack = 0.3f;

	// Token: 0x04000659 RID: 1625
	public float idleFadeInDelay = 0.1f;

	// Token: 0x0400065A RID: 1626
	public float idleFadeOutTime = 0.1f;

	// Token: 0x0400065B RID: 1627
	public Renderer chainRenderer;

	// Token: 0x0400065C RID: 1628
	private MaterialPropertyBlock block;

	// Token: 0x0400065D RID: 1629
	private Vector2 saveST;

	// Token: 0x0400065E RID: 1630
	[Header("Chainsaw")]
	public float fuelPerSec = 1f;

	// Token: 0x0400065F RID: 1631
	public int maxAmmo = 100;

	// Token: 0x04000660 RID: 1632
	public int ammo = 100;

	// Token: 0x04000661 RID: 1633
	public ItemDefinition fuelType;

	// Token: 0x04000662 RID: 1634
	public float reloadDuration = 2.5f;

	// Token: 0x04000663 RID: 1635
	[Header("Sounds")]
	public SoundPlayer idleLoop;

	// Token: 0x04000664 RID: 1636
	public SoundPlayer attackLoopAir;

	// Token: 0x04000665 RID: 1637
	public SoundPlayer revUp;

	// Token: 0x04000666 RID: 1638
	public SoundPlayer revDown;

	// Token: 0x04000667 RID: 1639
	public SoundPlayer offSound;

	// Token: 0x04000668 RID: 1640
	private int failedAttempts;

	// Token: 0x04000669 RID: 1641
	public float engineStartChance = 0.33f;

	// Token: 0x0400066A RID: 1642
	private float ammoRemainder;
}
