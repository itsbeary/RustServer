using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000758 RID: 1880
public class ItemSkinDirectory : ScriptableObject
{
	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x0600346B RID: 13419 RVA: 0x00143CD4 File Offset: 0x00141ED4
	public static ItemSkinDirectory Instance
	{
		get
		{
			if (ItemSkinDirectory._Instance == null)
			{
				ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
				if (ItemSkinDirectory._Instance == null)
				{
					throw new Exception("Couldn't load assets/skins.asset");
				}
				if (ItemSkinDirectory._Instance.skins == null || ItemSkinDirectory._Instance.skins.Length == 0)
				{
					throw new Exception("Loaded assets/skins.asset but something is wrong");
				}
			}
			return ItemSkinDirectory._Instance;
		}
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x00143D40 File Offset: 0x00141F40
	public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
	{
		return ItemSkinDirectory.Instance.skins.Where((ItemSkinDirectory.Skin x) => x.isSkin && x.itemid == item.itemid).ToArray<ItemSkinDirectory.Skin>();
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x00143D7C File Offset: 0x00141F7C
	public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
	{
		return ItemSkinDirectory.Instance.skins.Where((ItemSkinDirectory.Skin x) => x.id == id).FirstOrDefault<ItemSkinDirectory.Skin>();
	}

	// Token: 0x04002ADC RID: 10972
	private static ItemSkinDirectory _Instance;

	// Token: 0x04002ADD RID: 10973
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x02000E75 RID: 3701
	[Serializable]
	public struct Skin
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060052CC RID: 21196 RVA: 0x001B1168 File Offset: 0x001AF368
		public SteamInventoryItem invItem
		{
			get
			{
				if (this._invItem == null && !string.IsNullOrEmpty(this.name))
				{
					this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
				}
				return this._invItem;
			}
		}

		// Token: 0x04004BF4 RID: 19444
		public int id;

		// Token: 0x04004BF5 RID: 19445
		public int itemid;

		// Token: 0x04004BF6 RID: 19446
		public string name;

		// Token: 0x04004BF7 RID: 19447
		public bool isSkin;

		// Token: 0x04004BF8 RID: 19448
		private SteamInventoryItem _invItem;
	}
}
