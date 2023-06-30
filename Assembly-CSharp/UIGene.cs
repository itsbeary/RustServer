using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D1 RID: 2257
public class UIGene : MonoBehaviour
{
	// Token: 0x0600375D RID: 14173 RVA: 0x0014C538 File Offset: 0x0014A738
	public void Init(GrowableGene gene)
	{
		bool flag = gene.IsPositive();
		this.ImageBG.color = (flag ? this.PositiveColour : this.NegativeColour);
		this.TextGene.color = (flag ? this.PositiveTextColour : this.NegativeTextColour);
		this.TextGene.text = gene.GetDisplayCharacter();
		this.Show();
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0014C59B File Offset: 0x0014A79B
	public void InitPrevious(GrowableGene gene)
	{
		this.ImageBG.color = Color.black;
		this.TextGene.color = Color.grey;
		this.TextGene.text = GrowableGene.GetDisplayCharacter(gene.PreviousType);
		this.Show();
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0014C5D9 File Offset: 0x0014A7D9
	public void Hide()
	{
		this.Child.gameObject.SetActive(false);
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0014C5EC File Offset: 0x0014A7EC
	public void Show()
	{
		this.Child.gameObject.SetActive(true);
	}

	// Token: 0x040032D0 RID: 13008
	public GameObject Child;

	// Token: 0x040032D1 RID: 13009
	public Color PositiveColour;

	// Token: 0x040032D2 RID: 13010
	public Color NegativeColour;

	// Token: 0x040032D3 RID: 13011
	public Color PositiveTextColour;

	// Token: 0x040032D4 RID: 13012
	public Color NegativeTextColour;

	// Token: 0x040032D5 RID: 13013
	public Image ImageBG;

	// Token: 0x040032D6 RID: 13014
	public Text TextGene;
}
