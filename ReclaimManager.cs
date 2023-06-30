using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class ReclaimManager : global::BaseEntity
{
	// Token: 0x17000383 RID: 899
	// (get) Token: 0x060029D2 RID: 10706 RVA: 0x0010070A File Offset: 0x000FE90A
	public static global::ReclaimManager instance
	{
		get
		{
			return global::ReclaimManager._instance;
		}
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x00100714 File Offset: 0x000FE914
	public int AddPlayerReclaim(ulong victimID, List<global::Item> itemList, ulong killerID = 0UL, string killerString = "", int reclaimIDToUse = -1)
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
		for (int i = itemList.Count - 1; i >= 0; i--)
		{
			itemList[i].MoveToContainer(playerReclaimEntry.inventory, -1, true, false, null, true);
		}
		if (reclaimIDToUse == -1)
		{
			this.lastReclaimID++;
			reclaimIDToUse = this.lastReclaimID;
		}
		playerReclaimEntry.victimID = victimID;
		playerReclaimEntry.killerID = killerID;
		playerReclaimEntry.killerString = killerString;
		playerReclaimEntry.id = reclaimIDToUse;
		this.entries.Add(playerReclaimEntry);
		return reclaimIDToUse;
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x0010079C File Offset: 0x000FE99C
	public void DoCleanup()
	{
		for (int i = this.entries.Count - 1; i >= 0; i--)
		{
			global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.entries[i];
			if (playerReclaimEntry.inventory.itemList.Count == 0 || playerReclaimEntry.timeAlive / 60f > global::ReclaimManager.reclaim_expire_minutes)
			{
				this.RemoveEntry(playerReclaimEntry);
			}
		}
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x001007FC File Offset: 0x000FE9FC
	public void TickEntries()
	{
		float num = Time.realtimeSinceStartup - this.lastTickTime;
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			playerReclaimEntry.timeAlive += num;
		}
		this.lastTickTime = Time.realtimeSinceStartup;
		this.DoCleanup();
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x00100874 File Offset: 0x000FEA74
	public bool HasReclaims(ulong playerID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.inventory.itemList.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x001008E4 File Offset: 0x000FEAE4
	public global::ReclaimManager.PlayerReclaimEntry GetReclaimForPlayer(ulong playerID, int reclaimID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.id == reclaimID)
			{
				return playerReclaimEntry;
			}
		}
		return null;
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x0010094C File Offset: 0x000FEB4C
	public bool GetReclaimsForPlayer(ulong playerID, ref List<global::ReclaimManager.PlayerReclaimEntry> list)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID)
			{
				list.Add(playerReclaimEntry);
			}
		}
		return list.Count > 0;
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x001009B4 File Offset: 0x000FEBB4
	public global::ReclaimManager.PlayerReclaimEntry NewEntry()
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = Pool.Get<global::ReclaimManager.PlayerReclaimEntry>();
		playerReclaimEntry.Init();
		return playerReclaimEntry;
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x001009C1 File Offset: 0x000FEBC1
	public void RemoveEntry(global::ReclaimManager.PlayerReclaimEntry entry)
	{
		entry.Cleanup();
		this.entries.Remove(entry);
		Pool.Free<global::ReclaimManager.PlayerReclaimEntry>(ref entry);
		entry = null;
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x001009E0 File Offset: 0x000FEBE0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.reclaimManager != null)
		{
			this.lastReclaimID = info.msg.reclaimManager.lastReclaimID;
			foreach (ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo in info.msg.reclaimManager.reclaimEntries)
			{
				global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
				playerReclaimEntry.killerID = reclaimInfo.killerID;
				playerReclaimEntry.victimID = reclaimInfo.victimID;
				playerReclaimEntry.killerString = reclaimInfo.killerString;
				playerReclaimEntry.inventory.Load(reclaimInfo.inventory);
				this.entries.Add(playerReclaimEntry);
			}
		}
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x00100AB8 File Offset: 0x000FECB8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.reclaimManager = Pool.Get<ProtoBuf.ReclaimManager>();
			info.msg.reclaimManager.reclaimEntries = Pool.GetList<ProtoBuf.ReclaimManager.ReclaimInfo>();
			info.msg.reclaimManager.lastReclaimID = this.lastReclaimID;
			foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
			{
				ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo = Pool.Get<ProtoBuf.ReclaimManager.ReclaimInfo>();
				reclaimInfo.killerID = playerReclaimEntry.killerID;
				reclaimInfo.victimID = playerReclaimEntry.victimID;
				reclaimInfo.killerString = playerReclaimEntry.killerString;
				reclaimInfo.inventory = playerReclaimEntry.inventory.Save();
				info.msg.reclaimManager.reclaimEntries.Add(reclaimInfo);
			}
		}
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x00100BA4 File Offset: 0x000FEDA4
	public override void ServerInit()
	{
		base.InvokeRepeating(new Action(this.TickEntries), 1f, 60f);
		global::ReclaimManager._instance = this;
		base.ServerInit();
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x00100BCE File Offset: 0x000FEDCE
	internal override void DoServerDestroy()
	{
		global::ReclaimManager._instance = null;
		base.DoServerDestroy();
	}

	// Token: 0x040021E2 RID: 8674
	private const int defaultReclaims = 128;

	// Token: 0x040021E3 RID: 8675
	private const int reclaimSlotCount = 40;

	// Token: 0x040021E4 RID: 8676
	private int lastReclaimID;

	// Token: 0x040021E5 RID: 8677
	[ServerVar]
	public static float reclaim_expire_minutes = 120f;

	// Token: 0x040021E6 RID: 8678
	private static global::ReclaimManager _instance;

	// Token: 0x040021E7 RID: 8679
	public List<global::ReclaimManager.PlayerReclaimEntry> entries = new List<global::ReclaimManager.PlayerReclaimEntry>();

	// Token: 0x040021E8 RID: 8680
	private float lastTickTime;

	// Token: 0x02000D50 RID: 3408
	public class PlayerReclaimEntry
	{
		// Token: 0x060050C6 RID: 20678 RVA: 0x001AA520 File Offset: 0x001A8720
		public void Init()
		{
			this.inventory = Pool.Get<global::ItemContainer>();
			this.inventory.entityOwner = global::ReclaimManager.instance;
			this.inventory.allowedContents = global::ItemContainer.ContentsType.Generic;
			this.inventory.SetOnlyAllowedItem(null);
			this.inventory.maxStackSize = 0;
			this.inventory.ServerInitialize(null, 40);
			this.inventory.canAcceptItem = null;
			this.inventory.GiveUID();
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x001AA594 File Offset: 0x001A8794
		public void Cleanup()
		{
			this.timeAlive = 0f;
			this.killerID = 0UL;
			this.killerString = "";
			this.victimID = 0UL;
			this.id = -2;
			if (this.inventory != null)
			{
				this.inventory.Clear();
				Pool.Free<global::ItemContainer>(ref this.inventory);
			}
		}

		// Token: 0x0400476F RID: 18287
		public ulong killerID;

		// Token: 0x04004770 RID: 18288
		public string killerString;

		// Token: 0x04004771 RID: 18289
		public ulong victimID;

		// Token: 0x04004772 RID: 18290
		public float timeAlive;

		// Token: 0x04004773 RID: 18291
		public int id;

		// Token: 0x04004774 RID: 18292
		public global::ItemContainer inventory;
	}
}
