using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000759 RID: 1881
[CreateAssetMenu(menuName = "Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
	// Token: 0x0600346F RID: 13423 RVA: 0x00143DB6 File Offset: 0x00141FB6
	public ItemDefinition GetBlueprintBaseDef()
	{
		return ItemManager.FindItemDefinition("blueprintbase");
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x00143DC4 File Offset: 0x00141FC4
	public void SpawnIntoContainer(ItemContainer container)
	{
		if (this.subSpawn != null && this.subSpawn.Length != 0)
		{
			this.SubCategoryIntoContainer(container);
			return;
		}
		if (this.items != null)
		{
			foreach (ItemAmountRanged itemAmountRanged in this.items)
			{
				if (itemAmountRanged != null)
				{
					Item item2;
					if (itemAmountRanged.itemDef.spawnAsBlueprint)
					{
						ItemDefinition blueprintBaseDef = this.GetBlueprintBaseDef();
						if (blueprintBaseDef == null)
						{
							goto IL_EB;
						}
						Item item = ItemManager.Create(blueprintBaseDef, 1, 0UL);
						item.blueprintTarget = itemAmountRanged.itemDef.itemid;
						item2 = item;
					}
					else
					{
						item2 = ItemManager.CreateByItemID(itemAmountRanged.itemid, (int)itemAmountRanged.GetAmount(), 0UL);
					}
					if (item2 != null)
					{
						item2.OnVirginSpawn();
						if (!item2.MoveToContainer(container, -1, true, false, null, true))
						{
							if (container.playerOwner)
							{
								item2.Drop(container.playerOwner.GetDropPosition(), container.playerOwner.GetDropVelocity(), default(Quaternion));
							}
							else
							{
								item2.Remove(0f);
							}
						}
					}
				}
				IL_EB:;
			}
		}
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x00143ECC File Offset: 0x001420CC
	private void SubCategoryIntoContainer(ItemContainer container)
	{
		int num = this.subSpawn.Sum((LootSpawn.Entry x) => x.weight);
		int num2 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < this.subSpawn.Length; i++)
		{
			if (!(this.subSpawn[i].category == null))
			{
				num -= this.subSpawn[i].weight;
				if (num2 >= num)
				{
					for (int j = 0; j < 1 + this.subSpawn[i].extraSpawns; j++)
					{
						this.subSpawn[i].category.SpawnIntoContainer(container);
					}
					return;
				}
			}
		}
		string text = ((container.entityOwner != null) ? container.entityOwner.name : "Unknown");
		Debug.LogWarning(string.Format("SubCategoryIntoContainer for loot '{0}' for entity '{1}' ended with randomWeight ({2}) < totalWeight ({3}). This should never happen! ", new object[] { base.name, text, num2, num }), this);
	}

	// Token: 0x04002ADE RID: 10974
	public ItemAmountRanged[] items;

	// Token: 0x04002ADF RID: 10975
	public LootSpawn.Entry[] subSpawn;

	// Token: 0x02000E78 RID: 3704
	[Serializable]
	public struct Entry
	{
		// Token: 0x04004BFB RID: 19451
		[Tooltip("If this category is chosen, we will spawn 1+ this amount")]
		public int extraSpawns;

		// Token: 0x04004BFC RID: 19452
		[Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
		public LootSpawn category;

		// Token: 0x04004BFD RID: 19453
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}
}
