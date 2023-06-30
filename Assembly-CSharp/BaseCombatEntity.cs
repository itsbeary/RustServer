using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003A RID: 58
public class BaseCombatEntity : global::BaseEntity
{
	// Token: 0x0600025C RID: 604 RVA: 0x00028DD8 File Offset: 0x00026FD8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0))
		{
			if (rpc == 1191093595U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickupStart ");
				}
				using (TimeWarning.New("RPC_PickupStart", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1191093595U, "RPC_PickupStart", this, player, 3f))
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
							this.RPC_PickupStart(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_PickupStart");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00028F40 File Offset: 0x00027140
	protected virtual int GetPickupCount()
	{
		return this.pickup.itemCount;
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00028F4D File Offset: 0x0002714D
	public virtual bool CanPickup(global::BasePlayer player)
	{
		return this.pickup.enabled && (!this.pickup.requireBuildingPrivilege || player.CanBuild()) && (!this.pickup.requireHammer || player.IsHoldingEntity<Hammer>());
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00028F8C File Offset: 0x0002718C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_PickupStart(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanPickup(rpc.player))
		{
			return;
		}
		global::Item item = ItemManager.Create(this.pickup.itemTarget, this.GetPickupCount(), this.skinID);
		if (this.pickup.setConditionFromHealth && item.hasCondition)
		{
			item.conditionNormalized = Mathf.Clamp01(this.healthFraction - this.pickup.subtractCondition);
		}
		this.OnPickedUpPreItemMove(item, rpc.player);
		rpc.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		this.OnPickedUp(item, rpc.player);
		Analytics.Azure.OnEntityPickedUp(rpc.player, this);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00029040 File Offset: 0x00027240
	public virtual List<ItemAmount> BuildCost()
	{
		if (this.repair.itemTarget == null)
		{
			return null;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(this.repair.itemTarget);
		if (itemBlueprint == null)
		{
			return null;
		}
		return itemBlueprint.ingredients;
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00029084 File Offset: 0x00027284
	public virtual float RepairCostFraction()
	{
		return 0.5f;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0002908C File Offset: 0x0002728C
	public List<ItemAmount> RepairCost(float healthMissingFraction)
	{
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return null;
		}
		List<ItemAmount> list2 = new List<ItemAmount>();
		foreach (ItemAmount itemAmount in list)
		{
			list2.Add(new ItemAmount(itemAmount.itemDef, (float)Mathf.RoundToInt(itemAmount.amount * this.RepairCostFraction() * healthMissingFraction)));
		}
		RepairBench.StripComponentRepairCost(list2);
		return list2;
	}

	// Token: 0x06000265 RID: 613 RVA: 0x00029114 File Offset: 0x00027314
	public virtual void OnRepair()
	{
		Effect.server.Run(this.repair.repairEffect.isValid ? this.repair.repairEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00029152 File Offset: 0x00027352
	public virtual void OnRepairFinished()
	{
		Effect.server.Run(this.repair.repairFullEffect.isValid ? this.repair.repairFullEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_full.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00029190 File Offset: 0x00027390
	public virtual void OnRepairFailed(global::BasePlayer player, string reason)
	{
		Effect.server.Run(this.repair.repairFailedEffect.isValid ? this.repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		if (player != null && !string.IsNullOrEmpty(reason))
		{
			player.ChatMessage(reason);
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x000291F4 File Offset: 0x000273F4
	public virtual void OnRepairFailedResources(global::BasePlayer player, List<ItemAmount> requirements)
	{
		Effect.server.Run(this.repair.repairFailedEffect.isValid ? this.repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		if (player != null)
		{
			using (ItemAmountList itemAmountList = ItemAmount.SerialiseList(requirements))
			{
				player.ClientRPCPlayer<ItemAmountList>(null, player, "Client_OnRepairFailedResources", itemAmountList);
			}
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00029278 File Offset: 0x00027478
	public virtual void DoRepair(global::BasePlayer player)
	{
		if (!this.repair.enabled)
		{
			return;
		}
		float num = 30f;
		if (this.SecondsSinceAttacked <= num)
		{
			this.OnRepairFailed(player, string.Format("Unable to repair: Recently damaged. Repairable in: {0:N0}s.", num - this.SecondsSinceAttacked));
			return;
		}
		float num2 = this.MaxHealth() - this.Health();
		float num3 = num2 / this.MaxHealth();
		if (num2 <= 0f || num3 <= 0f)
		{
			this.OnRepairFailed(player, "Unable to repair: Not damaged.");
			return;
		}
		List<ItemAmount> list = this.RepairCost(num3);
		if (list == null)
		{
			return;
		}
		float num4 = list.Sum((ItemAmount x) => x.amount);
		float health = this.health;
		if (num4 > 0f)
		{
			float num5 = list.Min((ItemAmount x) => Mathf.Clamp01((float)player.inventory.GetAmount(x.itemid) / x.amount));
			if (float.IsNaN(num5))
			{
				num5 = 0f;
			}
			num5 = Mathf.Min(num5, 50f / num2);
			if (num5 <= 0f)
			{
				this.OnRepairFailedResources(player, list);
				return;
			}
			int num6 = 0;
			foreach (ItemAmount itemAmount in list)
			{
				int num7 = Mathf.CeilToInt(num5 * itemAmount.amount);
				int num8 = player.inventory.Take(null, itemAmount.itemid, num7);
				Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "repair_entity", itemAmount.itemDef.shortname, num8, this, null, false, null, player.userID, null, null, null);
				if (num8 > 0)
				{
					num6 += num8;
					player.Command("note.inv", new object[]
					{
						itemAmount.itemid,
						num8 * -1
					});
				}
			}
			float num9 = (float)num6 / num4;
			this.health += num2 * num9;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		else
		{
			this.health += num2;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		Analytics.Azure.OnEntityRepaired(player, this, health, this.health);
		if (this.Health() >= this.MaxHealth())
		{
			this.OnRepairFinished();
			return;
		}
		this.OnRepair();
	}

	// Token: 0x0600026A RID: 618 RVA: 0x000294E8 File Offset: 0x000276E8
	public virtual void InitializeHealth(float newhealth, float newmax)
	{
		this._maxHealth = newmax;
		this._health = newhealth;
		this.lifestate = BaseCombatEntity.LifeState.Alive;
	}

	// Token: 0x0600026B RID: 619 RVA: 0x000294FF File Offset: 0x000276FF
	public override void ServerInit()
	{
		this.propDirection = PrefabAttribute.server.FindAll<DirectionProperties>(this.prefabID);
		if (this.ResetLifeStateOnSpawn)
		{
			this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		base.ServerInit();
	}

	// Token: 0x0600026C RID: 620 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnHealthChanged(float oldvalue, float newvalue)
	{
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0002953E File Offset: 0x0002773E
	public void Hurt(float amount)
	{
		this.Hurt(Mathf.Abs(amount), DamageType.Generic, null, true);
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00029550 File Offset: 0x00027750
	public void Hurt(float amount, DamageType type, global::BaseEntity attacker = null, bool useProtection = true)
	{
		using (TimeWarning.New("Hurt", 0))
		{
			this.Hurt(new HitInfo(attacker, this, type, amount, base.transform.position)
			{
				UseProtection = useProtection
			});
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x000295AC File Offset: 0x000277AC
	public virtual void Hurt(HitInfo info)
	{
		Assert.IsTrue(base.isServer, "This should be called serverside only");
		if (this.IsDead())
		{
			return;
		}
		using (TimeWarning.New("Hurt( HitInfo )", 50))
		{
			float health = this.health;
			this.ScaleDamage(info);
			if (info.PointStart != Vector3.zero)
			{
				for (int i = 0; i < this.propDirection.Length; i++)
				{
					if (!(this.propDirection[i].extraProtection == null) && !this.propDirection[i].IsWeakspot(base.transform, info))
					{
						this.propDirection[i].extraProtection.Scale(info.damageTypes, 1f);
					}
				}
			}
			info.damageTypes.Scale(DamageType.Arrow, ConVar.Server.arrowdamage);
			info.damageTypes.Scale(DamageType.Bullet, ConVar.Server.bulletdamage);
			info.damageTypes.Scale(DamageType.Slash, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Blunt, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Stab, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Bleeding, ConVar.Server.bleedingdamage);
			if (!(this is global::BasePlayer))
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
			this.DebugHurt(info);
			this.health = health - info.damageTypes.Total();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (ConVar.Global.developer > 1)
			{
				Debug.Log(string.Concat(new object[]
				{
					"[Combat]".PadRight(10),
					base.gameObject.name,
					" hurt ",
					info.damageTypes.GetMajorityDamageType(),
					"/",
					info.damageTypes.Total(),
					" - ",
					this.health.ToString("0"),
					" health left"
				}));
			}
			this.lastDamage = info.damageTypes.GetMajorityDamageType();
			this.lastAttacker = info.Initiator;
			if (this.lastAttacker != null)
			{
				BaseCombatEntity baseCombatEntity = this.lastAttacker as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					baseCombatEntity.lastDealtDamageTime = UnityEngine.Time.time;
					baseCombatEntity.lastDealtDamageTo = this;
				}
			}
			BaseCombatEntity baseCombatEntity2 = this.lastAttacker as BaseCombatEntity;
			if (this.markAttackerHostile && baseCombatEntity2 != null && baseCombatEntity2 != this)
			{
				baseCombatEntity2.MarkHostileFor(60f);
			}
			if (this.lastDamage.IsConsideredAnAttack())
			{
				this.lastAttackedTime = UnityEngine.Time.time;
				if (this.lastAttacker != null)
				{
					this.LastAttackedDir = (this.lastAttacker.transform.position - base.transform.position).normalized;
				}
			}
			bool flag = this.Health() <= 0f;
			Analytics.Azure.OnEntityTakeDamage(info, flag);
			if (flag)
			{
				this.Die(info);
			}
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer)
			{
				if (this.IsDead())
				{
					initiatorPlayer.stats.combat.LogAttack(info, "killed", health);
				}
				else
				{
					initiatorPlayer.stats.combat.LogAttack(info, "", health);
				}
			}
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00029910 File Offset: 0x00027B10
	public virtual bool IsHostile()
	{
		return this.unHostileTime > UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00029920 File Offset: 0x00027B20
	public virtual void MarkHostileFor(float duration = 60f)
	{
		float num = UnityEngine.Time.realtimeSinceStartup + duration;
		this.unHostileTime = Mathf.Max(this.unHostileTime, num);
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00029948 File Offset: 0x00027B48
	private void DebugHurt(HitInfo info)
	{
		if (!ConVar.Vis.damage)
		{
			return;
		}
		if (info.PointStart != info.PointEnd)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", new object[]
			{
				60,
				Color.cyan,
				info.PointStart,
				info.PointEnd,
				0.1f
			});
			ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", new object[]
			{
				60,
				Color.cyan,
				info.HitPositionWorld,
				0.01f
			});
		}
		string text = "";
		for (int i = 0; i < info.damageTypes.types.Length; i++)
		{
			float num = info.damageTypes.types[i];
			if (num != 0f)
			{
				string[] array = new string[5];
				array[0] = text;
				array[1] = " ";
				int num2 = 2;
				DamageType damageType = (DamageType)i;
				array[num2] = damageType.ToString().PadRight(10);
				array[3] = num.ToString("0.00");
				array[4] = "\n";
				text = string.Concat(array);
			}
		}
		string text2 = string.Concat(new object[]
		{
			"<color=lightblue>Damage:</color>".PadRight(10),
			info.damageTypes.Total().ToString("0.00"),
			"\n<color=lightblue>Health:</color>".PadRight(10),
			this.health.ToString("0.00"),
			" / ",
			(this.health - info.damageTypes.Total() <= 0f) ? "<color=red>" : "<color=green>",
			(this.health - info.damageTypes.Total()).ToString("0.00"),
			"</color>",
			"\n<color=lightblue>HitEnt:</color>".PadRight(10),
			this,
			"\n<color=lightblue>HitBone:</color>".PadRight(10),
			info.boneName,
			"\n<color=lightblue>Attacker:</color>".PadRight(10),
			info.Initiator,
			"\n<color=lightblue>WeaponPrefab:</color>".PadRight(10),
			info.WeaponPrefab,
			"\n<color=lightblue>Damages:</color>\n",
			text
		});
		ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
		{
			60,
			Color.white,
			info.HitPositionWorld,
			text2
		});
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00029BE4 File Offset: 0x00027DE4
	public void SetHealth(float hp)
	{
		if (this.health == hp)
		{
			return;
		}
		this.health = hp;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00029C00 File Offset: 0x00027E00
	public virtual void Heal(float amount)
	{
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " healed");
		}
		this.health = this._health + amount;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00029C50 File Offset: 0x00027E50
	public virtual void OnKilled(HitInfo info)
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00029C5C File Offset: 0x00027E5C
	public virtual void Die(HitInfo info = null)
	{
		if (this.IsDead())
		{
			return;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " died");
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		if (info != null && info.InitiatorPlayer)
		{
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer.GetActiveMission() != -1 && !initiatorPlayer.IsNpc)
			{
				initiatorPlayer.ProcessMissionEvent(BaseMission.MissionEventType.KILL_ENTITY, this.prefabID.ToString(), 1f);
			}
		}
		using (TimeWarning.New("OnKilled", 0))
		{
			this.OnKilled(info);
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00029D2C File Offset: 0x00027F2C
	public void DieInstantly()
	{
		if (this.IsDead())
		{
			return;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " died");
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		this.OnKilled(null);
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00029D8C File Offset: 0x00027F8C
	public void UpdateSurroundings()
	{
		global::StabilityEntity.updateSurroundingsQueue.Add(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000279 RID: 633 RVA: 0x00029DB1 File Offset: 0x00027FB1
	public float TimeSinceLastNoise
	{
		get
		{
			return UnityEngine.Time.time - this.lastNoiseTime;
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x0600027A RID: 634 RVA: 0x00029DBF File Offset: 0x00027FBF
	// (set) Token: 0x0600027B RID: 635 RVA: 0x00029DC7 File Offset: 0x00027FC7
	public BaseCombatEntity.ActionVolume LastNoiseVolume { get; private set; }

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x0600027C RID: 636 RVA: 0x00029DD0 File Offset: 0x00027FD0
	// (set) Token: 0x0600027D RID: 637 RVA: 0x00029DD8 File Offset: 0x00027FD8
	public Vector3 LastNoisePosition { get; private set; }

	// Token: 0x0600027E RID: 638 RVA: 0x00029DE1 File Offset: 0x00027FE1
	public void MakeNoise(Vector3 position, BaseCombatEntity.ActionVolume loudness)
	{
		this.LastNoisePosition = position;
		this.LastNoiseVolume = loudness;
		this.lastNoiseTime = UnityEngine.Time.time;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00029DFC File Offset: 0x00027FFC
	public bool CanLastNoiseBeHeard(Vector3 listenPosition, float listenRange)
	{
		return listenRange > 0f && Vector3.Distance(listenPosition, this.LastNoisePosition) <= listenRange;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00029E1A File Offset: 0x0002801A
	public virtual bool IsDead()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Dead;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00029E25 File Offset: 0x00028025
	public virtual bool IsAlive()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Alive;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00029E30 File Offset: 0x00028030
	public BaseCombatEntity.Faction GetFaction()
	{
		return this.faction;
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsFriendly(BaseCombatEntity other)
	{
		return false;
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000284 RID: 644 RVA: 0x00029E38 File Offset: 0x00028038
	// (set) Token: 0x06000285 RID: 645 RVA: 0x00029E40 File Offset: 0x00028040
	public Vector3 LastAttackedDir { get; set; }

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000286 RID: 646 RVA: 0x00029E49 File Offset: 0x00028049
	public float SecondsSinceAttacked
	{
		get
		{
			return UnityEngine.Time.time - this.lastAttackedTime;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000287 RID: 647 RVA: 0x00029E57 File Offset: 0x00028057
	public float SecondsSinceDealtDamage
	{
		get
		{
			return UnityEngine.Time.time - this.lastDealtDamageTime;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000288 RID: 648 RVA: 0x00029E65 File Offset: 0x00028065
	public float healthFraction
	{
		get
		{
			return this.Health() / this.MaxHealth();
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00029E74 File Offset: 0x00028074
	public override void ResetState()
	{
		base.ResetState();
		this.health = this.MaxHealth();
		if (base.isServer)
		{
			this.lastAttackedTime = float.NegativeInfinity;
			this.lastDealtDamageTime = float.NegativeInfinity;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00029EA6 File Offset: 0x000280A6
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateSurroundings();
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float GetThreatLevel()
	{
		return 0f;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00029EC3 File Offset: 0x000280C3
	public override float PenetrationResistance(HitInfo info)
	{
		if (!this.baseProtection)
		{
			return 100f;
		}
		return this.baseProtection.density;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00029EE3 File Offset: 0x000280E3
	public virtual void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection && this.baseProtection != null)
		{
			this.baseProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00029F14 File Offset: 0x00028114
	public HitArea SkeletonLookup(uint boneID)
	{
		if (this.skeletonProperties == null)
		{
			return (HitArea)(-1);
		}
		SkeletonProperties.BoneProperty boneProperty = this.skeletonProperties.FindBone(boneID);
		if (boneProperty == null)
		{
			return (HitArea)(-1);
		}
		return boneProperty.area;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00029F4C File Offset: 0x0002814C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseCombat = Facepunch.Pool.Get<BaseCombat>();
		info.msg.baseCombat.state = (int)this.lifestate;
		info.msg.baseCombat.health = this.Health();
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00029F9C File Offset: 0x0002819C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Health() > this.MaxHealth())
		{
			this.health = this.MaxHealth();
		}
		if (float.IsNaN(this.Health()))
		{
			this.health = this.MaxHealth();
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x00029FD8 File Offset: 0x000281D8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (base.isServer)
		{
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		if (info.msg.baseCombat != null)
		{
			this.lifestate = (BaseCombatEntity.LifeState)info.msg.baseCombat.state;
			this._health = info.msg.baseCombat.health;
		}
		base.Load(info);
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000292 RID: 658 RVA: 0x0002A034 File Offset: 0x00028234
	// (set) Token: 0x06000293 RID: 659 RVA: 0x0002A03C File Offset: 0x0002823C
	public float health
	{
		get
		{
			return this._health;
		}
		set
		{
			float health = this._health;
			this._health = Mathf.Clamp(value, 0f, this.MaxHealth());
			if (base.isServer && this._health != health)
			{
				this.OnHealthChanged(health, this._health);
			}
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0002A034 File Offset: 0x00028234
	public override float Health()
	{
		return this._health;
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0002A085 File Offset: 0x00028285
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0002A08D File Offset: 0x0002828D
	public virtual float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0002A095 File Offset: 0x00028295
	public virtual float StartMaxHealth()
	{
		return this.StartHealth();
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0002A09D File Offset: 0x0002829D
	public void SetMaxHealth(float newMax)
	{
		this._maxHealth = newMax;
		this._health = Mathf.Min(this._health, newMax);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0002A0B8 File Offset: 0x000282B8
	public void DoHitNotify(HitInfo info)
	{
		using (TimeWarning.New("DoHitNotify", 0))
		{
			if (this.sendsHitNotification && !(info.Initiator == null) && info.Initiator is global::BasePlayer && !(this == info.Initiator))
			{
				if (!info.isHeadshot || !(info.HitEntity is global::BasePlayer))
				{
					if (UnityEngine.Time.frameCount != this.lastNotifyFrame)
					{
						this.lastNotifyFrame = UnityEngine.Time.frameCount;
						bool flag = info.Weapon is BaseMelee;
						if (base.isServer && (!flag || this.sendsMeleeHitNotification))
						{
							bool flag2 = info.Initiator.net.connection == info.Predicted;
							base.ClientRPCPlayerAndSpectators<bool>(null, info.Initiator as global::BasePlayer, "HitNotify", flag2);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0002A1A8 File Offset: 0x000283A8
	public override void OnAttacked(HitInfo info)
	{
		using (TimeWarning.New("BaseCombatEntity.OnAttacked", 0))
		{
			if (!this.IsDead())
			{
				this.DoHitNotify(info);
			}
			if (base.isServer)
			{
				this.Hurt(info);
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x04000230 RID: 560
	private const float MAX_HEALTH_REPAIR = 50f;

	// Token: 0x04000231 RID: 561
	[NonSerialized]
	public DamageType lastDamage;

	// Token: 0x04000232 RID: 562
	[NonSerialized]
	public global::BaseEntity lastAttacker;

	// Token: 0x04000233 RID: 563
	public global::BaseEntity lastDealtDamageTo;

	// Token: 0x04000234 RID: 564
	[NonSerialized]
	public bool ResetLifeStateOnSpawn = true;

	// Token: 0x04000235 RID: 565
	protected DirectionProperties[] propDirection;

	// Token: 0x04000236 RID: 566
	protected float unHostileTime;

	// Token: 0x04000239 RID: 569
	private float lastNoiseTime;

	// Token: 0x0400023A RID: 570
	[Header("BaseCombatEntity")]
	public SkeletonProperties skeletonProperties;

	// Token: 0x0400023B RID: 571
	public ProtectionProperties baseProtection;

	// Token: 0x0400023C RID: 572
	public float startHealth;

	// Token: 0x0400023D RID: 573
	public BaseCombatEntity.Pickup pickup;

	// Token: 0x0400023E RID: 574
	public BaseCombatEntity.Repair repair;

	// Token: 0x0400023F RID: 575
	public bool ShowHealthInfo = true;

	// Token: 0x04000240 RID: 576
	public BaseCombatEntity.LifeState lifestate;

	// Token: 0x04000241 RID: 577
	public bool sendsHitNotification;

	// Token: 0x04000242 RID: 578
	public bool sendsMeleeHitNotification = true;

	// Token: 0x04000243 RID: 579
	public bool markAttackerHostile = true;

	// Token: 0x04000244 RID: 580
	protected float _health;

	// Token: 0x04000245 RID: 581
	protected float _maxHealth = 100f;

	// Token: 0x04000246 RID: 582
	public BaseCombatEntity.Faction faction;

	// Token: 0x04000247 RID: 583
	[NonSerialized]
	public float lastAttackedTime = float.NegativeInfinity;

	// Token: 0x04000249 RID: 585
	[NonSerialized]
	public float lastDealtDamageTime = float.NegativeInfinity;

	// Token: 0x0400024A RID: 586
	private int lastNotifyFrame;

	// Token: 0x02000B7D RID: 2941
	[Serializable]
	public struct Pickup
	{
		// Token: 0x04003F98 RID: 16280
		public bool enabled;

		// Token: 0x04003F99 RID: 16281
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		// Token: 0x04003F9A RID: 16282
		public int itemCount;

		// Token: 0x04003F9B RID: 16283
		[Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
		public bool setConditionFromHealth;

		// Token: 0x04003F9C RID: 16284
		[Tooltip("How much to reduce the item condition when picking up")]
		public float subtractCondition;

		// Token: 0x04003F9D RID: 16285
		[Tooltip("Must have building access to pick up")]
		public bool requireBuildingPrivilege;

		// Token: 0x04003F9E RID: 16286
		[Tooltip("Must have hammer equipped to pick up")]
		public bool requireHammer;

		// Token: 0x04003F9F RID: 16287
		[Tooltip("Inventory Must be empty (if applicable) to be picked up")]
		public bool requireEmptyInv;
	}

	// Token: 0x02000B7E RID: 2942
	[Serializable]
	public struct Repair
	{
		// Token: 0x04003FA0 RID: 16288
		public bool enabled;

		// Token: 0x04003FA1 RID: 16289
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		// Token: 0x04003FA2 RID: 16290
		public GameObjectRef repairEffect;

		// Token: 0x04003FA3 RID: 16291
		public GameObjectRef repairFullEffect;

		// Token: 0x04003FA4 RID: 16292
		public GameObjectRef repairFailedEffect;
	}

	// Token: 0x02000B7F RID: 2943
	public enum ActionVolume
	{
		// Token: 0x04003FA6 RID: 16294
		Quiet,
		// Token: 0x04003FA7 RID: 16295
		Normal,
		// Token: 0x04003FA8 RID: 16296
		Loud
	}

	// Token: 0x02000B80 RID: 2944
	public enum LifeState
	{
		// Token: 0x04003FAA RID: 16298
		Alive,
		// Token: 0x04003FAB RID: 16299
		Dead
	}

	// Token: 0x02000B81 RID: 2945
	[Serializable]
	public enum Faction
	{
		// Token: 0x04003FAD RID: 16301
		Default,
		// Token: 0x04003FAE RID: 16302
		Player,
		// Token: 0x04003FAF RID: 16303
		Bandit,
		// Token: 0x04003FB0 RID: 16304
		Scientist,
		// Token: 0x04003FB1 RID: 16305
		Horror
	}
}
