using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class CombatLog
{
	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06002505 RID: 9477 RVA: 0x000EA60E File Offset: 0x000E880E
	// (set) Token: 0x06002506 RID: 9478 RVA: 0x000EA616 File Offset: 0x000E8816
	public float LastActive { get; private set; }

	// Token: 0x06002507 RID: 9479 RVA: 0x000EA61F File Offset: 0x000E881F
	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000EA62E File Offset: 0x000E882E
	public void Init()
	{
		this.storage = CombatLog.Get(this.player.userID);
		this.LastActive = this.storage.LastOrDefault<CombatLog.Event>().time;
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Save()
	{
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000EA65C File Offset: 0x000E885C
	public void LogInvalid(BasePlayer player, AttackEntity weapon, string description)
	{
		this.Log(player, weapon, null, description, null, -1, -1f, null);
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000EA67C File Offset: 0x000E887C
	public void LogInvalid(HitInfo info, string description)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, -1f, info);
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000EA6BC File Offset: 0x000E88BC
	public void LogAttack(HitInfo info, string description, float oldHealth = -1f)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, oldHealth, info);
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x000EA6F8 File Offset: 0x000E88F8
	public void Log(BaseEntity attacker, AttackEntity weapon, BaseCombatEntity hitEntity, string description, Projectile projectilePrefab = null, int projectileId = -1, float healthOld = -1f, HitInfo hitInfo = null)
	{
		CombatLog.Event @event = default(CombatLog.Event);
		float num = 0f;
		if (hitInfo != null)
		{
			num = (hitInfo.IsProjectile() ? hitInfo.ProjectileDistance : Vector3.Distance(hitInfo.PointStart, hitInfo.HitPositionWorld));
			BasePlayer basePlayer;
			if ((basePlayer = hitInfo.Initiator as BasePlayer) != null && hitInfo.HitEntity != hitInfo.Initiator)
			{
				@event.attacker_dead = basePlayer.IsDead() || basePlayer.IsWounded();
			}
		}
		float num2 = ((hitEntity != null) ? hitEntity.Health() : 0f);
		@event.time = UnityEngine.Time.realtimeSinceStartup;
		@event.attacker_id = ((attacker != null && attacker.net != null) ? attacker.net.ID : default(NetworkableId)).Value;
		@event.target_id = ((hitEntity != null && hitEntity.net != null) ? hitEntity.net.ID : default(NetworkableId)).Value;
		@event.attacker = ((this.player == attacker) ? "you" : (((attacker != null) ? attacker.ShortPrefabName : null) ?? "N/A"));
		@event.target = ((this.player == hitEntity) ? "you" : (((hitEntity != null) ? hitEntity.ShortPrefabName : null) ?? "N/A"));
		@event.weapon = ((weapon != null) ? weapon.name : "N/A");
		@event.ammo = ((projectilePrefab != null) ? ((projectilePrefab != null) ? projectilePrefab.name : null) : "N/A");
		@event.bone = ((hitInfo != null) ? hitInfo.boneName : null) ?? "N/A";
		@event.area = ((hitInfo != null) ? hitInfo.boneArea : ((HitArea)0));
		@event.distance = num;
		@event.health_old = ((healthOld == -1f) ? 0f : healthOld);
		@event.health_new = num2;
		@event.info = description ?? string.Empty;
		@event.proj_hits = ((hitInfo != null) ? hitInfo.ProjectileHits : 0);
		@event.proj_integrity = ((hitInfo != null) ? hitInfo.ProjectileIntegrity : 0f);
		@event.proj_travel = ((hitInfo != null) ? hitInfo.ProjectileTravelTime : 0f);
		@event.proj_mismatch = ((hitInfo != null) ? hitInfo.ProjectileTrajectoryMismatch : 0f);
		BasePlayer basePlayer2 = attacker as BasePlayer;
		BasePlayer.FiredProjectile firedProjectile;
		if (basePlayer2 != null && projectilePrefab != null && basePlayer2.firedProjectiles.TryGetValue(projectileId, out firedProjectile))
		{
			@event.desync = (int)(firedProjectile.desyncLifeTime * 1000f);
		}
		this.Log(@event);
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000EA9C4 File Offset: 0x000E8BC4
	private void Log(CombatLog.Event val)
	{
		this.LastActive = UnityEngine.Time.realtimeSinceStartup;
		if (this.storage == null)
		{
			return;
		}
		this.storage.Enqueue(val);
		int num = Mathf.Max(0, Server.combatlogsize);
		while (this.storage.Count > num)
		{
			this.storage.Dequeue();
		}
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000EAA1C File Offset: 0x000E8C1C
	public string Get(int count, NetworkableId filterByAttacker = default(NetworkableId), bool json = false, bool isAdmin = false, ulong requestingUser = 0UL)
	{
		if (this.storage == null)
		{
			return string.Empty;
		}
		if (this.storage.Count == 0 && !json)
		{
			return "Combat log empty.";
		}
		TextTable textTable = new TextTable();
		textTable.AddColumn("time");
		textTable.AddColumn("attacker");
		textTable.AddColumn("id");
		textTable.AddColumn("target");
		textTable.AddColumn("id");
		textTable.AddColumn("weapon");
		textTable.AddColumn("ammo");
		textTable.AddColumn("area");
		textTable.AddColumn("distance");
		textTable.AddColumn("old_hp");
		textTable.AddColumn("new_hp");
		textTable.AddColumn("info");
		textTable.AddColumn("hits");
		textTable.AddColumn("integrity");
		textTable.AddColumn("travel");
		textTable.AddColumn("mismatch");
		textTable.AddColumn("desync");
		int num = this.storage.Count - count;
		int combatlogdelay = Server.combatlogdelay;
		int num2 = 0;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		foreach (CombatLog.Event @event in this.storage)
		{
			if (num > 0)
			{
				num--;
			}
			else if ((!filterByAttacker.IsValid || @event.attacker_id == filterByAttacker.Value) && (!(activeGameMode != null) || activeGameMode.returnValidCombatlog || isAdmin || @event.proj_hits <= 0))
			{
				float num3 = UnityEngine.Time.realtimeSinceStartup - @event.time;
				if (num3 >= (float)combatlogdelay)
				{
					string text = num3.ToString("0.00s");
					string attacker = @event.attacker;
					ulong num4 = @event.attacker_id;
					string text2 = num4.ToString();
					string target = @event.target;
					num4 = @event.target_id;
					string text3 = num4.ToString();
					string weapon = @event.weapon;
					string ammo = @event.ammo;
					string text4 = HitAreaUtil.Format(@event.area).ToLower();
					float num5 = @event.distance;
					string text5 = num5.ToString("0.0m");
					num5 = @event.health_old;
					string text6 = num5.ToString("0.0");
					num5 = @event.health_new;
					string text7 = num5.ToString("0.0");
					string text8 = @event.info;
					if (!this.player.IsDestroyed && this.player.userID == requestingUser && @event.attacker_dead)
					{
						text8 = "you died first (" + text8 + ")";
					}
					int num6 = @event.proj_hits;
					string text9 = num6.ToString();
					num5 = @event.proj_integrity;
					string text10 = num5.ToString("0.00");
					num5 = @event.proj_travel;
					string text11 = num5.ToString("0.00s");
					num5 = @event.proj_mismatch;
					string text12 = num5.ToString("0.00m");
					num6 = @event.desync;
					string text13 = num6.ToString();
					textTable.AddRow(new string[]
					{
						text, attacker, text2, target, text3, weapon, ammo, text4, text5, text6,
						text7, text8, text9, text10, text11, text12, text13
					});
				}
				else
				{
					num2++;
				}
			}
		}
		string text14;
		if (json)
		{
			text14 = textTable.ToJson();
		}
		else
		{
			text14 = textTable.ToString();
			if (num2 > 0)
			{
				text14 = string.Concat(new object[]
				{
					text14,
					"+ ",
					num2,
					" ",
					(num2 > 1) ? "events" : "event"
				});
				text14 = string.Concat(new object[]
				{
					text14,
					" in the last ",
					combatlogdelay,
					" ",
					(combatlogdelay > 1) ? "seconds" : "second"
				});
			}
		}
		return text14;
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000EAE48 File Offset: 0x000E9048
	public static Queue<CombatLog.Event> Get(ulong id)
	{
		Queue<CombatLog.Event> queue;
		if (CombatLog.players.TryGetValue(id, out queue))
		{
			return queue;
		}
		queue = new Queue<CombatLog.Event>();
		CombatLog.players.Add(id, queue);
		return queue;
	}

	// Token: 0x04001D4D RID: 7501
	private const string selfname = "you";

	// Token: 0x04001D4E RID: 7502
	private const string noname = "N/A";

	// Token: 0x04001D4F RID: 7503
	private BasePlayer player;

	// Token: 0x04001D50 RID: 7504
	private Queue<CombatLog.Event> storage;

	// Token: 0x04001D52 RID: 7506
	private static Dictionary<ulong, Queue<CombatLog.Event>> players = new Dictionary<ulong, Queue<CombatLog.Event>>();

	// Token: 0x02000D03 RID: 3331
	public struct Event
	{
		// Token: 0x04004670 RID: 18032
		public float time;

		// Token: 0x04004671 RID: 18033
		public ulong attacker_id;

		// Token: 0x04004672 RID: 18034
		public ulong target_id;

		// Token: 0x04004673 RID: 18035
		public string attacker;

		// Token: 0x04004674 RID: 18036
		public string target;

		// Token: 0x04004675 RID: 18037
		public string weapon;

		// Token: 0x04004676 RID: 18038
		public string ammo;

		// Token: 0x04004677 RID: 18039
		public string bone;

		// Token: 0x04004678 RID: 18040
		public HitArea area;

		// Token: 0x04004679 RID: 18041
		public float distance;

		// Token: 0x0400467A RID: 18042
		public float health_old;

		// Token: 0x0400467B RID: 18043
		public float health_new;

		// Token: 0x0400467C RID: 18044
		public string info;

		// Token: 0x0400467D RID: 18045
		public int proj_hits;

		// Token: 0x0400467E RID: 18046
		public float proj_integrity;

		// Token: 0x0400467F RID: 18047
		public float proj_travel;

		// Token: 0x04004680 RID: 18048
		public float proj_mismatch;

		// Token: 0x04004681 RID: 18049
		public int desync;

		// Token: 0x04004682 RID: 18050
		public bool attacker_dead;
	}
}
