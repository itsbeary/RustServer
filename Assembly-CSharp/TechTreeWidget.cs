using System;
using UnityEngine;

// Token: 0x020007E0 RID: 2016
public class TechTreeWidget : BaseMonoBehaviour
{
	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06003554 RID: 13652 RVA: 0x001459EF File Offset: 0x00143BEF
	public RectTransform rectTransform
	{
		get
		{
			return base.GetComponent<RectTransform>();
		}
	}

	// Token: 0x04002D61 RID: 11617
	public int id;
}
