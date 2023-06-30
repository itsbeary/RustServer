using System;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class BlendTexture : ProcessedTexture
{
	// Token: 0x0600385A RID: 14426 RVA: 0x0014F526 File Offset: 0x0014D726
	public BlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/BlitCopyAlpha");
		this.result = base.CreateRenderTexture("Blend Texture", width, height, linear);
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0014F553 File Offset: 0x0014D753
	public void Blend(Texture source, Texture target, float alpha)
	{
		this.material.SetTexture("_BlendTex", target);
		this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
		Graphics.Blit(source, this.result, this.material);
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x0014F58E File Offset: 0x0014D78E
	public void CopyTo(BlendTexture target)
	{
		Graphics.Blit(this.result, target.result);
	}
}
