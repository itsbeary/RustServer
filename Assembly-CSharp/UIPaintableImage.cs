using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A3 RID: 2211
public class UIPaintableImage : MonoBehaviour
{
	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x060036E9 RID: 14057 RVA: 0x000B98F4 File Offset: 0x000B7AF4
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x040031C7 RID: 12743
	public RawImage image;

	// Token: 0x040031C8 RID: 12744
	public int texSize = 64;

	// Token: 0x040031C9 RID: 12745
	public Color clearColor = Color.clear;

	// Token: 0x040031CA RID: 12746
	public FilterMode filterMode = FilterMode.Bilinear;

	// Token: 0x040031CB RID: 12747
	public bool mipmaps;

	// Token: 0x02000EB2 RID: 3762
	public enum DrawMode
	{
		// Token: 0x04004CF7 RID: 19703
		AlphaBlended,
		// Token: 0x04004CF8 RID: 19704
		Additive,
		// Token: 0x04004CF9 RID: 19705
		Lighten,
		// Token: 0x04004CFA RID: 19706
		Erase
	}
}
