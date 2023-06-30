using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B8 RID: 2232
public class RepairCostIndicatorRow : MonoBehaviour
{
	// Token: 0x04003248 RID: 12872
	public RustText ItemName;

	// Token: 0x04003249 RID: 12873
	public Image ItemSprite;

	// Token: 0x0400324A RID: 12874
	public RustText Amount;

	// Token: 0x0400324B RID: 12875
	public RectTransform FillRect;

	// Token: 0x0400324C RID: 12876
	public Image BackgroundImage;

	// Token: 0x0400324D RID: 12877
	public Color OkColour;

	// Token: 0x0400324E RID: 12878
	public Color MissingColour;
}
