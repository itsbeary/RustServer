using System;
using UnityEngine;

// Token: 0x02000730 RID: 1840
public class LightCloneShadow : MonoBehaviour
{
	// Token: 0x04002A0F RID: 10767
	public bool cloneShadowMap;

	// Token: 0x04002A10 RID: 10768
	public string shaderPropNameMap = "_MainLightShadowMap";

	// Token: 0x04002A11 RID: 10769
	[Range(0f, 2f)]
	public int cloneShadowMapDownscale = 1;

	// Token: 0x04002A12 RID: 10770
	public RenderTexture map;

	// Token: 0x04002A13 RID: 10771
	public bool cloneShadowMask = true;

	// Token: 0x04002A14 RID: 10772
	public string shaderPropNameMask = "_MainLightShadowMask";

	// Token: 0x04002A15 RID: 10773
	[Range(0f, 2f)]
	public int cloneShadowMaskDownscale = 1;

	// Token: 0x04002A16 RID: 10774
	public RenderTexture mask;
}
