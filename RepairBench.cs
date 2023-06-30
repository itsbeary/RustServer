using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BE RID: 190
public class RepairBench : StorageContainer
{
	// Token: 0x06001126 RID: 4390 RVA: 0x0008CAE4 File Offset: 0x0008ACE4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepairBench.OnRpcMessage", 0))
		{
			if (rpc == 1942825351U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ChangeSkin ");
				}
				using (TimeWarning.New("ChangeSkin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1942825351U, "ChangeSkin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ChangeSkin(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ChangeSkin");
					}
				}
				return true;
			}
			if (rpc == 1178348163U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RepairItem ");
				}
				using (TimeWarning.New("RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1178348163U, "RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RepairItem(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RepairItem");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0008CDE4 File Offset: 0x0008AFE4
	public static float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0008CDF9 File Offset: 0x0008AFF9
	public static float RepairCostFraction(Item itemToRepair)
	{
		return RepairBench.GetRepairFraction(itemToRepair) * 0.2f;
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0008CE08 File Offset: 0x0008B008
	public static void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
		}
		RepairBench.StripComponentRepairCost(allIngredients);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0008CE74 File Offset: 0x0008B074
	public static void StripComponentRepairCost(List<ItemAmount> allIngredients)
	{
		if (allIngredients == null)
		{
			return;
		}
		for (int i = 0; i < allIngredients.Count; i++)
		{
			ItemAmount itemAmount = allIngredients[i];
			if (itemAmount.itemDef.category == ItemCategory.Component)
			{
				if (itemAmount.itemDef.Blueprint != null)
				{
					bool flag = false;
					ItemAmount itemAmount2 = itemAmount.itemDef.Blueprint.ingredients[0];
					foreach (ItemAmount itemAmount3 in allIngredients)
					{
						if (itemAmount3.itemDef == itemAmount2.itemDef)
						{
							itemAmount3.amount += itemAmount2.amount * itemAmount.amount;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						allIngredients.Add(new ItemAmount(itemAmount2.itemDef, itemAmount2.amount * itemAmount.amount));
					}
				}
				allIngredients.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0008CF80 File Offset: 0x0008B180
	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning(toPrint);
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0008CF90 File Offset: 0x0008B190
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ChangeSkin(BaseEntity.RPCMessage msg)
	{
		if (UnityEngine.Time.realtimeSinceStartup < this.nextSkinChangeTime)
		{
			return;
		}
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		bool flag = false;
		if (msg.player.UnlockAllSkins)
		{
			flag = true;
		}
		if (num != 0 && !flag && !player.blueprints.CheckSkinOwnership(num, player.userID))
		{
			this.debugprint("RepairBench.ChangeSkin player does not have item :" + num + ":");
			return;
		}
		ulong Skin = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (Skin == slot.skin && slot.info.isRedirectOf == null)
		{
			this.debugprint(string.Concat(new object[] { "RepairBench.ChangeSkin cannot apply same skin twice : ", Skin, ": ", slot.skin }));
			return;
		}
		this.nextSkinChangeTime = UnityEngine.Time.realtimeSinceStartup + 0.75f;
		ItemSkinDirectory.Skin skin = slot.info.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		if (slot.info.isRedirectOf != null)
		{
			Skin = ItemDefinition.FindSkin(slot.info.isRedirectOf.itemid, num);
			skin = slot.info.isRedirectOf.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		}
		ItemSkin itemSkin = ((skin.id == 0) ? null : (skin.invItem as ItemSkin));
		if ((itemSkin && (itemSkin.Redirect != null || slot.info.isRedirectOf != null)) || (!itemSkin && slot.info.isRedirectOf != null))
		{
			ItemDefinition itemDefinition = ((itemSkin != null) ? itemSkin.Redirect : slot.info.isRedirectOf);
			bool flag2 = false;
			if (itemSkin != null && itemSkin.Redirect == null && slot.info.isRedirectOf != null)
			{
				itemDefinition = slot.info.isRedirectOf;
				flag2 = num != 0;
			}
			float condition = slot.condition;
			float maxCondition = slot.maxCondition;
			int amount = slot.amount;
			int num2 = 0;
			ItemDefinition itemDefinition2 = null;
			BaseProjectile baseProjectile;
			if (slot.GetHeldEntity() != null && (baseProjectile = slot.GetHeldEntity() as BaseProjectile) != null && baseProjectile.primaryMagazine != null)
			{
				num2 = baseProjectile.primaryMagazine.contents;
				itemDefinition2 = baseProjectile.primaryMagazine.ammoType;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			if (slot.contents != null && slot.contents.itemList != null && slot.contents.itemList.Count > 0)
			{
				foreach (Item item in slot.contents.itemList)
				{
					list.Add(item);
				}
				foreach (Item item2 in list)
				{
					item2.RemoveFromContainer();
				}
			}
			slot.Remove(0f);
			ItemManager.DoRemoves();
			Item item3 = ItemManager.Create(itemDefinition, 1, 0UL);
			item3.MoveToContainer(base.inventory, 0, false, false, null, true);
			item3.maxCondition = maxCondition;
			item3.condition = condition;
			item3.amount = amount;
			BaseProjectile baseProjectile2;
			if (item3.GetHeldEntity() != null && (baseProjectile2 = item3.GetHeldEntity() as BaseProjectile) != null)
			{
				if (baseProjectile2.primaryMagazine != null)
				{
					baseProjectile2.primaryMagazine.contents = num2;
					baseProjectile2.primaryMagazine.ammoType = itemDefinition2;
				}
				baseProjectile2.ForceModsChanged();
			}
			if (list.Count > 0 && item3.contents != null)
			{
				foreach (Item item4 in list)
				{
					item4.MoveToContainer(item3.contents, -1, true, false, null, true);
				}
			}
			Facepunch.Pool.FreeList<Item>(ref list);
			if (flag2)
			{
				this.ApplySkinToItem(item3, Skin);
			}
			Analytics.Server.SkinUsed(item3.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, item3, Skin);
		}
		else
		{
			this.ApplySkinToItem(slot, Skin);
			Analytics.Server.SkinUsed(slot.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, slot, Skin);
		}
		if (this.skinchangeEffect.isValid)
		{
			Effect.server.Run(this.skinchangeEffect.resourcePath, this, 0U, new Vector3(0f, 1.5f, 0f), Vector3.zero, null, false);
		}
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0008D494 File Offset: 0x0008B694
	private void ApplySkinToItem(Item item, ulong Skin)
	{
		item.skin = Skin;
		item.MarkDirty();
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.skinID = Skin;
			heldEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x0008D4CC File Offset: 0x0008B6CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RepairItem(BaseEntity.RPCMessage msg)
	{
		Item slot = base.inventory.GetSlot(0);
		BasePlayer player = msg.player;
		RepairBench.RepairAnItem(slot, player, this, this.maxConditionLostOnRepair, true);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		return 0;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0008D4FC File Offset: 0x0008B6FC
	public static void RepairAnItem(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
	{
		if (itemToRepair == null)
		{
			return;
		}
		ItemDefinition info = itemToRepair.info;
		ItemBlueprint component = info.GetComponent<ItemBlueprint>();
		if (!component)
		{
			return;
		}
		if (!info.condition.repairable)
		{
			return;
		}
		if (itemToRepair.condition == itemToRepair.maxCondition)
		{
			return;
		}
		if (mustKnowBlueprint)
		{
			ItemDefinition itemDefinition = ((info.isRedirectOf != null) ? info.isRedirectOf : info);
			if (!player.blueprints.HasUnlocked(itemDefinition) && (!(itemDefinition.Blueprint != null) || itemDefinition.Blueprint.isResearchable))
			{
				return;
			}
		}
		float num = RepairBench.RepairCostFraction(itemToRepair);
		bool flag = false;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		RepairBench.GetRepairCostList(component, list);
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category != ItemCategory.Component)
			{
				int amount = player.inventory.GetAmount(itemAmount.itemDef.itemid);
				if (Mathf.CeilToInt(itemAmount.amount * num) > amount)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Facepunch.Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount itemAmount2 in list)
		{
			if (itemAmount2.itemDef.category != ItemCategory.Component)
			{
				int num2 = Mathf.CeilToInt(itemAmount2.amount * num);
				player.inventory.Take(null, itemAmount2.itemid, num2);
				Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "repair", itemAmount2.itemDef.shortname, num2, repairBenchEntity, null, false, null, 0UL, null, itemToRepair, null);
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		float conditionNormalized = itemToRepair.conditionNormalized;
		float maxConditionNormalized = itemToRepair.maxConditionNormalized;
		itemToRepair.DoRepair(maxConditionLostOnRepair);
		Analytics.Azure.OnItemRepaired(player, repairBenchEntity, itemToRepair, conditionNormalized, maxConditionNormalized);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[] { "Item repaired! condition : ", itemToRepair.condition, "/", itemToRepair.maxCondition }));
		}
		Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", repairBenchEntity, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x04000AC2 RID: 2754
	public float maxConditionLostOnRepair = 0.2f;

	// Token: 0x04000AC3 RID: 2755
	public GameObjectRef skinchangeEffect;

	// Token: 0x04000AC4 RID: 2756
	public const float REPAIR_COST_FRACTION = 0.2f;

	// Token: 0x04000AC5 RID: 2757
	private float nextSkinChangeTime;
}
