using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007A6 RID: 1958
public class Crosshair : BaseMonoBehaviour
{
	// Token: 0x04002BB0 RID: 11184
	public static bool Enabled = true;

	// Token: 0x04002BB1 RID: 11185
	public Image Image;

	// Token: 0x04002BB2 RID: 11186
	public RectTransform reticleTransform;

	// Token: 0x04002BB3 RID: 11187
	public CanvasGroup reticleAlpha;

	// Token: 0x04002BB4 RID: 11188
	public RectTransform hitNotifyMarker;

	// Token: 0x04002BB5 RID: 11189
	public CanvasGroup hitNotifyAlpha;

	// Token: 0x04002BB6 RID: 11190
	public static Crosshair instance;

	// Token: 0x04002BB7 RID: 11191
	public static float lastHitTime = 0f;

	// Token: 0x04002BB8 RID: 11192
	public float crosshairAlpha = 0.75f;
}
