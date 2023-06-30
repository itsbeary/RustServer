using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000449 RID: 1097
public class PlayerBlueprints : EntityComponent<global::BasePlayer>
{
	// Token: 0x060024C7 RID: 9415 RVA: 0x000E99A8 File Offset: 0x000E7BA8
	internal void Reset()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (persistantPlayerInfo.unlockedItems != null)
		{
			persistantPlayerInfo.unlockedItems.Clear();
		}
		else
		{
			persistantPlayerInfo.unlockedItems = Pool.GetList<int>();
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000E99FC File Offset: 0x000E7BFC
	internal void UnlockAll()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		foreach (ItemBlueprint itemBlueprint in ItemManager.bpList)
		{
			if (itemBlueprint.userCraftable && !itemBlueprint.defaultBlueprint && !persistantPlayerInfo.unlockedItems.Contains(itemBlueprint.targetItem.itemid))
			{
				persistantPlayerInfo.unlockedItems.Add(itemBlueprint.targetItem.itemid);
			}
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdateImmediate(false);
		base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", 0);
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000E9AC4 File Offset: 0x000E7CC4
	public bool IsUnlocked(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		return persistantPlayerInfo.unlockedItems != null && persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000E9AF8 File Offset: 0x000E7CF8
	public void Unlock(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (!persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
		{
			persistantPlayerInfo.unlockedItems.Add(itemDef.itemid);
			base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
			base.baseEntity.SendNetworkUpdateImmediate(false);
			base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", itemDef.itemid);
			base.baseEntity.stats.Add("blueprint_studied", 1, (Stats)5);
		}
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000E9B84 File Offset: 0x000E7D84
	public bool HasUnlocked(ItemDefinition targetItem)
	{
		if (targetItem.Blueprint)
		{
			if (targetItem.Blueprint.NeedsSteamItem)
			{
				if (targetItem.steamItem != null && !this.steamInventory.HasItem(targetItem.steamItem.id))
				{
					return false;
				}
				if (base.baseEntity.UnlockAllSkins)
				{
					return true;
				}
				if (targetItem.steamItem == null)
				{
					bool flag = false;
					foreach (ItemSkinDirectory.Skin skin in targetItem.skins)
					{
						if (this.steamInventory.HasItem(skin.id))
						{
							flag = true;
							break;
						}
					}
					if (!flag && targetItem.skins2 != null)
					{
						foreach (IPlayerItemDefinition playerItemDefinition in targetItem.skins2)
						{
							if (this.steamInventory.HasItem(playerItemDefinition.DefinitionId))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			else if (targetItem.Blueprint.NeedsSteamDLC)
			{
				if (base.baseEntity.UnlockAllSkins)
				{
					return true;
				}
				if (targetItem.steamDlc != null && targetItem.steamDlc.HasLicense(base.baseEntity.userID))
				{
					return true;
				}
			}
		}
		int[] defaultBlueprints = ItemManager.defaultBlueprints;
		for (int i = 0; i < defaultBlueprints.Length; i++)
		{
			if (defaultBlueprints[i] == targetItem.itemid)
			{
				return true;
			}
		}
		return base.baseEntity.isServer && this.IsUnlocked(targetItem);
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000E9CF4 File Offset: 0x000E7EF4
	public bool CanCraft(int itemid, int skinItemId, ulong playerId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		return !(itemDefinition == null) && (skinItemId == 0 || base.baseEntity.UnlockAllSkins || this.CheckSkinOwnership(skinItemId, playerId)) && base.baseEntity.currentCraftLevel >= (float)itemDefinition.Blueprint.workbenchLevelRequired && this.HasUnlocked(itemDefinition);
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000E9D58 File Offset: 0x000E7F58
	public bool CheckSkinOwnership(int skinItemId, ulong playerId)
	{
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId(skinItemId);
		return (skin.invItem != null && skin.invItem.HasUnlocked(playerId)) || this.steamInventory.HasItem(skinItemId);
	}

	// Token: 0x04001CD0 RID: 7376
	public SteamInventory steamInventory;
}
