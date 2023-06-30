using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class GrowableGenes
{
	// Token: 0x060022F9 RID: 8953 RVA: 0x000E0749 File Offset: 0x000DE949
	public GrowableGenes()
	{
		this.Clear();
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000E0758 File Offset: 0x000DE958
	private void Clear()
	{
		this.Genes = new GrowableGene[6];
		for (int i = 0; i < 6; i++)
		{
			this.Genes[i] = new GrowableGene();
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000E078C File Offset: 0x000DE98C
	public void GenerateRandom(GrowableEntity growable)
	{
		if (growable == null)
		{
			return;
		}
		if (growable.Properties.Genes == null)
		{
			return;
		}
		this.CalculateBaseWeights(growable.Properties.Genes);
		for (int i = 0; i < 6; i++)
		{
			this.CalculateSlotWeights(growable.Properties.Genes, i);
			this.Genes[i].Set(this.PickWeightedGeneType(), true);
		}
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000E07FC File Offset: 0x000DE9FC
	private void CalculateBaseWeights(GrowableGeneProperties properties)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.baseWeights[num].GeneType = (GrowableGenes.slotWeights[num].GeneType = (GrowableGenetics.GeneType)num);
			GrowableGenes.baseWeights[num].Weighting = geneWeight.BaseWeight;
			num++;
		}
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000E0868 File Offset: 0x000DEA68
	private void CalculateSlotWeights(GrowableGeneProperties properties, int slot)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.slotWeights[num].Weighting = GrowableGenes.baseWeights[num].Weighting + geneWeight.SlotWeights[slot];
			num++;
		}
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x000E08C4 File Offset: 0x000DEAC4
	private GrowableGenetics.GeneType PickWeightedGeneType()
	{
		IOrderedEnumerable<GrowableGenetics.GeneWeighting> orderedEnumerable = GrowableGenes.slotWeights.OrderBy((GrowableGenetics.GeneWeighting w) => w.Weighting);
		float num = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting in orderedEnumerable)
		{
			num += geneWeighting.Weighting;
		}
		GrowableGenetics.GeneType geneType = GrowableGenetics.GeneType.Empty;
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting2 in orderedEnumerable)
		{
			num3 += geneWeighting2.Weighting;
			if (num2 < num3)
			{
				geneType = geneWeighting2.GeneType;
				break;
			}
		}
		return geneType;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000E09AC File Offset: 0x000DEBAC
	public int GetGeneTypeCount(GrowableGenetics.GeneType geneType)
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].Type == geneType)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000E09E0 File Offset: 0x000DEBE0
	public int GetPositiveGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000E0A14 File Offset: 0x000DEC14
	public int GetNegativeGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (!genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000E0A47 File Offset: 0x000DEC47
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.growableEntity.genes = GrowableGeneEncoding.EncodeGenesToInt(this);
		info.msg.growableEntity.previousGenes = GrowableGeneEncoding.EncodePreviousGenesToInt(this);
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000E0A75 File Offset: 0x000DEC75
	public void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.growableEntity == null)
		{
			return;
		}
		GrowableGeneEncoding.DecodeIntToGenes(info.msg.growableEntity.genes, this);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(info.msg.growableEntity.previousGenes, this);
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000E0AB1 File Offset: 0x000DECB1
	public void DebugPrint()
	{
		Debug.Log(this.GetDisplayString(false));
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000E0AC0 File Offset: 0x000DECC0
	private string GetDisplayString(bool previousGenes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetDisplayCharacter(previousGenes ? this.Genes[i].PreviousType : this.Genes[i].Type));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04001AC5 RID: 6853
	public GrowableGene[] Genes;

	// Token: 0x04001AC6 RID: 6854
	private static GrowableGenetics.GeneWeighting[] baseWeights = new GrowableGenetics.GeneWeighting[6];

	// Token: 0x04001AC7 RID: 6855
	private static GrowableGenetics.GeneWeighting[] slotWeights = new GrowableGenetics.GeneWeighting[6];
}
