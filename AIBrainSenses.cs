using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using UnityEngine;

// Token: 0x02000361 RID: 865
public class AIBrainSenses
{
	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06001FA3 RID: 8099 RVA: 0x000D59E1 File Offset: 0x000D3BE1
	public float TimeSinceThreat
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.LastThreatTimestamp;
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06001FA4 RID: 8100 RVA: 0x000D59EF File Offset: 0x000D3BEF
	// (set) Token: 0x06001FA5 RID: 8101 RVA: 0x000D59F7 File Offset: 0x000D3BF7
	public SimpleAIMemory Memory { get; private set; } = new SimpleAIMemory();

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001FA6 RID: 8102 RVA: 0x000D5A00 File Offset: 0x000D3C00
	public float TargetLostRange
	{
		get
		{
			return this.targetLostRange;
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001FA7 RID: 8103 RVA: 0x000D5A08 File Offset: 0x000D3C08
	// (set) Token: 0x06001FA8 RID: 8104 RVA: 0x000D5A10 File Offset: 0x000D3C10
	public bool ignoreSafeZonePlayers { get; private set; }

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000D5A1C File Offset: 0x000D3C1C
	public void Init(BaseEntity owner, BaseAIBrain brain, float memoryDuration, float range, float targetLostRange, float visionCone, bool checkVision, bool checkLOS, bool ignoreNonVisionSneakers, float listenRange, bool hostileTargetsOnly, bool senseFriendlies, bool ignoreSafeZonePlayers, EntityType senseTypes, bool refreshKnownLOS)
	{
		this.aiCaresAbout = new Func<BaseEntity, bool>(this.AiCaresAbout);
		this.owner = owner;
		this.brain = brain;
		this.MemoryDuration = memoryDuration;
		this.ownerAttack = owner as IAIAttack;
		this.playerOwner = owner as BasePlayer;
		this.maxRange = range;
		this.targetLostRange = targetLostRange;
		this.visionCone = visionCone;
		this.checkVision = checkVision;
		this.checkLOS = checkLOS;
		this.ignoreNonVisionSneakers = ignoreNonVisionSneakers;
		this.listenRange = listenRange;
		this.hostileTargetsOnly = hostileTargetsOnly;
		this.senseFriendlies = senseFriendlies;
		this.ignoreSafeZonePlayers = ignoreSafeZonePlayers;
		this.senseTypes = senseTypes;
		this.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
		this.refreshKnownLOS = refreshKnownLOS;
		this.ownerSenses = owner as IAISenses;
		this.knownPlayersLOSUpdateInterval = ((owner is HumanNPC) ? AIBrainSenses.HumanKnownPlayersLOSUpdateInterval : AIBrainSenses.KnownPlayersLOSUpdateInterval);
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000D5AF9 File Offset: 0x000D3CF9
	public void DelaySenseUpdate(float delay)
	{
		this.nextUpdateTime = UnityEngine.Time.time + delay;
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000D5B08 File Offset: 0x000D3D08
	public void Update()
	{
		if (this.owner == null)
		{
			return;
		}
		this.UpdateSenses();
		this.UpdateKnownPlayersLOS();
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000D5B28 File Offset: 0x000D3D28
	private void UpdateSenses()
	{
		if (UnityEngine.Time.time < this.nextUpdateTime)
		{
			return;
		}
		this.nextUpdateTime = UnityEngine.Time.time + AIBrainSenses.UpdateInterval;
		if (this.senseTypes != (EntityType)0)
		{
			if (this.senseTypes == EntityType.Player)
			{
				this.SensePlayers();
			}
			else
			{
				this.SenseBrains();
				if (this.senseTypes.HasFlag(EntityType.Player))
				{
					this.SensePlayers();
				}
			}
		}
		this.Memory.Forget(this.MemoryDuration);
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000D5BA4 File Offset: 0x000D3DA4
	public void UpdateKnownPlayersLOS()
	{
		if (UnityEngine.Time.time < this.nextKnownPlayersLOSUpdateTime)
		{
			return;
		}
		this.nextKnownPlayersLOSUpdateTime = UnityEngine.Time.time + this.knownPlayersLOSUpdateInterval;
		foreach (BaseEntity baseEntity in this.Memory.Players)
		{
			if (!(baseEntity == null) && !baseEntity.IsNpc)
			{
				bool flag = this.ownerAttack.CanSeeTarget(baseEntity);
				this.Memory.SetLOS(baseEntity, flag);
				if (this.refreshKnownLOS && this.owner != null && flag && Vector3.Distance(baseEntity.transform.position, this.owner.transform.position) <= this.TargetLostRange)
				{
					this.Memory.SetKnown(baseEntity, this.owner, this);
				}
			}
		}
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000D5C9C File Offset: 0x000D3E9C
	private void SensePlayers()
	{
		int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.playerQueryResults, this.aiCaresAbout);
		for (int i = 0; i < playersInSphere; i++)
		{
			BasePlayer basePlayer = AIBrainSenses.playerQueryResults[i];
			this.Memory.SetKnown(basePlayer, this.owner, this);
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x000D5CFC File Offset: 0x000D3EFC
	private void SenseBrains()
	{
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.queryResults, this.aiCaresAbout);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity baseEntity = AIBrainSenses.queryResults[i];
			this.Memory.SetKnown(baseEntity, this.owner, this);
		}
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x000D5D5C File Offset: 0x000D3F5C
	private bool AiCaresAbout(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (!entity.isServer)
		{
			return false;
		}
		if (entity.EqualNetID(this.owner))
		{
			return false;
		}
		if (entity.Health() <= 0f)
		{
			return false;
		}
		if (!this.IsValidSenseType(entity))
		{
			return false;
		}
		BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
		BasePlayer basePlayer = entity as BasePlayer;
		if (basePlayer != null && basePlayer.IsDead())
		{
			return false;
		}
		if (this.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			return false;
		}
		if (this.listenRange > 0f && baseCombatEntity != null && baseCombatEntity.TimeSinceLastNoise <= 1f && baseCombatEntity.CanLastNoiseBeHeard(this.owner.transform.position, this.listenRange))
		{
			return true;
		}
		if (this.senseFriendlies && this.ownerSenses != null && this.ownerSenses.IsFriendly(entity))
		{
			return true;
		}
		float num = float.PositiveInfinity;
		if (baseCombatEntity != null && AI.accuratevisiondistance)
		{
			num = Vector3.Distance(this.owner.transform.position, baseCombatEntity.transform.position);
			if (num > this.maxRange)
			{
				return false;
			}
		}
		if (this.checkVision && !this.IsTargetInVision(entity))
		{
			if (!this.ignoreNonVisionSneakers)
			{
				return false;
			}
			if (basePlayer != null && !basePlayer.IsNpc)
			{
				if (!AI.accuratevisiondistance)
				{
					num = Vector3.Distance(this.owner.transform.position, basePlayer.transform.position);
				}
				if ((basePlayer.IsDucked() && num >= this.brain.IgnoreSneakersMaxDistance) || num >= this.brain.IgnoreNonVisionMaxDistance)
				{
					return false;
				}
			}
		}
		if (this.hostileTargetsOnly && baseCombatEntity != null && !baseCombatEntity.IsHostile() && !(baseCombatEntity is ScarecrowNPC))
		{
			return false;
		}
		if (this.checkLOS && this.ownerAttack != null)
		{
			bool flag = this.ownerAttack.CanSeeTarget(entity);
			this.Memory.SetLOS(entity, flag);
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x000D5F58 File Offset: 0x000D4158
	private bool IsValidSenseType(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				if (ent is BasePet)
				{
					return true;
				}
				if (ent is ScarecrowNPC)
				{
					return true;
				}
				if (this.senseTypes.HasFlag(EntityType.BasePlayerNPC))
				{
					return true;
				}
			}
			else if (this.senseTypes.HasFlag(EntityType.Player))
			{
				return true;
			}
		}
		return (this.senseTypes.HasFlag(EntityType.NPC) && ent is BaseNpc) || (this.senseTypes.HasFlag(EntityType.WorldItem) && ent is WorldItem) || (this.senseTypes.HasFlag(EntityType.Corpse) && ent is BaseCorpse) || (this.senseTypes.HasFlag(EntityType.TimedExplosive) && ent is TimedExplosive) || (this.senseTypes.HasFlag(EntityType.Chair) && ent is BaseChair);
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x000D6074 File Offset: 0x000D4274
	private bool IsTargetInVision(BaseEntity target)
	{
		Vector3 vector = Vector3Ex.Direction(target.transform.position, this.owner.transform.position);
		return Vector3.Dot((this.playerOwner != null) ? this.playerOwner.eyes.BodyForward() : this.owner.transform.forward, vector) >= this.visionCone;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x000D60E3 File Offset: 0x000D42E3
	public BaseEntity GetNearestPlayer(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Players, rangeFraction);
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06001FB4 RID: 8116 RVA: 0x000D60F7 File Offset: 0x000D42F7
	public List<BaseEntity> Players
	{
		get
		{
			return this.Memory.Players;
		}
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000D6104 File Offset: 0x000D4304
	public BaseEntity GetNearestThreat(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Threats, rangeFraction);
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000D6118 File Offset: 0x000D4318
	public BaseEntity GetNearestTarget(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Targets, rangeFraction);
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000D612C File Offset: 0x000D432C
	private BaseEntity GetNearest(List<BaseEntity> entities, float rangeFraction)
	{
		if (entities == null || entities.Count == 0)
		{
			return null;
		}
		float positiveInfinity = float.PositiveInfinity;
		BaseEntity baseEntity = null;
		foreach (BaseEntity baseEntity2 in entities)
		{
			if (!(baseEntity2 == null) && baseEntity2.Health() > 0f)
			{
				float num = Vector3.Distance(baseEntity2.transform.position, this.owner.transform.position);
				if (num <= rangeFraction * this.maxRange && num < positiveInfinity)
				{
					baseEntity = baseEntity2;
				}
			}
		}
		return baseEntity;
	}

	// Token: 0x040018E9 RID: 6377
	[ServerVar]
	public static float UpdateInterval = 0.5f;

	// Token: 0x040018EA RID: 6378
	[ServerVar]
	public static float HumanKnownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040018EB RID: 6379
	[ServerVar]
	public static float KnownPlayersLOSUpdateInterval = 0.5f;

	// Token: 0x040018EC RID: 6380
	private float knownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040018ED RID: 6381
	public float MemoryDuration = 10f;

	// Token: 0x040018EE RID: 6382
	public float LastThreatTimestamp;

	// Token: 0x040018EF RID: 6383
	public float TimeInAgressiveState;

	// Token: 0x040018F1 RID: 6385
	private static BaseEntity[] queryResults = new BaseEntity[64];

	// Token: 0x040018F2 RID: 6386
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x040018F3 RID: 6387
	private float nextUpdateTime;

	// Token: 0x040018F4 RID: 6388
	private float nextKnownPlayersLOSUpdateTime;

	// Token: 0x040018F5 RID: 6389
	private BaseEntity owner;

	// Token: 0x040018F6 RID: 6390
	private BasePlayer playerOwner;

	// Token: 0x040018F7 RID: 6391
	private IAISenses ownerSenses;

	// Token: 0x040018F8 RID: 6392
	private float maxRange;

	// Token: 0x040018F9 RID: 6393
	private float targetLostRange;

	// Token: 0x040018FA RID: 6394
	private float visionCone;

	// Token: 0x040018FB RID: 6395
	private bool checkVision;

	// Token: 0x040018FC RID: 6396
	private bool checkLOS;

	// Token: 0x040018FD RID: 6397
	private bool ignoreNonVisionSneakers;

	// Token: 0x040018FE RID: 6398
	private float listenRange;

	// Token: 0x040018FF RID: 6399
	private bool hostileTargetsOnly;

	// Token: 0x04001900 RID: 6400
	private bool senseFriendlies;

	// Token: 0x04001901 RID: 6401
	private bool refreshKnownLOS;

	// Token: 0x04001903 RID: 6403
	private EntityType senseTypes;

	// Token: 0x04001904 RID: 6404
	private IAIAttack ownerAttack;

	// Token: 0x04001905 RID: 6405
	public BaseAIBrain brain;

	// Token: 0x04001906 RID: 6406
	private Func<BaseEntity, bool> aiCaresAbout;
}
