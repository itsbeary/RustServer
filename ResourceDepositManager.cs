using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public class ResourceDepositManager : BaseEntity
{
	// Token: 0x06002349 RID: 9033 RVA: 0x000E1CE0 File Offset: 0x000DFEE0
	public static Vector2i GetIndexFrom(Vector3 pos)
	{
		return new Vector2i((int)pos.x / 20, (int)pos.z / 20);
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000E1CFB File Offset: 0x000DFEFB
	public static ResourceDepositManager Get()
	{
		return ResourceDepositManager._manager;
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000E1D02 File Offset: 0x000DFF02
	public ResourceDepositManager()
	{
		ResourceDepositManager._manager = this;
		this._deposits = new Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit>();
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000E1D1C File Offset: 0x000DFF1C
	public ResourceDepositManager.ResourceDeposit CreateFromPosition(Vector3 pos)
	{
		Vector2i indexFrom = ResourceDepositManager.GetIndexFrom(pos);
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)new Vector2((float)indexFrom.x, (float)indexFrom.y).Seed(World.Seed + World.Salt));
		ResourceDepositManager.ResourceDeposit resourceDeposit = new ResourceDepositManager.ResourceDeposit();
		resourceDeposit.origin = new Vector3((float)(indexFrom.x * 20), 0f, (float)(indexFrom.y * 20));
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (!false)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, UnityEngine.Random.Range(30000, 100000), UnityEngine.Random.Range(0.3f, 0.5f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			float num;
			if (World.Procedural)
			{
				num = ((TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5f) ? 1f : 0f) * 0.25f;
			}
			else
			{
				num = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(2f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num2;
			if (World.Procedural)
			{
				num2 = ((TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5f) ? 1f : 0f) * (0.25f + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1f : 0f) + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1f : 0f));
			}
			else
			{
				num2 = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num2)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(4f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num3 = 0f;
			if (World.Procedural)
			{
				if (TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5f || TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5f)
				{
					num3 += 0.25f;
				}
			}
			else
			{
				num3 += 0.15f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num3)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, UnityEngine.Random.Range(5000, 10000), UnityEngine.Random.Range(30f, 50f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
		}
		this._deposits.Add(indexFrom, resourceDeposit);
		UnityEngine.Random.state = state;
		return resourceDeposit;
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000E1FF8 File Offset: 0x000E01F8
	public ResourceDepositManager.ResourceDeposit GetFromPosition(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit resourceDeposit = null;
		if (this._deposits.TryGetValue(ResourceDepositManager.GetIndexFrom(pos), out resourceDeposit))
		{
			return resourceDeposit;
		}
		return null;
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000E2020 File Offset: 0x000E0220
	public static ResourceDepositManager.ResourceDeposit GetOrCreate(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit fromPosition = ResourceDepositManager.Get().GetFromPosition(pos);
		if (fromPosition != null)
		{
			return fromPosition;
		}
		return ResourceDepositManager.Get().CreateFromPosition(pos);
	}

	// Token: 0x04001B2B RID: 6955
	public static ResourceDepositManager _manager;

	// Token: 0x04001B2C RID: 6956
	private const int resolution = 20;

	// Token: 0x04001B2D RID: 6957
	public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

	// Token: 0x02000CF0 RID: 3312
	[Serializable]
	public class ResourceDeposit
	{
		// Token: 0x0600501A RID: 20506 RVA: 0x001A81D4 File Offset: 0x001A63D4
		public ResourceDeposit()
		{
			this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x001A81F4 File Offset: 0x001A63F4
		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry();
			resourceDepositEntry.type = type;
			resourceDepositEntry.efficiency = efficiency;
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry2 = resourceDepositEntry;
			resourceDepositEntry.amount = amount;
			resourceDepositEntry2.startAmount = amount;
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			this._resources.Add(resourceDepositEntry);
		}

		// Token: 0x040045F1 RID: 17905
		public float lastSurveyTime = float.NegativeInfinity;

		// Token: 0x040045F2 RID: 17906
		public Vector3 origin;

		// Token: 0x040045F3 RID: 17907
		public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

		// Token: 0x02000FDE RID: 4062
		[Serializable]
		public enum surveySpawnType
		{
			// Token: 0x0400517F RID: 20863
			ITEM,
			// Token: 0x04005180 RID: 20864
			OIL,
			// Token: 0x04005181 RID: 20865
			WATER
		}

		// Token: 0x02000FDF RID: 4063
		[Serializable]
		public class ResourceDepositEntry
		{
			// Token: 0x060055DA RID: 21978 RVA: 0x001BB1D6 File Offset: 0x001B93D6
			public void Subtract(int subamount)
			{
				if (subamount <= 0)
				{
					return;
				}
				this.amount -= subamount;
				if (this.amount < 0)
				{
					this.amount = 0;
				}
			}

			// Token: 0x04005182 RID: 20866
			public ItemDefinition type;

			// Token: 0x04005183 RID: 20867
			public float efficiency = 1f;

			// Token: 0x04005184 RID: 20868
			public int amount;

			// Token: 0x04005185 RID: 20869
			public int startAmount;

			// Token: 0x04005186 RID: 20870
			public float workNeeded = 1f;

			// Token: 0x04005187 RID: 20871
			public float workDone;

			// Token: 0x04005188 RID: 20872
			public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;

			// Token: 0x04005189 RID: 20873
			public bool isLiquid;
		}
	}
}
