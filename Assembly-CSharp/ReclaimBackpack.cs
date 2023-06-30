using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class ReclaimBackpack : StorageContainer
{
	// Token: 0x060016F7 RID: 5879 RVA: 0x000AFEAB File Offset: 0x000AE0AB
	public void InitForPlayer(ulong playerID, int newID)
	{
		this.playerSteamID = playerID;
		this.reclaimID = newID;
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x000AFEBC File Offset: 0x000AE0BC
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
		base.Invoke(new Action(this.RemoveMe), global::ReclaimManager.reclaim_expire_minutes * 60f);
		base.InvokeRandomized(new Action(this.CheckEmpty), 1f, 30f, 3f);
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x000AFF1E File Offset: 0x000AE11E
	public void CheckEmpty()
	{
		if (global::ReclaimManager.instance.GetReclaimForPlayer(this.playerSteamID, this.reclaimID) == null && !this.isBeingLooted)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x000AFF48 File Offset: 0x000AE148
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		if (this.onlyOwnerLoot && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		global::ReclaimManager.PlayerReclaimEntry reclaimForPlayer = global::ReclaimManager.instance.GetReclaimForPlayer(baseEntity.userID, this.reclaimID);
		if (reclaimForPlayer != null)
		{
			for (int i = reclaimForPlayer.inventory.itemList.Count - 1; i >= 0; i--)
			{
				reclaimForPlayer.inventory.itemList[i].MoveToContainer(base.inventory, -1, true, false, null, true);
			}
			global::ReclaimManager.instance.RemoveEntry(reclaimForPlayer);
		}
		this.isBeingLooted = true;
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x000AFFF8 File Offset: 0x000AE1F8
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.isBeingLooted = false;
		if (base.inventory.itemList.Count > 0)
		{
			global::ReclaimManager.instance.AddPlayerReclaim(this.playerSteamID, base.inventory.itemList, 0UL, "", this.reclaimID);
		}
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x000B0050 File Offset: 0x000AE250
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		info.msg.lootableCorpse.underwearSkin = (uint)this.reclaimID;
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x000B00A0 File Offset: 0x000AE2A0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			this.reclaimID = (int)info.msg.lootableCorpse.underwearSkin;
		}
	}

	// Token: 0x04000F24 RID: 3876
	public int reclaimID;

	// Token: 0x04000F25 RID: 3877
	public ulong playerSteamID;

	// Token: 0x04000F26 RID: 3878
	public bool onlyOwnerLoot = true;

	// Token: 0x04000F27 RID: 3879
	public Collider myCollider;

	// Token: 0x04000F28 RID: 3880
	public GameObject art;

	// Token: 0x04000F29 RID: 3881
	private bool isBeingLooted;
}
