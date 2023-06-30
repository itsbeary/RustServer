using System;

// Token: 0x020003F6 RID: 1014
public class GrowableGene
{
	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x060022E2 RID: 8930 RVA: 0x000E048D File Offset: 0x000DE68D
	// (set) Token: 0x060022E3 RID: 8931 RVA: 0x000E0495 File Offset: 0x000DE695
	public GrowableGenetics.GeneType Type { get; private set; }

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x060022E4 RID: 8932 RVA: 0x000E049E File Offset: 0x000DE69E
	// (set) Token: 0x060022E5 RID: 8933 RVA: 0x000E04A6 File Offset: 0x000DE6A6
	public GrowableGenetics.GeneType PreviousType { get; private set; }

	// Token: 0x060022E6 RID: 8934 RVA: 0x000E04AF File Offset: 0x000DE6AF
	public void Set(GrowableGenetics.GeneType geneType, bool firstSet = false)
	{
		if (firstSet)
		{
			this.SetPrevious(geneType);
		}
		else
		{
			this.SetPrevious(this.Type);
		}
		this.Type = geneType;
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x000E04D0 File Offset: 0x000DE6D0
	public void SetPrevious(GrowableGenetics.GeneType type)
	{
		this.PreviousType = type;
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000E04D9 File Offset: 0x000DE6D9
	public string GetDisplayCharacter()
	{
		return GrowableGene.GetDisplayCharacter(this.Type);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000E04E8 File Offset: 0x000DE6E8
	public static string GetDisplayCharacter(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return "X";
		case GrowableGenetics.GeneType.WaterRequirement:
			return "W";
		case GrowableGenetics.GeneType.GrowthSpeed:
			return "G";
		case GrowableGenetics.GeneType.Yield:
			return "Y";
		case GrowableGenetics.GeneType.Hardiness:
			return "H";
		default:
			return "U";
		}
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000E0534 File Offset: 0x000DE734
	public string GetColourCodedDisplayCharacter()
	{
		return GrowableGene.GetColourCodedDisplayCharacter(this.Type);
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000E0541 File Offset: 0x000DE741
	public static string GetColourCodedDisplayCharacter(GrowableGenetics.GeneType type)
	{
		return "<color=" + (GrowableGene.IsPositive(type) ? "#60891B>" : "#AA4734>") + GrowableGene.GetDisplayCharacter(type) + "</color>";
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000E056C File Offset: 0x000DE76C
	public static bool IsPositive(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return false;
		case GrowableGenetics.GeneType.WaterRequirement:
			return false;
		case GrowableGenetics.GeneType.GrowthSpeed:
			return true;
		case GrowableGenetics.GeneType.Yield:
			return true;
		case GrowableGenetics.GeneType.Hardiness:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000E0595 File Offset: 0x000DE795
	public bool IsPositive()
	{
		return GrowableGene.IsPositive(this.Type);
	}
}
