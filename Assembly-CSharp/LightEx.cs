using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class LightEx : UpdateBehaviour, IClientComponent
{
	// Token: 0x06001DBE RID: 7614 RVA: 0x000CC4D6 File Offset: 0x000CA6D6
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x00007A44 File Offset: 0x00005C44
	public static bool CheckConflict(GameObject go)
	{
		return false;
	}

	// Token: 0x04001689 RID: 5769
	public bool alterColor;

	// Token: 0x0400168A RID: 5770
	public float colorTimeScale = 1f;

	// Token: 0x0400168B RID: 5771
	public Color colorA = Color.red;

	// Token: 0x0400168C RID: 5772
	public Color colorB = Color.yellow;

	// Token: 0x0400168D RID: 5773
	public AnimationCurve blendCurve = new AnimationCurve();

	// Token: 0x0400168E RID: 5774
	public bool loopColor = true;

	// Token: 0x0400168F RID: 5775
	public bool alterIntensity;

	// Token: 0x04001690 RID: 5776
	public float intensityTimeScale = 1f;

	// Token: 0x04001691 RID: 5777
	public AnimationCurve intenseCurve = new AnimationCurve();

	// Token: 0x04001692 RID: 5778
	public float intensityCurveScale = 3f;

	// Token: 0x04001693 RID: 5779
	public bool loopIntensity = true;

	// Token: 0x04001694 RID: 5780
	public bool randomOffset;

	// Token: 0x04001695 RID: 5781
	public float randomIntensityStartScale = -1f;

	// Token: 0x04001696 RID: 5782
	public List<Light> syncLights = new List<Light>(0);
}
