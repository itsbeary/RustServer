using System;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
	// Token: 0x06002C77 RID: 11383 RVA: 0x0010D7A4 File Offset: 0x0010B9A4
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.fogMaterial)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.fogMaterial.SetColor("_FogColor", this.fogColor);
		this.fogMaterial.SetFloat("_Start", this.fogStart);
		this.fogMaterial.SetFloat("_Range", this.fogRange);
		this.fogMaterial.SetFloat("_Density", this.fogDensity);
		if (this.fogSky)
		{
			this.fogMaterial.SetFloat("_CutOff", 2f);
		}
		else
		{
			this.fogMaterial.SetFloat("_CutOff", 1f);
		}
		for (int i = 0; i < this.fogMaterial.passCount; i++)
		{
			Graphics.Blit(source, destination, this.fogMaterial, i);
		}
	}

	// Token: 0x04002447 RID: 9287
	public Material fogMaterial;

	// Token: 0x04002448 RID: 9288
	public Color fogColor = Color.white;

	// Token: 0x04002449 RID: 9289
	public float fogStart;

	// Token: 0x0400244A RID: 9290
	public float fogRange = 1f;

	// Token: 0x0400244B RID: 9291
	public float fogDensity = 1f;

	// Token: 0x0400244C RID: 9292
	public bool fogSky;
}
