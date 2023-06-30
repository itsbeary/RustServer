using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class EggHuntEvent : BaseHuntEvent
{
	// Token: 0x06001750 RID: 5968 RVA: 0x000B128B File Offset: 0x000AF48B
	public bool IsEventActive()
	{
		return this.timeAlive > this.warmupTime && this.timeAlive - this.warmupTime < EggHuntEvent.durationSeconds;
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x000B12B4 File Offset: 0x000AF4B4
	public override void ServerInit()
	{
		base.ServerInit();
		if (EggHuntEvent.serverEvent && base.isServer)
		{
			EggHuntEvent.serverEvent.Kill(global::BaseNetworkable.DestroyMode.None);
			EggHuntEvent.serverEvent = null;
		}
		EggHuntEvent.serverEvent = this;
		base.Invoke(new Action(this.StartEvent), this.warmupTime);
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x000B130A File Offset: 0x000AF50A
	public void StartEvent()
	{
		this.SpawnEggs();
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x000B1314 File Offset: 0x000AF514
	public void SpawnEggsAtPoint(int numEggs, Vector3 pos, Vector3 aimDir, float minDist = 1f, float maxDist = 2f)
	{
		for (int i = 0; i < numEggs; i++)
		{
			Vector3 vector = pos;
			if (aimDir == Vector3.zero)
			{
				aimDir = UnityEngine.Random.onUnitSphere;
			}
			else
			{
				aimDir = AimConeUtil.GetModifiedAimConeDirection(90f, aimDir, true);
			}
			vector = pos + Vector3Ex.Direction2D(pos + aimDir * 10f, pos) * UnityEngine.Random.Range(minDist, maxDist);
			vector.y = TerrainMeta.HeightMap.GetHeight(vector);
			CollectableEasterEgg collectableEasterEgg = GameManager.server.CreateEntity(this.HuntablePrefab[UnityEngine.Random.Range(0, this.HuntablePrefab.Length)].resourcePath, vector, default(Quaternion), true) as CollectableEasterEgg;
			collectableEasterEgg.Spawn();
			this._spawnedEggs.Add(collectableEasterEgg);
		}
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x000B13E0 File Offset: 0x000AF5E0
	[ContextMenu("SpawnDebug")]
	public void SpawnEggs()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.SpawnEggsAtPoint(UnityEngine.Random.Range(4, 6) + Mathf.RoundToInt(basePlayer.eggVision), basePlayer.transform.position, basePlayer.eyes.BodyForward(), 15f, 25f);
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000B1464 File Offset: 0x000AF664
	public void RandPickup()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
		}
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000B14B0 File Offset: 0x000AF6B0
	public void EggCollected(global::BasePlayer player)
	{
		EggHuntEvent.EggHunter eggHunter;
		if (this._eggHunters.ContainsKey(player.userID))
		{
			eggHunter = this._eggHunters[player.userID];
		}
		else
		{
			eggHunter = new EggHuntEvent.EggHunter();
			eggHunter.displayName = player.displayName;
			eggHunter.userid = player.userID;
			this._eggHunters.Add(player.userID, eggHunter);
		}
		if (eggHunter == null)
		{
			Debug.LogWarning("Easter error");
			return;
		}
		eggHunter.numEggs++;
		this.QueueUpdate();
		int num = (((float)Mathf.RoundToInt(player.eggVision) * 0.5f < 1f) ? UnityEngine.Random.Range(0, 2) : 1);
		this.SpawnEggsAtPoint(UnityEngine.Random.Range(1 + num, 2 + num), player.transform.position, player.eyes.BodyForward(), 15f, 25f);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000B158D File Offset: 0x000AF78D
	public void QueueUpdate()
	{
		if (base.IsInvoking(new Action(this.DoNetworkUpdate)))
		{
			return;
		}
		base.Invoke(new Action(this.DoNetworkUpdate), 2f);
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x00007D2F File Offset: 0x00005F2F
	public void DoNetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000B15BB File Offset: 0x000AF7BB
	public static void Sort(List<EggHuntEvent.EggHunter> hunterList)
	{
		hunterList.Sort((EggHuntEvent.EggHunter a, EggHuntEvent.EggHunter b) => b.numEggs.CompareTo(a.numEggs));
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000B15E4 File Offset: 0x000AF7E4
	public List<EggHuntEvent.EggHunter> GetTopHunters()
	{
		List<EggHuntEvent.EggHunter> list = Facepunch.Pool.GetList<EggHuntEvent.EggHunter>();
		foreach (KeyValuePair<ulong, EggHuntEvent.EggHunter> keyValuePair in this._eggHunters)
		{
			list.Add(keyValuePair.Value);
		}
		EggHuntEvent.Sort(list);
		return list;
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000B164C File Offset: 0x000AF84C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.eggHunt = Facepunch.Pool.Get<EggHunt>();
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		info.msg.eggHunt.hunters = Facepunch.Pool.GetList<EggHunt.EggHunter>();
		for (int i = 0; i < Mathf.Min(10, topHunters.Count); i++)
		{
			EggHunt.EggHunter eggHunter = Facepunch.Pool.Get<EggHunt.EggHunter>();
			eggHunter.displayName = topHunters[i].displayName;
			eggHunter.numEggs = topHunters[i].numEggs;
			eggHunter.playerID = topHunters[i].userid;
			info.msg.eggHunt.hunters.Add(eggHunter);
		}
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x000B16F8 File Offset: 0x000AF8F8
	public void CleanupEggs()
	{
		foreach (CollectableEasterEgg collectableEasterEgg in this._spawnedEggs)
		{
			if (collectableEasterEgg != null)
			{
				collectableEasterEgg.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x000B1754 File Offset: 0x000AF954
	public void Cooldown()
	{
		base.CancelInvoke(new Action(this.Cooldown));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x000B1770 File Offset: 0x000AF970
	public virtual void PrintWinnersAndAward()
	{
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		if (topHunters.Count > 0)
		{
			EggHuntEvent.EggHunter eggHunter = topHunters[0];
			Chat.Broadcast(string.Concat(new object[] { eggHunter.displayName, " is the top bunny with ", eggHunter.numEggs, " eggs collected." }), "", "#eee", 0UL);
			for (int i = 0; i < topHunters.Count; i++)
			{
				EggHuntEvent.EggHunter eggHunter2 = topHunters[i];
				global::BasePlayer basePlayer = global::BasePlayer.FindByID(eggHunter2.userid);
				if (basePlayer)
				{
					basePlayer.ChatMessage(string.Concat(new object[]
					{
						"You placed ",
						i + 1,
						" of ",
						topHunters.Count,
						" with ",
						topHunters[i].numEggs,
						" eggs collected."
					}));
				}
				else
				{
					Debug.LogWarning("EggHuntEvent Printwinners could not find player with id :" + eggHunter2.userid);
				}
			}
			int num = 0;
			while (num < this.placementAwards.Length && num < topHunters.Count)
			{
				global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(topHunters[num].userid);
				if (basePlayer2)
				{
					basePlayer2.inventory.GiveItem(ItemManager.Create(this.placementAwards[num].itemDef, (int)this.placementAwards[num].amount, 0UL), basePlayer2.inventory.containerMain, false);
					basePlayer2.ChatMessage(string.Concat(new object[]
					{
						"You received ",
						(int)this.placementAwards[num].amount,
						"x ",
						this.placementAwards[num].itemDef.displayName.english,
						" as an award!"
					}));
				}
				num++;
			}
			return;
		}
		Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", 0UL);
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x000B1985 File Offset: 0x000AFB85
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			EggHuntEvent.serverEvent = null;
			return;
		}
		EggHuntEvent.clientEvent = null;
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000B19A4 File Offset: 0x000AFBA4
	public void Update()
	{
		this.timeAlive += UnityEngine.Time.deltaTime;
		if (base.isServer && !base.IsDestroyed)
		{
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds - this.warnTime)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
			}
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds && !base.IsInvoking(new Action(this.Cooldown)))
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				this.CleanupEggs();
				this.PrintWinnersAndAward();
				base.Invoke(new Action(this.Cooldown), 10f);
			}
		}
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x000B1A5C File Offset: 0x000AFC5C
	public float GetTimeRemaining()
	{
		float num = EggHuntEvent.durationSeconds - this.timeAlive;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x04000FED RID: 4077
	public float warmupTime = 10f;

	// Token: 0x04000FEE RID: 4078
	public float cooldownTime = 10f;

	// Token: 0x04000FEF RID: 4079
	public float warnTime = 20f;

	// Token: 0x04000FF0 RID: 4080
	public float timeAlive;

	// Token: 0x04000FF1 RID: 4081
	public static EggHuntEvent serverEvent = null;

	// Token: 0x04000FF2 RID: 4082
	public static EggHuntEvent clientEvent = null;

	// Token: 0x04000FF3 RID: 4083
	[NonSerialized]
	public static float durationSeconds = 180f;

	// Token: 0x04000FF4 RID: 4084
	private Dictionary<ulong, EggHuntEvent.EggHunter> _eggHunters = new Dictionary<ulong, EggHuntEvent.EggHunter>();

	// Token: 0x04000FF5 RID: 4085
	public List<CollectableEasterEgg> _spawnedEggs = new List<CollectableEasterEgg>();

	// Token: 0x04000FF6 RID: 4086
	public ItemAmount[] placementAwards;

	// Token: 0x02000C3C RID: 3132
	public class EggHunter
	{
		// Token: 0x0400430F RID: 17167
		public ulong userid;

		// Token: 0x04004310 RID: 17168
		public string displayName;

		// Token: 0x04004311 RID: 17169
		public int numEggs;
	}
}
