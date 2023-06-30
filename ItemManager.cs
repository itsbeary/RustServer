using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000611 RID: 1553
public class ItemManager
{
	// Token: 0x06002E04 RID: 11780 RVA: 0x00114CA8 File Offset: 0x00112EA8
	public static void InvalidateWorkshopSkinCache()
	{
		if (ItemManager.itemList == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			itemDefinition.InvalidateWorkshopSkinCache();
		}
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x00114D00 File Offset: 0x00112F00
	public static void Initialize()
	{
		if (ItemManager.itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject[] array = FileSystem.LoadAllFromBundle<GameObject>("items.preload.bundle", "l:ItemDefinition");
		if (array.Length == 0)
		{
			throw new Exception("items.preload.bundle has no items!");
		}
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log("Loading Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds");
		}
		List<ItemDefinition> list = (from x in array
			select x.GetComponent<ItemDefinition>() into x
			where x != null
			select x).ToList<ItemDefinition>();
		List<ItemBlueprint> list2 = (from x in array
			select x.GetComponent<ItemBlueprint>() into x
			where x != null && x.userCraftable
			select x).ToList<ItemBlueprint>();
		Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
		Dictionary<string, ItemDefinition> dictionary2 = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (ItemDefinition itemDefinition in list)
		{
			itemDefinition.Initialize(list);
			if (dictionary.ContainsKey(itemDefinition.itemid))
			{
				ItemDefinition itemDefinition2 = dictionary[itemDefinition.itemid];
				UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Item ID duplicate ", itemDefinition.itemid, " (", itemDefinition.name, ") - have you given your items unique shortnames?" }), itemDefinition.gameObject);
				UnityEngine.Debug.LogWarning("Other item is " + itemDefinition2.name, itemDefinition2);
			}
			else if (string.IsNullOrEmpty(itemDefinition.shortname))
			{
				UnityEngine.Debug.LogWarning(string.Format("{0} has a null short name! id: {1} {2}", itemDefinition, itemDefinition.itemid, itemDefinition.displayName.english));
			}
			else
			{
				dictionary.Add(itemDefinition.itemid, itemDefinition);
				dictionary2.Add(itemDefinition.shortname, itemDefinition);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"Building Items Took: ",
				(stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString(),
				" seconds / Items: ",
				list.Count.ToString(),
				" / Blueprints: ",
				list2.Count.ToString()
			}));
		}
		ItemManager.defaultBlueprints = (from x in list2
			where !x.NeedsSteamItem && !x.NeedsSteamDLC && x.defaultBlueprint
			select x.targetItem.itemid).ToArray<int>();
		ItemManager.itemList = list;
		ItemManager.bpList = list2;
		ItemManager.itemDictionary = dictionary;
		ItemManager.itemDictionaryByName = dictionary2;
		ItemManager.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x0011506C File Offset: 0x0011326C
	public static global::Item CreateByName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x001150B8 File Offset: 0x001132B8
	public static global::Item CreateByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x00115124 File Offset: 0x00113324
	public static ItemDefinition FindDefinitionByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		return itemDefinition;
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x00115178 File Offset: 0x00113378
	public static global::Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.Create(itemDefinition, iAmount, skin);
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x001151A0 File Offset: 0x001133A0
	public static global::Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0UL)
	{
		ItemManager.TrySkinChangeItem(ref template, ref skin);
		if (template == null)
		{
			UnityEngine.Debug.LogWarning("Creating invalid/missing item!");
			return null;
		}
		global::Item item = new global::Item();
		item.isServer = true;
		if (iAmount <= 0)
		{
			UnityEngine.Debug.LogError("Creating item with less than 1 amount! (" + template.displayName.english + ")");
			return null;
		}
		item.info = template;
		item.amount = iAmount;
		item.skin = skin;
		item.Initialize(template);
		return item;
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x0011521C File Offset: 0x0011341C
	private static void TrySkinChangeItem(ref ItemDefinition template, ref ulong skinId)
	{
		if (skinId == 0UL)
		{
			return;
		}
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId((int)skinId);
		if (skin.id == 0)
		{
			return;
		}
		ItemSkin itemSkin = skin.invItem as ItemSkin;
		if (itemSkin == null)
		{
			return;
		}
		if (itemSkin.Redirect == null)
		{
			return;
		}
		template = itemSkin.Redirect;
		skinId = 0UL;
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x00115274 File Offset: 0x00113474
	public static global::Item Load(ProtoBuf.Item load, global::Item created, bool isServer)
	{
		if (created == null)
		{
			created = new global::Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if (created.info == null)
		{
			UnityEngine.Debug.LogWarning("Item loading failed - item is invalid");
			return null;
		}
		if (created.info == ItemManager.blueprintBaseDef && created.blueprintTargetDef == null)
		{
			UnityEngine.Debug.LogWarning("Blueprint item loading failed - invalid item target");
			return null;
		}
		return created;
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x001152E0 File Offset: 0x001134E0
	public static ItemDefinition FindItemDefinition(int itemID)
	{
		ItemManager.Initialize();
		ItemDefinition itemDefinition;
		if (ItemManager.itemDictionary.TryGetValue(itemID, out itemDefinition))
		{
			return itemDefinition;
		}
		return null;
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x00115304 File Offset: 0x00113504
	public static ItemDefinition FindItemDefinition(string shortName)
	{
		ItemManager.Initialize();
		ItemDefinition itemDefinition;
		if (ItemManager.itemDictionaryByName.TryGetValue(shortName, out itemDefinition))
		{
			return itemDefinition;
		}
		return null;
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x00115328 File Offset: 0x00113528
	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return item.GetComponent<ItemBlueprint>();
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x00115330 File Offset: 0x00113530
	public static List<ItemDefinition> GetItemDefinitions()
	{
		ItemManager.Initialize();
		return ItemManager.itemList;
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x0011533C File Offset: 0x0011353C
	public static List<ItemBlueprint> GetBlueprints()
	{
		ItemManager.Initialize();
		return ItemManager.bpList;
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x00115348 File Offset: 0x00113548
	public static void DoRemoves()
	{
		using (TimeWarning.New("DoRemoves", 0))
		{
			for (int i = 0; i < ItemManager.ItemRemoves.Count; i++)
			{
				if (ItemManager.ItemRemoves[i].time <= Time.time)
				{
					global::Item item = ItemManager.ItemRemoves[i].item;
					ItemManager.ItemRemoves.RemoveAt(i--);
					item.DoRemove();
				}
			}
		}
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x001153D0 File Offset: 0x001135D0
	public static void Heartbeat()
	{
		ItemManager.DoRemoves();
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x001153D8 File Offset: 0x001135D8
	public static void RemoveItem(global::Item item, float fTime = 0f)
	{
		Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
		ItemManager.ItemRemove itemRemove = default(ItemManager.ItemRemove);
		itemRemove.item = item;
		itemRemove.time = Time.time + fTime;
		ItemManager.ItemRemoves.Add(itemRemove);
	}

	// Token: 0x040025C4 RID: 9668
	public static List<ItemDefinition> itemList;

	// Token: 0x040025C5 RID: 9669
	public static Dictionary<int, ItemDefinition> itemDictionary;

	// Token: 0x040025C6 RID: 9670
	public static Dictionary<string, ItemDefinition> itemDictionaryByName;

	// Token: 0x040025C7 RID: 9671
	public static List<ItemBlueprint> bpList;

	// Token: 0x040025C8 RID: 9672
	public static int[] defaultBlueprints;

	// Token: 0x040025C9 RID: 9673
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x040025CA RID: 9674
	private static List<ItemManager.ItemRemove> ItemRemoves = new List<ItemManager.ItemRemove>();

	// Token: 0x02000DA0 RID: 3488
	private struct ItemRemove
	{
		// Token: 0x040048B0 RID: 18608
		public global::Item item;

		// Token: 0x040048B1 RID: 18609
		public float time;
	}
}
