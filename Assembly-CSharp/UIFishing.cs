using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200080E RID: 2062
public class UIFishing : SingletonComponent<UIFishing>
{
	// Token: 0x060035C0 RID: 13760 RVA: 0x0014551A File Offset: 0x0014371A
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002E7D RID: 11901
	public Slider TensionLine;

	// Token: 0x04002E7E RID: 11902
	public Image FillImage;

	// Token: 0x04002E7F RID: 11903
	public Gradient FillGradient;
}
