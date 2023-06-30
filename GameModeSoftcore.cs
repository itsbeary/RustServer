using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x0200051B RID: 1307
public class GameModeSoftcore : GameModeVanilla
{
	// Token: 0x060029C4 RID: 10692 RVA: 0x001001C6 File Offset: 0x000FE3C6
	protected override void OnCreated()
	{
		base.OnCreated();
		SingletonComponent<ServerMgr>.Instance.CreateImportantEntity<ReclaimManager>(this.reclaimManagerPrefab.resourcePath);
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x001001E4 File Offset: 0x000FE3E4
	public void AddFractionOfContainer(ItemContainer from, ref List<Item> to, float fraction = 1f, bool takeLastItem = false)
	{
		if (from.itemList.Count == 0)
		{
			return;
		}
		fraction = Mathf.Clamp01(fraction);
		int count = from.itemList.Count;
		float num = Mathf.Ceil((float)count * fraction);
		if (count == 1 && num == 1f && !takeLastItem)
		{
			return;
		}
		List<int> list = Facepunch.Pool.GetList<int>();
		for (int i = 0; i < from.capacity; i++)
		{
			if (from.GetSlot(i) != null)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			Facepunch.Pool.FreeList<int>(ref list);
			return;
		}
		int num2 = 0;
		while ((float)num2 < num)
		{
			int num3 = UnityEngine.Random.Range(0, list.Count);
			Item item = from.GetSlot(list[num3]);
			if (item.MaxStackable() > 1)
			{
				foreach (Item item2 in from.itemList)
				{
					if (item2.info == item.info && item2.amount < item.amount && !to.Contains(item2))
					{
						item = item2;
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j] == item2.position)
							{
								num3 = j;
							}
						}
					}
				}
			}
			to.Add(item);
			list.RemoveAt(num3);
			num2++;
		}
		Facepunch.Pool.FreeList<int>(ref list);
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x00100358 File Offset: 0x000FE558
	public List<Item> RemoveItemsFrom(ItemContainer itemContainer, ItemAmount[] types)
	{
		List<Item> list = Facepunch.Pool.GetList<Item>();
		foreach (ItemAmount itemAmount in types)
		{
			int num = 0;
			while ((float)num < itemAmount.amount)
			{
				Item item = itemContainer.FindItemByItemID(itemAmount.itemDef.itemid);
				if (item != null)
				{
					item.RemoveFromContainer();
					list.Add(item);
				}
				num++;
			}
		}
		return list;
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x001003BC File Offset: 0x000FE5BC
	public void ReturnItemsTo(ref List<Item> source, ItemContainer itemContainer)
	{
		foreach (Item item in source)
		{
			item.MoveToContainer(itemContainer, -1, true, false, null, true);
		}
		Facepunch.Pool.FreeList<Item>(ref source);
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x00100418 File Offset: 0x000FE618
	public override void OnPlayerDeath(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		if (victim != null && !victim.IsNpc)
		{
			this.SetInventoryLocked(victim, false);
			int num = 0;
			if (ReclaimManager.instance == null)
			{
				Debug.LogError("No reclaim manage for softcore");
				return;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			List<Item> list2 = this.RemoveItemsFrom(victim.inventory.containerBelt, this.startingGear);
			this.AddFractionOfContainer(victim.inventory.containerBelt, ref list, GameModeSoftcore.reclaim_fraction_belt, false);
			this.AddFractionOfContainer(victim.inventory.containerWear, ref list, GameModeSoftcore.reclaim_fraction_wear, false);
			this.AddFractionOfContainer(victim.inventory.containerMain, ref list, GameModeSoftcore.reclaim_fraction_main, false);
			if (list.Count > 0)
			{
				num = ReclaimManager.instance.AddPlayerReclaim(victim.userID, list, (instigator == null) ? 0UL : instigator.userID, (instigator == null) ? "" : instigator.displayName, -1);
			}
			this.ReturnItemsTo(ref list2, victim.inventory.containerBelt);
			if (list.Count > 0)
			{
				Vector3 vector = victim.transform.position + Vector3.up * 0.25f;
				Quaternion quaternion = Quaternion.Euler(0f, victim.transform.eulerAngles.y, 0f);
				ReclaimBackpack component = GameManager.server.CreateEntity(this.reclaimBackpackPrefab.resourcePath, vector, quaternion, true).GetComponent<ReclaimBackpack>();
				component.InitForPlayer(victim.userID, num);
				component.Spawn();
			}
			Facepunch.Pool.FreeList<Item>(ref list);
		}
		base.OnPlayerDeath(instigator, victim, deathInfo);
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x001005A9 File Offset: 0x000FE7A9
	public override void OnPlayerRespawn(BasePlayer player)
	{
		base.OnPlayerRespawn(player);
		player.ShowToast(GameTip.Styles.Blue_Long, GameModeSoftcore.ReclaimToast, Array.Empty<string>());
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x00030F91 File Offset: 0x0002F191
	public override SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x001005C4 File Offset: 0x000FE7C4
	public override float CorpseRemovalTime(BaseCorpse corpse)
	{
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsSafeZone && monumentInfo.Bounds.Contains(corpse.transform.position))
			{
				return 30f;
			}
		}
		return Server.corpsedespawn;
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x0010064C File Offset: 0x000FE84C
	public void SetInventoryLocked(BasePlayer player, bool wantsLocked)
	{
		player.inventory.containerMain.SetLocked(wantsLocked);
		player.inventory.containerBelt.SetLocked(wantsLocked);
		player.inventory.containerWear.SetLocked(wantsLocked);
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x00100681 File Offset: 0x000FE881
	public override void OnPlayerWounded(BasePlayer instigator, BasePlayer victim, HitInfo info)
	{
		base.OnPlayerWounded(instigator, victim, info);
		this.SetInventoryLocked(victim, true);
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x00100694 File Offset: 0x000FE894
	public override void OnPlayerRevived(BasePlayer instigator, BasePlayer victim)
	{
		this.SetInventoryLocked(victim, false);
		base.OnPlayerRevived(instigator, victim);
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x001006A6 File Offset: 0x000FE8A6
	public override bool CanMoveItemsFrom(PlayerInventory inv, BaseEntity source, Item item)
	{
		if (item.parent != null && item.parent.HasFlag(ItemContainer.Flag.IsPlayer))
		{
			return !item.parent.IsLocked();
		}
		return base.CanMoveItemsFrom(inv, source, item);
	}

	// Token: 0x040021DB RID: 8667
	public GameObjectRef reclaimManagerPrefab;

	// Token: 0x040021DC RID: 8668
	public GameObjectRef reclaimBackpackPrefab;

	// Token: 0x040021DD RID: 8669
	public static readonly Translate.Phrase ReclaimToast = new Translate.Phrase("softcore.reclaim", "You can reclaim some of your lost items by visiting the Outpost or Bandit Town.");

	// Token: 0x040021DE RID: 8670
	public ItemAmount[] startingGear;

	// Token: 0x040021DF RID: 8671
	[ServerVar]
	public static float reclaim_fraction_belt = 0.5f;

	// Token: 0x040021E0 RID: 8672
	[ServerVar]
	public static float reclaim_fraction_wear = 0f;

	// Token: 0x040021E1 RID: 8673
	[ServerVar]
	public static float reclaim_fraction_main = 0.5f;
}
