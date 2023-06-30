using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x0200041A RID: 1050
public class LootContainer : StorageContainer
{
	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x0600238E RID: 9102 RVA: 0x000E2DDB File Offset: 0x000E0FDB
	public bool shouldRefreshContents
	{
		get
		{
			return this.minSecondsBetweenRefresh > 0f && this.maxSecondsBetweenRefresh > 0f;
		}
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000E2DF9 File Offset: 0x000E0FF9
	public override void ResetState()
	{
		this.FirstLooted = false;
		base.ResetState();
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000E2E08 File Offset: 0x000E1008
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.initialLootSpawn)
		{
			this.SpawnLoot();
		}
		if (this.BlockPlayerItemInput && !Rust.Application.isLoadingSave && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, PlayerInventory.IsBirthday(), false, true);
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000E2E63 File Offset: 0x000E1063
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.BlockPlayerItemInput && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000E2E8C File Offset: 0x000E108C
	public virtual void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		base.inventory.Clear();
		ItemManager.DoRemoves();
		this.PopulateLoot();
		if (this.shouldRefreshContents)
		{
			base.Invoke(new Action(this.SpawnLoot), UnityEngine.Random.Range(this.minSecondsBetweenRefresh, this.maxSecondsBetweenRefresh));
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000E2EEE File Offset: 0x000E10EE
	public int ScoreForRarity(Rarity rarity)
	{
		switch (rarity)
		{
		case Rarity.Common:
			return 1;
		case Rarity.Uncommon:
			return 2;
		case Rarity.Rare:
			return 3;
		case Rarity.VeryRare:
			return 4;
		default:
			return 5000;
		}
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000E2F18 File Offset: 0x000E1118
	public virtual void PopulateLoot()
	{
		if (this.LootSpawnSlots.Length != 0)
		{
			foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
			{
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(base.inventory);
					}
				}
			}
		}
		else if (this.lootDefinition != null)
		{
			for (int k = 0; k < this.maxDefinitionsToSpawn; k++)
			{
				this.lootDefinition.SpawnIntoContainer(base.inventory);
			}
		}
		if (this.SpawnType == LootContainer.spawnType.ROADSIDE || this.SpawnType == LootContainer.spawnType.TOWN)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.hasCondition)
				{
					item.condition = UnityEngine.Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
				}
			}
		}
		this.GenerateScrap();
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000E3070 File Offset: 0x000E1270
	public void GenerateScrap()
	{
		if (this.scrapAmount <= 0)
		{
			return;
		}
		if (LootContainer.scrapDef == null)
		{
			LootContainer.scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = this.scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(LootContainer.scrapDef, num, 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				item.Drop(base.transform.position, this.GetInheritedDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000E30F0 File Offset: 0x000E12F0
	public override void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
		base.DropBonusItems(initiator, container);
		if (initiator == null || container == null)
		{
			return;
		}
		BasePlayer basePlayer = initiator as BasePlayer;
		if (basePlayer == null)
		{
			return;
		}
		if (this.scrapAmount > 0 && LootContainer.scrapDef != null)
		{
			float num = ((basePlayer.modifiers != null) ? (1f + basePlayer.modifiers.GetValue(Modifier.ModifierType.Scrap_Yield, 0f)) : 0f);
			if (num > 1f)
			{
				float num2 = basePlayer.modifiers.GetVariableValue(Modifier.ModifierType.Scrap_Yield, 0f);
				float num3 = Mathf.Max((float)this.scrapAmount * num - (float)this.scrapAmount, 0f);
				num2 += num3;
				int num4 = 0;
				if (num2 >= 1f)
				{
					num4 = (int)num2;
					num2 -= (float)num4;
				}
				basePlayer.modifiers.SetVariableValue(Modifier.ModifierType.Scrap_Yield, num2);
				if (num4 > 0)
				{
					Item item = ItemManager.Create(LootContainer.scrapDef, num4, 0UL);
					if (item != null)
					{
						(item.Drop(this.GetDropPosition() + new Vector3(0f, 0.5f, 0f), this.GetInheritedDropVelocity(), default(Quaternion)) as DroppedItem).DropReason = DroppedItem.DropReasonEnum.Loot;
					}
				}
			}
		}
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000E3225 File Offset: 0x000E1425
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!this.FirstLooted)
		{
			this.FirstLooted = true;
			Analytics.Azure.OnFirstLooted(this, baseEntity);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000E3244 File Offset: 0x000E1444
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (this.destroyOnEmpty && (base.inventory == null || base.inventory.itemList == null || base.inventory.itemList.Count == 0))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x00029C50 File Offset: 0x00027E50
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000E3284 File Offset: 0x000E1484
	public override void OnKilled(HitInfo info)
	{
		Analytics.Azure.OnLootContainerDestroyed(this, info.InitiatorPlayer, info.Weapon);
		base.OnKilled(info);
		if (info != null && info.InitiatorPlayer != null && !string.IsNullOrEmpty(this.deathStat))
		{
			info.InitiatorPlayer.stats.Add(this.deathStat, 1, Stats.Life);
		}
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000E32E0 File Offset: 0x000E14E0
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x0004D3EB File Offset: 0x0004B5EB
	public override void InitShared()
	{
		base.InitShared();
	}

	// Token: 0x04001B74 RID: 7028
	public bool destroyOnEmpty = true;

	// Token: 0x04001B75 RID: 7029
	public LootSpawn lootDefinition;

	// Token: 0x04001B76 RID: 7030
	public int maxDefinitionsToSpawn;

	// Token: 0x04001B77 RID: 7031
	public float minSecondsBetweenRefresh = 3600f;

	// Token: 0x04001B78 RID: 7032
	public float maxSecondsBetweenRefresh = 7200f;

	// Token: 0x04001B79 RID: 7033
	public bool initialLootSpawn = true;

	// Token: 0x04001B7A RID: 7034
	public float xpLootedScale = 1f;

	// Token: 0x04001B7B RID: 7035
	public float xpDestroyedScale = 1f;

	// Token: 0x04001B7C RID: 7036
	public bool BlockPlayerItemInput;

	// Token: 0x04001B7D RID: 7037
	public int scrapAmount;

	// Token: 0x04001B7E RID: 7038
	public string deathStat = "";

	// Token: 0x04001B7F RID: 7039
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x04001B80 RID: 7040
	public LootContainer.spawnType SpawnType;

	// Token: 0x04001B81 RID: 7041
	public bool FirstLooted;

	// Token: 0x04001B82 RID: 7042
	private static ItemDefinition scrapDef;

	// Token: 0x02000CF3 RID: 3315
	public enum spawnType
	{
		// Token: 0x0400460B RID: 17931
		GENERIC,
		// Token: 0x0400460C RID: 17932
		PLAYER,
		// Token: 0x0400460D RID: 17933
		TOWN,
		// Token: 0x0400460E RID: 17934
		AIRDROP,
		// Token: 0x0400460F RID: 17935
		CRASHSITE,
		// Token: 0x04004610 RID: 17936
		ROADSIDE
	}

	// Token: 0x02000CF4 RID: 3316
	[Serializable]
	public struct LootSpawnSlot
	{
		// Token: 0x04004611 RID: 17937
		public LootSpawn definition;

		// Token: 0x04004612 RID: 17938
		public int numberToSpawn;

		// Token: 0x04004613 RID: 17939
		public float probability;
	}
}
