using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D2 RID: 2258
public class UIGenesDisplay : MonoBehaviour
{
	// Token: 0x06003762 RID: 14178 RVA: 0x0014C600 File Offset: 0x0014A800
	public void Init(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			this.GeneUI[num].Init(growableGene);
			num++;
			if (num < genes.Genes.Length)
			{
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x0014C683 File Offset: 0x0014A883
	public void InitDualRow(GrowableGenes genes, bool firstRow)
	{
		if (firstRow)
		{
			this.InitFirstRow(genes);
			return;
		}
		this.InitSecondRow(genes);
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0014C698 File Offset: 0x0014A898
	private void InitFirstRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].InitPrevious(growableGene);
			}
			else
			{
				this.GeneUI[num].Init(growableGene);
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			if (growableGene.Type != growableGene.PreviousType || genes.Genes[num].Type != genes.Genes[num].PreviousType)
			{
				this.TextLinks[num - 1].enabled = false;
			}
			else
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0014C78C File Offset: 0x0014A98C
	private void InitSecondRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].Init(growableGene);
			}
			else
			{
				this.GeneUI[num].Hide();
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			this.TextLinks[num - 1].enabled = false;
			GrowableGene growableGene2 = genes.Genes[num];
			this.TextDiagLinks[num - 1].enabled = false;
			if (growableGene.Type != growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (growableGene2.IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
			else if (growableGene.Type == growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, -43f, growableGene2);
			}
			else if (growableGene.Type != growableGene.PreviousType && growableGene2.Type == growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, 43f, growableGene2);
			}
		}
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x0014C8E8 File Offset: 0x0014AAE8
	private void ShowDiagLink(int index, float rotation, GrowableGene nextGene)
	{
		Vector3 localEulerAngles = this.TextDiagLinks[index].transform.localEulerAngles;
		localEulerAngles.z = rotation;
		this.TextDiagLinks[index].transform.localEulerAngles = localEulerAngles;
		this.TextDiagLinks[index].enabled = true;
		this.TextDiagLinks[index].color = (nextGene.IsPositive() ? this.GeneUI[index].PositiveColour : this.GeneUI[index].NegativeColour);
	}

	// Token: 0x040032D7 RID: 13015
	public UIGene[] GeneUI;

	// Token: 0x040032D8 RID: 13016
	public Text[] TextLinks;

	// Token: 0x040032D9 RID: 13017
	public Text[] TextDiagLinks;
}
