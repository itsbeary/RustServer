using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000048 RID: 72
public class BaseProjectile : AttackEntity
{
	// Token: 0x060006F7 RID: 1783 RVA: 0x00048410 File Offset: 0x00046610
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseProjectile.OnRpcMessage", 0))
		{
			if (rpc == 3168282921U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CLProject ");
				}
				using (TimeWarning.New("CLProject", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3168282921U, "CLProject", this, player))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3168282921U, "CLProject", this, player))
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
							this.CLProject(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in CLProject");
					}
				}
				return true;
			}
			if (rpc == 1720368164U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Reload ");
				}
				using (TimeWarning.New("Reload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1720368164U, "Reload", this, player))
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
							this.Reload(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Reload");
					}
				}
				return true;
			}
			if (rpc == 240404208U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerFractionalReloadInsert ");
				}
				using (TimeWarning.New("ServerFractionalReloadInsert", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(240404208U, "ServerFractionalReloadInsert", this, player))
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
							this.ServerFractionalReloadInsert(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ServerFractionalReloadInsert");
					}
				}
				return true;
			}
			if (rpc == 555589155U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartReload ");
				}
				using (TimeWarning.New("StartReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(555589155U, "StartReload", this, player))
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
							this.StartReload(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in StartReload");
					}
				}
				return true;
			}
			if (rpc == 1918419884U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SwitchAmmoTo ");
				}
				using (TimeWarning.New("SwitchAmmoTo", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1918419884U, "SwitchAmmoTo", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SwitchAmmoTo(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in SwitchAmmoTo");
					}
				}
				return true;
			}
			if (rpc == 3327286961U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleFireMode ");
				}
				using (TimeWarning.New("ToggleFireMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3327286961U, "ToggleFireMode", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3327286961U, "ToggleFireMode", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleFireMode(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in ToggleFireMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060006F8 RID: 1784 RVA: 0x00048C94 File Offset: 0x00046E94
	public RecoilProperties recoilProperties
	{
		get
		{
			if (!(this.recoil == null))
			{
				return this.recoil.GetRecoil();
			}
			return null;
		}
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00048CB1 File Offset: 0x00046EB1
	public override Vector3 GetInheritedVelocity(global::BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedProjectileVelocity(direction);
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00048CBA File Offset: 0x00046EBA
	public virtual float GetDamageScale(bool getMax = false)
	{
		return this.damageScale;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00048CC2 File Offset: 0x00046EC2
	public virtual float GetDistanceScale(bool getMax = false)
	{
		return this.distanceScale;
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00048CCA File Offset: 0x00046ECA
	public virtual float GetProjectileVelocityScale(bool getMax = false)
	{
		return this.projectileVelocityScale;
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00048CD2 File Offset: 0x00046ED2
	protected void StartReloadCooldown(float cooldown)
	{
		this.nextReloadTime = base.CalculateCooldownTime(this.nextReloadTime, cooldown, false);
		this.startReloadTime = this.nextReloadTime - cooldown;
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00048CF6 File Offset: 0x00046EF6
	protected void ResetReloadCooldown()
	{
		this.nextReloadTime = float.NegativeInfinity;
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00048D03 File Offset: 0x00046F03
	protected bool HasReloadCooldown()
	{
		return UnityEngine.Time.time < this.nextReloadTime;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00048D12 File Offset: 0x00046F12
	protected float GetReloadCooldown()
	{
		return Mathf.Max(this.nextReloadTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00048D2A File Offset: 0x00046F2A
	protected float GetReloadIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextReloadTime, 0f);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00048D44 File Offset: 0x00046F44
	private void OnDrawGizmos()
	{
		if (!base.isClient)
		{
			return;
		}
		if (this.MuzzlePoint != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + this.MuzzlePoint.forward * 10f);
			global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
			if (ownerPlayer)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + ownerPlayer.eyes.rotation * Vector3.forward * 10f);
			}
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00048E02 File Offset: 0x00047002
	public virtual RecoilProperties GetRecoil()
	{
		return this.recoilProperties;
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000704 RID: 1796 RVA: 0x00048E0A File Offset: 0x0004700A
	public bool isSemiAuto
	{
		get
		{
			return !this.automatic;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000705 RID: 1797 RVA: 0x00048E15 File Offset: 0x00047015
	public override bool IsUsableByTurret
	{
		get
		{
			return this.usableByTurret;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000706 RID: 1798 RVA: 0x00048E1D File Offset: 0x0004701D
	public override Transform MuzzleTransform
	{
		get
		{
			return this.MuzzlePoint;
		}
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DidAttackServerside()
	{
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00048E25 File Offset: 0x00047025
	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadTime;
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00048E3B File Offset: 0x0004703B
	public override bool CanReload()
	{
		return this.primaryMagazine.contents < this.primaryMagazine.capacity;
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00048E55 File Offset: 0x00047055
	public override float AmmoFraction()
	{
		return (float)this.primaryMagazine.contents / (float)this.primaryMagazine.capacity;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00048E70 File Offset: 0x00047070
	public override void TopUpAmmo()
	{
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00048E88 File Offset: 0x00047088
	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadTime);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00048ED8 File Offset: 0x000470D8
	public override Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		bool flag = false;
		float num = UnityEngine.Time.time * (this.aimSwaySpeed * 1f + this.aiAimSwayOffset);
		float num2 = Mathf.Sin(UnityEngine.Time.time * 2f);
		float num3 = ((num2 < 0f) ? (1f - Mathf.Clamp(Mathf.Abs(num2) / 1f, 0f, 1f)) : 1f);
		float num4 = (flag ? 0.6f : 1f);
		float num5 = (this.aimSway * 1f + this.aiAimSwayOffset) * num4 * num3 * swayModifier;
		eulerInput.y += (Mathf.PerlinNoise(num, num) - 0.5f) * num5 * UnityEngine.Time.deltaTime;
		eulerInput.x += (Mathf.PerlinNoise(num + 0.1f, num + 0.2f) - 0.5f) * num5 * UnityEngine.Time.deltaTime;
		return eulerInput;
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00048FBC File Offset: 0x000471BC
	public float GetAIAimcone()
	{
		NPCPlayer npcplayer = base.GetOwnerPlayer() as NPCPlayer;
		if (npcplayer)
		{
			return npcplayer.GetAimConeScale() * this.aiAimCone;
		}
		return this.aiAimCone;
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00031A4C File Offset: 0x0002FC4C
	public override void ServerUse()
	{
		this.ServerUse(1f, null);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00048FF4 File Offset: 0x000471F4
	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		if (base.isClient)
		{
			return;
		}
		if (base.HasAttackCooldown())
		{
			return;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = ownerPlayer != null;
		if (this.primaryMagazine.contents <= 0)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.DryFire, null);
			base.StartAttackCooldownRaw(1f);
			return;
		}
		this.primaryMagazine.contents--;
		if (this.primaryMagazine.contents < 0)
		{
			this.primaryMagazine.contents = 0;
		}
		bool flag2 = flag && ownerPlayer.IsNpc;
		if (flag2 && (ownerPlayer.isMounted || ownerPlayer.GetParentEntity() != null))
		{
			NPCPlayer npcplayer = ownerPlayer as NPCPlayer;
			if (npcplayer != null)
			{
				npcplayer.SetAimDirection(npcplayer.GetAimDirection());
			}
		}
		base.StartAttackCooldownRaw(this.repeatDelay);
		Vector3 vector = (flag ? ownerPlayer.eyes.position : this.MuzzlePoint.transform.position);
		Vector3 vector2 = this.MuzzlePoint.transform.forward;
		if (originOverride != null)
		{
			vector = originOverride.position;
			vector2 = originOverride.forward;
		}
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, string.Empty, null);
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		global::BaseEntity baseEntity = null;
		if (flag)
		{
			vector2 = ownerPlayer.eyes.BodyForward();
		}
		for (int i = 0; i < component.numProjectiles; i++)
		{
			Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(component.projectileSpread + this.GetAimCone() + this.GetAIAimcone() * 1f, vector2, true);
			List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0f, list, 300f, 1220225793, QueryTriggerInteraction.UseGlobal, null);
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit raycastHit = list[j];
				global::BaseEntity entity = raycastHit.GetEntity();
				if ((!(entity != null) || (!(entity == this) && !entity.EqualNetID(this))) && (!(entity != null) || !entity.isClient))
				{
					ColliderInfo component3 = raycastHit.collider.GetComponent<ColliderInfo>();
					if (!(component3 != null) || component3.HasFlag(ColliderInfo.Flags.Shootable))
					{
						BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
						if ((!(entity != null) || !entity.IsNpc || !flag2 || baseCombatEntity.GetFaction() == BaseCombatEntity.Faction.Horror || entity is BasePet) && baseCombatEntity != null && (baseEntity == null || entity == baseEntity || entity.EqualNetID(baseEntity)))
						{
							HitInfo hitInfo = new HitInfo();
							this.AssignInitiator(hitInfo);
							hitInfo.Weapon = this;
							hitInfo.WeaponPrefab = base.gameManager.FindPrefab(base.PrefabName).GetComponent<AttackEntity>();
							hitInfo.IsPredicting = false;
							hitInfo.DoHitEffects = component2.doDefaultHitEffects;
							hitInfo.DidHit = true;
							hitInfo.ProjectileVelocity = modifiedAimConeDirection * 300f;
							hitInfo.PointStart = this.MuzzlePoint.position;
							hitInfo.PointEnd = raycastHit.point;
							hitInfo.HitPositionWorld = raycastHit.point;
							hitInfo.HitNormalWorld = raycastHit.normal;
							hitInfo.HitEntity = entity;
							hitInfo.UseProtection = true;
							component2.CalculateDamage(hitInfo, this.GetProjectileModifier(), 1f);
							hitInfo.damageTypes.ScaleAll(this.GetDamageScale(false) * damageModifier * (flag2 ? this.npcDamageScale : this.turretDamageScale));
							baseCombatEntity.OnAttacked(hitInfo);
							component.ServerProjectileHit(hitInfo);
							if (entity is global::BasePlayer || entity is BaseNpc)
							{
								hitInfo.HitPositionLocal = entity.transform.InverseTransformPoint(hitInfo.HitPositionWorld);
								hitInfo.HitNormalLocal = entity.transform.InverseTransformDirection(hitInfo.HitNormalWorld);
								hitInfo.HitMaterial = StringPool.Get("Flesh");
								Effect.server.ImpactEffect(hitInfo);
							}
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
				}
			}
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			Vector3 vector3 = ((flag && ownerPlayer.isMounted) ? (modifiedAimConeDirection * 6f) : Vector3.zero);
			this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, vector + vector3, modifiedAimConeDirection * component.projectileVelocity, UnityEngine.Random.Range(1, 100), null, this.IsSilenced(), true);
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00049489 File Offset: 0x00047689
	private void AssignInitiator(HitInfo info)
	{
		info.Initiator = base.GetOwnerPlayer();
		if (info.Initiator == null)
		{
			info.Initiator = base.GetParentEntity();
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000494B1 File Offset: 0x000476B1
	public override void ServerInit()
	{
		base.ServerInit();
		this.primaryMagazine.ServerInit();
		base.Invoke(new Action(this.DelayedModSetup), 0.1f);
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x000494DC File Offset: 0x000476DC
	public void DelayedModSetup()
	{
		if (this.modsChangedInitialized)
		{
			return;
		}
		global::Item cachedItem = base.GetCachedItem();
		if (cachedItem != null && cachedItem.contents != null)
		{
			global::ItemContainer contents = cachedItem.contents;
			contents.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(contents.onItemAddedRemoved, new Action<global::Item, bool>(this.ModsChanged));
			this.modsChangedInitialized = true;
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00049534 File Offset: 0x00047734
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			global::Item cachedItem = base.GetCachedItem();
			if (cachedItem != null && cachedItem.contents != null)
			{
				global::ItemContainer contents = cachedItem.contents;
				contents.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Remove(contents.onItemAddedRemoved, new Action<global::Item, bool>(this.ModsChanged));
				this.modsChangedInitialized = false;
			}
		}
		base.DestroyShared();
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0004958F File Offset: 0x0004778F
	public void ModsChanged(global::Item item, bool added)
	{
		base.Invoke(new Action(this.DelayedModsChanged), 0.1f);
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000495A8 File Offset: 0x000477A8
	public void ForceModsChanged()
	{
		base.Invoke(new Action(this.DelayedModSetup), 0f);
		base.Invoke(new Action(this.DelayedModsChanged), 0.2f);
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x000495D8 File Offset: 0x000477D8
	public void DelayedModsChanged()
	{
		int num = Mathf.CeilToInt(ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.magazineCapacity, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * (float)this.primaryMagazine.definition.builtInSize);
		if (num == this.primaryMagazine.capacity)
		{
			return;
		}
		if (this.primaryMagazine.contents > 0 && this.primaryMagazine.contents > num)
		{
			ItemDefinition ammoType = this.primaryMagazine.ammoType;
			int contents = this.primaryMagazine.contents;
			global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
			global::ItemContainer itemContainer = null;
			if (ownerPlayer != null)
			{
				itemContainer = ownerPlayer.inventory.containerMain;
			}
			else if (base.GetCachedItem() != null)
			{
				itemContainer = base.GetCachedItem().parent;
			}
			this.primaryMagazine.contents = 0;
			if (itemContainer != null)
			{
				global::Item item = ItemManager.Create(this.primaryMagazine.ammoType, contents, 0UL);
				if (!item.MoveToContainer(itemContainer, -1, true, false, null, true))
				{
					Vector3 vector = base.transform.position;
					if (itemContainer.entityOwner != null)
					{
						vector = itemContainer.entityOwner.transform.position + Vector3.up * 0.25f;
					}
					item.Drop(vector, Vector3.up * 5f, default(Quaternion));
				}
			}
		}
		this.primaryMagazine.capacity = num;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00049770 File Offset: 0x00047970
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo" && !this.HasReloadCooldown())
		{
			this.UnloadAmmo(item, player);
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00049794 File Offset: 0x00047994
	public void UnloadAmmo(global::Item item, global::BasePlayer player)
	{
		global::BaseProjectile component = item.GetHeldEntity().GetComponent<global::BaseProjectile>();
		if (!component.canUnloadAmmo)
		{
			return;
		}
		if (component)
		{
			int contents = component.primaryMagazine.contents;
			if (contents > 0)
			{
				component.primaryMagazine.contents = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(component.primaryMagazine.ammoType, contents, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
			}
		}
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00049829 File Offset: 0x00047A29
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		this.UnloadAmmo(item, crafter);
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00049840 File Offset: 0x00047A40
	public override void ReturnedFromCancelledCraft(global::Item item, global::BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		global::BaseProjectile component = item.GetHeldEntity().GetComponent<global::BaseProjectile>();
		if (component)
		{
			component.primaryMagazine.contents = 0;
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0004987A File Offset: 0x00047A7A
	public override void SetLightsOn(bool isOn)
	{
		base.SetLightsOn(isOn);
		this.UpdateAttachmentsState();
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0004988C File Offset: 0x00047A8C
	public void UpdateAttachmentsState()
	{
		global::BaseEntity.Flags flags = this.flags;
		bool flag = this.ShouldLightsBeOn();
		if (this.children != null)
		{
			foreach (ProjectileWeaponMod projectileWeaponMod in from ProjectileWeaponMod x in this.children
				where x != null && x.isLight
				select x)
			{
				projectileWeaponMod.SetFlag(global::BaseEntity.Flags.On, flag, false, true);
			}
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0004991C File Offset: 0x00047B1C
	private bool ShouldLightsBeOn()
	{
		return base.LightsOn() && (base.IsDeployed() || this.parentEntity.Get(base.isServer) is global::AutoTurret);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0004994C File Offset: 0x00047B4C
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		ProjectileWeaponMod projectileWeaponMod;
		if ((projectileWeaponMod = child as ProjectileWeaponMod) != null && projectileWeaponMod.isLight)
		{
			child.SetFlag(global::BaseEntity.Flags.On, false, false, true);
			this.SetLightsOn(false);
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanAiAttack()
	{
		return true;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00049984 File Offset: 0x00047B84
	public virtual float GetAimCone()
	{
		uint num = 0U;
		foreach (global::BaseEntity baseEntity in this.children)
		{
			num += (uint)baseEntity.net.ID.Value;
			num = (uint)((int)num + baseEntity.flags);
		}
		uint num2 = CRC.Compute32(0U, num);
		if (num2 != this.cachedModHash)
		{
			this.sightAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			this.sightAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			this.hipAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			this.hipAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			this.cachedModHash = num2;
		}
		float num3 = this.aimCone;
		num3 *= (this.UsingInternalBurstMode() ? this.internalBurstAimConeScale : 1f);
		if (this.recoilProperties != null && this.recoilProperties.overrideAimconeWithCurve && this.primaryMagazine.capacity > 0)
		{
			num3 += this.recoilProperties.aimconeCurve.Evaluate((float)this.numShotsFired / (float)this.primaryMagazine.capacity % 1f) * this.recoilProperties.aimconeCurveScale;
			this.aimconePenalty = 0f;
		}
		if (this.aiming || base.isServer)
		{
			return (num3 + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * this.sightAimConeScale + this.sightAimConeOffset;
		}
		return (num3 + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * this.sightAimConeScale + this.sightAimConeOffset + this.hipAimCone * this.hipAimConeScale + this.hipAimConeOffset;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00049C44 File Offset: 0x00047E44
	public float ScaleRepeatDelay(float delay)
	{
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		float num3 = (this.UsingInternalBurstMode() ? this.internalBurstFireRateScale : 1f);
		return delay * num * num3 + num2;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00049D04 File Offset: 0x00047F04
	public Projectile.Modifier GetProjectileModifier()
	{
		Projectile.Modifier modifier = default(Projectile.Modifier);
		modifier.damageOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		modifier.damageScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDamageScale(false);
		modifier.distanceOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		modifier.distanceScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDistanceScale(false);
		return modifier;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00049E6C File Offset: 0x0004806C
	public bool UsingBurstMode()
	{
		if (this.IsBurstDisabled())
		{
			return false;
		}
		if (this.isBurstWeapon)
		{
			return true;
		}
		if (this.children != null)
		{
			return (from ProjectileWeaponMod x in this.children
				where x != null && x.burstCount > 0
				select x).FirstOrDefault<ProjectileWeaponMod>() != null;
		}
		return false;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00049ED1 File Offset: 0x000480D1
	public bool UsingInternalBurstMode()
	{
		return !this.IsBurstDisabled() && this.isBurstWeapon;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00049EE4 File Offset: 0x000480E4
	public bool IsBurstEligable()
	{
		if (this.isBurstWeapon)
		{
			return true;
		}
		if (this.children != null)
		{
			return (from ProjectileWeaponMod x in this.children
				where x != null && x.burstCount > 0
				select x).FirstOrDefault<ProjectileWeaponMod>() != null;
		}
		return false;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00049F3F File Offset: 0x0004813F
	public float TimeBetweenBursts()
	{
		return this.repeatDelay * 3f;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00049F50 File Offset: 0x00048150
	public float GetReloadDuration()
	{
		if (this.fractionalReload)
		{
			int num = Mathf.Min(this.primaryMagazine.capacity - this.primaryMagazine.contents, this.GetAvailableAmmo());
			return this.reloadStartDuration + this.reloadEndDuration + this.reloadFractionDuration * (float)num;
		}
		return this.reloadTime;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00049FA8 File Offset: 0x000481A8
	public int GetAvailableAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return this.primaryMagazine.capacity;
		}
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		ownerPlayer.inventory.FindAmmo(list, this.primaryMagazine.definition.ammoTypes);
		int num = 0;
		if (list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				global::Item item = list[i];
				if (item.info == this.primaryMagazine.ammoType)
				{
					num += item.amount;
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0004A042 File Offset: 0x00048242
	public bool IsBurstDisabled()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved6) == this.defaultOn;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0004A058 File Offset: 0x00048258
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void ToggleFireMode(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canChangeFireModes)
		{
			return;
		}
		if (!this.IsBurstEligable())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, !base.HasFlag(global::BaseEntity.Flags.Reserved6), false, true);
		base.SendNetworkUpdate_Flags();
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer.IsNpc && ownerPlayer.IsConnected)
		{
			ownerPlayer.ShowToast(GameTip.Styles.Blue_Short, this.IsBurstDisabled() ? this.Toast_BurstDisabled : this.Toast_BurstEnabled, Array.Empty<string>());
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0004A0D4 File Offset: 0x000482D4
	protected virtual void ReloadMagazine(int desiredAmount = -1)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount, true);
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0004A11C File Offset: 0x0004831C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void SwitchAmmoTo(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		int num = msg.read.Int32();
		if (num == this.primaryMagazine.ammoType.itemid)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
		if (itemDefinition == null)
		{
			return;
		}
		ItemModProjectile component = itemDefinition.GetComponent<ItemModProjectile>();
		if (!component || !component.IsAmmo(this.primaryMagazine.definition.ammoTypes))
		{
			return;
		}
		if (this.primaryMagazine.contents > 0)
		{
			ownerPlayer.GiveItem(ItemManager.CreateByItemID(this.primaryMagazine.ammoType.itemid, this.primaryMagazine.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
			this.primaryMagazine.contents = 0;
		}
		this.primaryMagazine.ammoType = itemDefinition;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0004A1FD File Offset: 0x000483FD
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.reloadStarted = false;
		this.reloadFinished = false;
		this.fractionalInsertCounter = 0;
		this.UpdateAttachmentsState();
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0004A220 File Offset: 0x00048420
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StartReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		this.reloadFinished = false;
		this.reloadStarted = true;
		this.fractionalInsertCounter = 0;
		if (this.CanRefundAmmo)
		{
			this.primaryMagazine.SwitchAmmoTypesIfNeeded(player);
		}
		this.StartReloadCooldown(this.GetReloadDuration());
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000730 RID: 1840 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanRefundAmmo
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0004A288 File Offset: 0x00048488
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void ServerFractionalReloadInsert(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.fractionalReload)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload not allowed (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_type");
			return;
		}
		if (!this.reloadStarted)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (this.GetReloadIdle() > 3f)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
			{
				"T+",
				this.GetReloadIdle(),
				"s (",
				base.ShortPrefabName,
				")"
			}));
			player.stats.combat.LogInvalid(player, this, "reload_time");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload too early (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_too_early");
			this.reloadStarted = false;
			this.reloadFinished = false;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration + (float)this.fractionalInsertCounter * this.reloadFractionDuration)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload rate too high (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_rate");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		this.fractionalInsertCounter++;
		if (this.primaryMagazine.contents < this.primaryMagazine.capacity)
		{
			this.ReloadMagazine(1);
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0004A49C File Offset: 0x0004869C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void Reload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.reloadStarted)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.fractionalReload)
		{
			if (this.GetReloadCooldown() > 1f)
			{
				global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
				{
					"T-",
					this.GetReloadCooldown(),
					"s (",
					base.ShortPrefabName,
					")"
				}));
				player.stats.combat.LogInvalid(player, this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
			if (this.GetReloadIdle() > 1.5f)
			{
				global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
				{
					"T+",
					this.GetReloadIdle(),
					"s (",
					base.ShortPrefabName,
					")"
				}));
				player.stats.combat.LogInvalid(player, this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
		}
		if (this.fractionalReload)
		{
			this.ResetReloadCooldown();
		}
		this.reloadStarted = false;
		this.reloadFinished = true;
		if (!this.fractionalReload)
		{
			this.ReloadMagazine(-1);
		}
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0004A638 File Offset: 0x00048838
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void CLProject(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this.reloadFinished && this.HasReloadCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (this.primaryMagazine.contents <= 0 && !base.UsingInfiniteAmmoCheat)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_missing");
			return;
		}
		ItemDefinition primaryMagazineAmmo = this.PrimaryMagazineAmmo;
		ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);
		if (primaryMagazineAmmo.itemid != projectileShoot.ammoType)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Ammo mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_mismatch");
			return;
		}
		if (!base.UsingInfiniteAmmoCheat)
		{
			this.primaryMagazine.contents--;
		}
		ItemModProjectile component = primaryMagazineAmmo.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		if (projectileShoot.projectiles.Count > component.numProjectiles)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Count mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "count_mismatch");
			return;
		}
		if (player.InGesture)
		{
			return;
		}
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, string.Empty, msg.connection);
		player.CleanupExpiredProjectiles();
		Guid guid = Guid.NewGuid();
		foreach (ProjectileShoot.Projectile projectile in projectileShoot.projectiles)
		{
			if (player.HasFiredProjectile(projectile.projectileID))
			{
				global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Duplicate ID (" + projectile.projectileID + ")");
				player.stats.combat.LogInvalid(player, this, "duplicate_id");
			}
			else if (base.ValidateEyePos(player, projectile.startPos))
			{
				player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, primaryMagazineAmmo, guid, null);
				this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, projectile.startPos, projectile.startVel, projectile.seed, msg.connection, this.IsSilenced(), false);
			}
		}
		player.MakeNoise(player.transform.position, BaseCombatEntity.ActionVolume.Loud);
		player.stats.Add(component.category + "_fired", projectileShoot.projectiles.Count<ProjectileShoot.Projectile>(), (global::Stats)5);
		player.LifeStoryShotFired(this);
		base.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay) + this.animationDelay);
		player.MarkHostileFor(60f);
		this.UpdateItemCondition();
		this.DidAttackServerside();
		float num = 0f;
		if (component.projectileObject != null)
		{
			GameObject gameObject = component.projectileObject.Get();
			if (gameObject != null)
			{
				Projectile component2 = gameObject.GetComponent<Projectile>();
				if (component2 != null)
				{
					foreach (DamageTypeEntry damageTypeEntry in component2.damageTypes)
					{
						num += damageTypeEntry.amount;
					}
				}
			}
		}
		float num2 = this.NoiseRadius;
		if (this.IsSilenced())
		{
			num2 *= AI.npc_gun_noise_silencer_modifier;
		}
		Sense.Stimulate(new Sensation
		{
			Type = SensationType.Gunshot,
			Position = player.transform.position,
			Radius = num2,
			DamagePotential = num,
			InitiatorPlayer = player,
			Initiator = player
		});
		EACServer.LogPlayerUseWeapon(player, this);
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000734 RID: 1844 RVA: 0x0004AA70 File Offset: 0x00048C70
	protected virtual ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			return this.primaryMagazine.ammoType;
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0004AA80 File Offset: 0x00048C80
	private void CreateProjectileEffectClientside(string prefabName, Vector3 pos, Vector3 velocity, int seed, Connection sourceConnection, bool silenced = false, bool forceClientsideEffects = false)
	{
		Effect effect = global::BaseProjectile.reusableInstance;
		effect.Clear();
		effect.Init(Effect.Type.Projectile, pos, velocity, sourceConnection);
		effect.scale = (silenced ? 0f : 1f);
		if (forceClientsideEffects)
		{
			effect.scale = 2f;
		}
		effect.pooledString = prefabName;
		effect.number = seed;
		EffectNetwork.Send(effect);
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0004AAE0 File Offset: 0x00048CE0
	public void UpdateItemCondition()
	{
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		float barrelConditionLoss = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>().barrelConditionLoss;
		float num = 0.25f;
		bool usingInfiniteAmmoCheat = base.UsingInfiniteAmmoCheat;
		if (!usingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(num + barrelConditionLoss);
		}
		if (ownerItem.contents != null && ownerItem.contents.itemList != null)
		{
			for (int i = ownerItem.contents.itemList.Count - 1; i >= 0; i--)
			{
				global::Item item = ownerItem.contents.itemList[i];
				if (item != null && !usingInfiniteAmmoCheat)
				{
					item.LoseCondition(num + barrelConditionLoss);
				}
			}
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0004AB84 File Offset: 0x00048D84
	public bool IsSilenced()
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				ProjectileWeaponMod projectileWeaponMod = baseEntity as ProjectileWeaponMod;
				if (projectileWeaponMod != null && projectileWeaponMod.isSilencer && !projectileWeaponMod.IsBroken())
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0004AC00 File Offset: 0x00048E00
	public override bool CanUseNetworkCache(Connection sendingTo)
	{
		Connection ownerConnection = base.GetOwnerConnection();
		return sendingTo == null || ownerConnection == null || sendingTo != ownerConnection;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x0004AC24 File Offset: 0x00048E24
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		if (info.forDisk || info.SendingTo(base.GetOwnerConnection()) || this.ForceSendMagazine(info))
		{
			info.msg.baseProjectile.primaryMagazine = this.primaryMagazine.Save();
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0004AC84 File Offset: 0x00048E84
	public virtual bool ForceSendMagazine(global::BaseNetworkable.SaveInfo saveInfo)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer && ownerPlayer.IsBeingSpectated)
		{
			foreach (global::BaseEntity baseEntity in ownerPlayer.children)
			{
				if (baseEntity.net != null && baseEntity.net.connection == saveInfo.forConnection)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0004AD0C File Offset: 0x00048F0C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.primaryMagazine.Load(info.msg.baseProjectile.primaryMagazine);
		}
	}

	// Token: 0x04000476 RID: 1142
	[Header("NPC Info")]
	public float NoiseRadius = 100f;

	// Token: 0x04000477 RID: 1143
	[Header("Projectile")]
	public float damageScale = 1f;

	// Token: 0x04000478 RID: 1144
	public float distanceScale = 1f;

	// Token: 0x04000479 RID: 1145
	public float projectileVelocityScale = 1f;

	// Token: 0x0400047A RID: 1146
	public bool automatic;

	// Token: 0x0400047B RID: 1147
	public bool usableByTurret = true;

	// Token: 0x0400047C RID: 1148
	[Tooltip("Final damage is scaled by this amount before being applied to a target when this weapon is mounted to a turret")]
	public float turretDamageScale = 0.35f;

	// Token: 0x0400047D RID: 1149
	[Header("Effects")]
	public GameObjectRef attackFX;

	// Token: 0x0400047E RID: 1150
	public GameObjectRef silencedAttack;

	// Token: 0x0400047F RID: 1151
	public GameObjectRef muzzleBrakeAttack;

	// Token: 0x04000480 RID: 1152
	public Transform MuzzlePoint;

	// Token: 0x04000481 RID: 1153
	[Header("Reloading")]
	public float reloadTime = 1f;

	// Token: 0x04000482 RID: 1154
	public bool canUnloadAmmo = true;

	// Token: 0x04000483 RID: 1155
	public global::BaseProjectile.Magazine primaryMagazine;

	// Token: 0x04000484 RID: 1156
	public bool fractionalReload;

	// Token: 0x04000485 RID: 1157
	public float reloadStartDuration;

	// Token: 0x04000486 RID: 1158
	public float reloadFractionDuration;

	// Token: 0x04000487 RID: 1159
	public float reloadEndDuration;

	// Token: 0x04000488 RID: 1160
	[Header("Recoil")]
	public float aimSway = 3f;

	// Token: 0x04000489 RID: 1161
	public float aimSwaySpeed = 1f;

	// Token: 0x0400048A RID: 1162
	public RecoilProperties recoil;

	// Token: 0x0400048B RID: 1163
	[Header("Aim Cone")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x0400048C RID: 1164
	public float aimCone;

	// Token: 0x0400048D RID: 1165
	public float hipAimCone = 1.8f;

	// Token: 0x0400048E RID: 1166
	public float aimconePenaltyPerShot;

	// Token: 0x0400048F RID: 1167
	public float aimConePenaltyMax;

	// Token: 0x04000490 RID: 1168
	public float aimconePenaltyRecoverTime = 0.1f;

	// Token: 0x04000491 RID: 1169
	public float aimconePenaltyRecoverDelay = 0.1f;

	// Token: 0x04000492 RID: 1170
	public float stancePenaltyScale = 1f;

	// Token: 0x04000493 RID: 1171
	[Header("Iconsights")]
	public bool hasADS = true;

	// Token: 0x04000494 RID: 1172
	public bool noAimingWhileCycling;

	// Token: 0x04000495 RID: 1173
	public bool manualCycle;

	// Token: 0x04000496 RID: 1174
	[NonSerialized]
	protected bool needsCycle;

	// Token: 0x04000497 RID: 1175
	[NonSerialized]
	protected bool isCycling;

	// Token: 0x04000498 RID: 1176
	[NonSerialized]
	public bool aiming;

	// Token: 0x04000499 RID: 1177
	[Header("ViewModel")]
	public bool useEmptyAmmoState;

	// Token: 0x0400049A RID: 1178
	[Header("Burst Information")]
	public bool isBurstWeapon;

	// Token: 0x0400049B RID: 1179
	public bool canChangeFireModes = true;

	// Token: 0x0400049C RID: 1180
	public bool defaultOn = true;

	// Token: 0x0400049D RID: 1181
	public float internalBurstRecoilScale = 0.8f;

	// Token: 0x0400049E RID: 1182
	public float internalBurstFireRateScale = 0.8f;

	// Token: 0x0400049F RID: 1183
	public float internalBurstAimConeScale = 0.8f;

	// Token: 0x040004A0 RID: 1184
	public Translate.Phrase Toast_BurstDisabled = new Translate.Phrase("burst_disabled", "Burst Disabled");

	// Token: 0x040004A1 RID: 1185
	public Translate.Phrase Toast_BurstEnabled = new Translate.Phrase("burst enabled", "Burst Enabled");

	// Token: 0x040004A2 RID: 1186
	public float resetDuration = 0.3f;

	// Token: 0x040004A3 RID: 1187
	public int numShotsFired;

	// Token: 0x040004A4 RID: 1188
	[NonSerialized]
	private float nextReloadTime = float.NegativeInfinity;

	// Token: 0x040004A5 RID: 1189
	[NonSerialized]
	private float startReloadTime = float.NegativeInfinity;

	// Token: 0x040004A6 RID: 1190
	private float lastReloadTime = -10f;

	// Token: 0x040004A7 RID: 1191
	private bool modsChangedInitialized;

	// Token: 0x040004A8 RID: 1192
	private float stancePenalty;

	// Token: 0x040004A9 RID: 1193
	private float aimconePenalty;

	// Token: 0x040004AA RID: 1194
	private uint cachedModHash;

	// Token: 0x040004AB RID: 1195
	private float sightAimConeScale = 1f;

	// Token: 0x040004AC RID: 1196
	private float sightAimConeOffset;

	// Token: 0x040004AD RID: 1197
	private float hipAimConeScale = 1f;

	// Token: 0x040004AE RID: 1198
	private float hipAimConeOffset;

	// Token: 0x040004AF RID: 1199
	protected bool reloadStarted;

	// Token: 0x040004B0 RID: 1200
	protected bool reloadFinished;

	// Token: 0x040004B1 RID: 1201
	private int fractionalInsertCounter;

	// Token: 0x040004B2 RID: 1202
	private static readonly Effect reusableInstance = new Effect();

	// Token: 0x02000BBF RID: 3007
	[Serializable]
	public class Magazine
	{
		// Token: 0x06004D7F RID: 19839 RVA: 0x001A107E File Offset: 0x0019F27E
		public void ServerInit()
		{
			if (this.definition.builtInSize > 0)
			{
				this.capacity = this.definition.builtInSize;
			}
		}

		// Token: 0x06004D80 RID: 19840 RVA: 0x001A10A0 File Offset: 0x0019F2A0
		public ProtoBuf.Magazine Save()
		{
			ProtoBuf.Magazine magazine = Facepunch.Pool.Get<ProtoBuf.Magazine>();
			if (this.ammoType == null)
			{
				magazine.capacity = this.capacity;
				magazine.contents = 0;
				magazine.ammoType = 0;
			}
			else
			{
				magazine.capacity = this.capacity;
				magazine.contents = this.contents;
				magazine.ammoType = this.ammoType.itemid;
			}
			return magazine;
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x001A1107 File Offset: 0x0019F307
		public void Load(ProtoBuf.Magazine mag)
		{
			this.contents = mag.contents;
			this.capacity = mag.capacity;
			this.ammoType = ItemManager.FindItemDefinition(mag.ammoType);
		}

		// Token: 0x06004D82 RID: 19842 RVA: 0x001A1132 File Offset: 0x0019F332
		public bool CanReload(global::BasePlayer owner)
		{
			return this.contents < this.capacity && owner.inventory.HasAmmo(this.definition.ammoTypes);
		}

		// Token: 0x06004D83 RID: 19843 RVA: 0x001A115A File Offset: 0x0019F35A
		public bool CanAiReload(global::BasePlayer owner)
		{
			return this.contents < this.capacity;
		}

		// Token: 0x06004D84 RID: 19844 RVA: 0x001A1170 File Offset: 0x0019F370
		public void SwitchAmmoTypesIfNeeded(global::BasePlayer owner)
		{
			List<global::Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<global::Item>();
			if (list.Count == 0)
			{
				List<global::Item> list2 = new List<global::Item>();
				owner.inventory.FindAmmo(list2, this.definition.ammoTypes);
				if (list2.Count == 0)
				{
					return;
				}
				list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList<global::Item>();
				if (list == null || list.Count == 0)
				{
					return;
				}
				if (this.contents > 0)
				{
					owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x001A123C File Offset: 0x0019F43C
		public bool Reload(global::BasePlayer owner, int desiredAmount = -1, bool canRefundAmmo = true)
		{
			List<global::Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<global::Item>();
			if (list.Count == 0)
			{
				List<global::Item> list2 = new List<global::Item>();
				owner.inventory.FindAmmo(list2, this.definition.ammoTypes);
				if (list2.Count == 0)
				{
					return false;
				}
				list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList<global::Item>();
				if (list == null || list.Count == 0)
				{
					return false;
				}
				if (this.contents > 0)
				{
					if (canRefundAmmo)
					{
						owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
					}
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
			int num = desiredAmount;
			if (num == -1)
			{
				num = this.capacity - this.contents;
			}
			foreach (global::Item item in list)
			{
				int amount = item.amount;
				int num2 = Mathf.Min(num, item.amount);
				item.UseItem(num2);
				this.contents += num2;
				num -= num2;
				if (num <= 0)
				{
					break;
				}
			}
			return false;
		}

		// Token: 0x04004130 RID: 16688
		public global::BaseProjectile.Magazine.Definition definition;

		// Token: 0x04004131 RID: 16689
		public int capacity;

		// Token: 0x04004132 RID: 16690
		public int contents;

		// Token: 0x04004133 RID: 16691
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition ammoType;

		// Token: 0x02000FD7 RID: 4055
		[Serializable]
		public struct Definition
		{
			// Token: 0x04005162 RID: 20834
			[Tooltip("Set to 0 to not use inbuilt mag")]
			public int builtInSize;

			// Token: 0x04005163 RID: 20835
			[Tooltip("If using inbuilt mag, will accept these types of ammo")]
			[global::InspectorFlags]
			public AmmoTypes ammoTypes;
		}
	}

	// Token: 0x02000BC0 RID: 3008
	public static class BaseProjectileFlags
	{
		// Token: 0x04004134 RID: 16692
		public const global::BaseEntity.Flags BurstToggle = global::BaseEntity.Flags.Reserved6;
	}
}
