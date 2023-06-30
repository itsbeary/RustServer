using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004BE RID: 1214
[CreateAssetMenu(menuName = "Rust/Vehicles/Train Wagon Loot Data", fileName = "Train Wagon Loot Data")]
public class TrainWagonLootData : ScriptableObject
{
	// Token: 0x060027C8 RID: 10184 RVA: 0x000F8759 File Offset: 0x000F6959
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		TrainWagonLootData.instance = Resources.Load<TrainWagonLootData>("Train Wagon Loot Data");
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000F876C File Offset: 0x000F696C
	public TrainWagonLootData.LootOption GetLootOption(TrainCarUnloadable.WagonType wagonType, out int index)
	{
		if (wagonType == TrainCarUnloadable.WagonType.Lootboxes)
		{
			index = 1000;
			return this.lootWagonContent;
		}
		if (wagonType != TrainCarUnloadable.WagonType.Fuel)
		{
			float num = 0f;
			foreach (TrainWagonLootData.LootOption lootOption in this.oreOptions)
			{
				num += lootOption.spawnWeighting;
			}
			float num2 = UnityEngine.Random.value * num;
			for (index = 0; index < this.oreOptions.Length; index++)
			{
				if ((num2 -= this.oreOptions[index].spawnWeighting) < 0f)
				{
					return this.oreOptions[index];
				}
			}
			return this.oreOptions[index];
		}
		index = 1001;
		return this.fuelWagonContent;
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000F8814 File Offset: 0x000F6A14
	public bool TryGetLootFromIndex(int index, out TrainWagonLootData.LootOption lootOption)
	{
		if (index == 1000)
		{
			lootOption = this.lootWagonContent;
			return true;
		}
		if (index != 1001)
		{
			index = Mathf.Clamp(index, 0, this.oreOptions.Length - 1);
			lootOption = this.oreOptions[index];
			return true;
		}
		lootOption = this.fuelWagonContent;
		return true;
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000F8868 File Offset: 0x000F6A68
	public bool TryGetIndexFromLoot(TrainWagonLootData.LootOption lootOption, out int index)
	{
		if (lootOption == this.lootWagonContent)
		{
			index = 1000;
			return true;
		}
		if (lootOption == this.fuelWagonContent)
		{
			index = 1001;
			return true;
		}
		for (index = 0; index < this.oreOptions.Length; index++)
		{
			if (this.oreOptions[index] == lootOption)
			{
				return true;
			}
		}
		index = -1;
		return false;
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000F88C4 File Offset: 0x000F6AC4
	public static float GetOrePercent(int lootTypeIndex, StorageContainer sc)
	{
		TrainWagonLootData.LootOption lootOption;
		if (TrainWagonLootData.instance.TryGetLootFromIndex(lootTypeIndex, out lootOption))
		{
			return TrainWagonLootData.GetOrePercent(lootOption, sc);
		}
		return 0f;
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000F88F0 File Offset: 0x000F6AF0
	public static float GetOrePercent(TrainWagonLootData.LootOption lootOption, StorageContainer sc)
	{
		float num = 0f;
		if (sc.IsValid())
		{
			int maxLootAmount = lootOption.maxLootAmount;
			if ((float)maxLootAmount == 0f)
			{
				num = 0f;
			}
			else
			{
				num = Mathf.Clamp01((float)sc.inventory.GetAmount(lootOption.lootItem.itemid, false) / (float)maxLootAmount);
			}
		}
		return num;
	}

	// Token: 0x04002069 RID: 8297
	[SerializeField]
	private TrainWagonLootData.LootOption[] oreOptions;

	// Token: 0x0400206A RID: 8298
	[SerializeField]
	[ReadOnly]
	private TrainWagonLootData.LootOption lootWagonContent;

	// Token: 0x0400206B RID: 8299
	[SerializeField]
	private TrainWagonLootData.LootOption fuelWagonContent;

	// Token: 0x0400206C RID: 8300
	public static TrainWagonLootData instance;

	// Token: 0x0400206D RID: 8301
	private const int LOOT_WAGON_INDEX = 1000;

	// Token: 0x0400206E RID: 8302
	private const int FUEL_WAGON_INDEX = 1001;

	// Token: 0x02000D30 RID: 3376
	[Serializable]
	public class LootOption
	{
		// Token: 0x04004704 RID: 18180
		public bool showsFX = true;

		// Token: 0x04004705 RID: 18181
		public ItemDefinition lootItem;

		// Token: 0x04004706 RID: 18182
		[FormerlySerializedAs("lootAmount")]
		public int maxLootAmount;

		// Token: 0x04004707 RID: 18183
		public int minLootAmount;

		// Token: 0x04004708 RID: 18184
		public Material lootMaterial;

		// Token: 0x04004709 RID: 18185
		public float spawnWeighting = 1f;

		// Token: 0x0400470A RID: 18186
		public Color fxTint;

		// Token: 0x0400470B RID: 18187
		[FormerlySerializedAs("indoorFXTint")]
		public Color particleFXTint;
	}
}
