using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public static class GrowableGenetics
{
	// Token: 0x06002307 RID: 8967 RVA: 0x000E0B28 File Offset: 0x000DED28
	public static void CrossBreed(GrowableEntity growable)
	{
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities<GrowableEntity>(growable.transform.position, 1.5f, list, 524288, QueryTriggerInteraction.Collide);
		bool flag = false;
		for (int i = 0; i < 6; i++)
		{
			GrowableGene growableGene = growable.Genes.Genes[i];
			GrowableGenetics.GeneWeighting dominantGeneWeighting = GrowableGenetics.GetDominantGeneWeighting(growable, list, i);
			if (dominantGeneWeighting.Weighting > growable.Properties.Genes.Weights[(int)growableGene.Type].CrossBreedingWeight)
			{
				flag = true;
				growableGene.Set(dominantGeneWeighting.GeneType, false);
			}
		}
		if (flag)
		{
			growable.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000E0BC0 File Offset: 0x000DEDC0
	private static GrowableGenetics.GeneWeighting GetDominantGeneWeighting(GrowableEntity crossBreedingGrowable, List<GrowableEntity> neighbours, int slot)
	{
		PlanterBox planter = crossBreedingGrowable.GetPlanter();
		if (planter == null)
		{
			GrowableGenetics.dominant.Weighting = -1f;
			return GrowableGenetics.dominant;
		}
		for (int i = 0; i < GrowableGenetics.neighbourWeights.Length; i++)
		{
			GrowableGenetics.neighbourWeights[i].Weighting = 0f;
			GrowableGenetics.neighbourWeights[i].GeneType = (GrowableGenetics.GeneType)i;
		}
		GrowableGenetics.dominant.Weighting = 0f;
		foreach (GrowableEntity growableEntity in neighbours)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter2 = growableEntity.GetPlanter();
				if (!(planter2 == null) && !(planter2 != planter) && !(growableEntity == crossBreedingGrowable) && growableEntity.prefabID == crossBreedingGrowable.prefabID && !growableEntity.IsDead())
				{
					GrowableGenetics.GeneType type = growableEntity.Genes.Genes[slot].Type;
					float crossBreedingWeight = growableEntity.Properties.Genes.Weights[(int)type].CrossBreedingWeight;
					GrowableGenetics.GeneWeighting[] array = GrowableGenetics.neighbourWeights;
					GrowableGenetics.GeneType geneType = type;
					float num = (array[(int)geneType].Weighting = array[(int)geneType].Weighting + crossBreedingWeight);
					if (num > GrowableGenetics.dominant.Weighting)
					{
						GrowableGenetics.dominant.Weighting = num;
						GrowableGenetics.dominant.GeneType = type;
					}
				}
			}
		}
		return GrowableGenetics.dominant;
	}

	// Token: 0x04001AC8 RID: 6856
	public const int GeneSlotCount = 6;

	// Token: 0x04001AC9 RID: 6857
	public const float CrossBreedingRadius = 1.5f;

	// Token: 0x04001ACA RID: 6858
	private static GrowableGenetics.GeneWeighting[] neighbourWeights = new GrowableGenetics.GeneWeighting[Enum.GetValues(typeof(GrowableGenetics.GeneType)).Length];

	// Token: 0x04001ACB RID: 6859
	private static GrowableGenetics.GeneWeighting dominant = default(GrowableGenetics.GeneWeighting);

	// Token: 0x02000CE2 RID: 3298
	public enum GeneType
	{
		// Token: 0x040045B4 RID: 17844
		Empty,
		// Token: 0x040045B5 RID: 17845
		WaterRequirement,
		// Token: 0x040045B6 RID: 17846
		GrowthSpeed,
		// Token: 0x040045B7 RID: 17847
		Yield,
		// Token: 0x040045B8 RID: 17848
		Hardiness
	}

	// Token: 0x02000CE3 RID: 3299
	public struct GeneWeighting
	{
		// Token: 0x040045B9 RID: 17849
		public float Weighting;

		// Token: 0x040045BA RID: 17850
		public GrowableGenetics.GeneType GeneType;
	}
}
