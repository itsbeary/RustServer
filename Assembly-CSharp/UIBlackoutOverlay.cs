using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008CB RID: 2251
public class UIBlackoutOverlay : MonoBehaviour
{
	// Token: 0x040032AE RID: 12974
	public CanvasGroup group;

	// Token: 0x040032AF RID: 12975
	public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;

	// Token: 0x040032B0 RID: 12976
	public UIBlackoutOverlay.blackoutType overlayType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x040032B1 RID: 12977
	public bool overrideCanvasScaling;

	// Token: 0x040032B2 RID: 12978
	public float referenceScale = 1f;

	// Token: 0x02000EBB RID: 3771
	public enum blackoutType
	{
		// Token: 0x04004D1A RID: 19738
		FULLBLACK,
		// Token: 0x04004D1B RID: 19739
		BINOCULAR,
		// Token: 0x04004D1C RID: 19740
		SCOPE,
		// Token: 0x04004D1D RID: 19741
		HELMETSLIT,
		// Token: 0x04004D1E RID: 19742
		SNORKELGOGGLE,
		// Token: 0x04004D1F RID: 19743
		NVG,
		// Token: 0x04004D20 RID: 19744
		FULLWHITE,
		// Token: 0x04004D21 RID: 19745
		SUNGLASSES,
		// Token: 0x04004D22 RID: 19746
		NONE = 64
	}
}
