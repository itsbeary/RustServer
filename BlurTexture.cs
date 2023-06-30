using System;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public class BlurTexture : ProcessedTexture
{
	// Token: 0x0600385D RID: 14429 RVA: 0x0014F5A1 File Offset: 0x0014D7A1
	public BlurTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/Rust/SeparableBlur");
		this.result = base.CreateRenderTexture("Blur Texture", width, height, linear);
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x0014F5CE File Offset: 0x0014D7CE
	public void Blur(float radius)
	{
		this.Blur(this.result, radius);
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x0014F5E0 File Offset: 0x0014D7E0
	public void Blur(Texture source, float radius)
	{
		RenderTexture renderTexture = base.CreateTemporary();
		this.material.SetVector("offsets", new Vector4(radius / (float)Screen.width, 0f, 0f, 0f));
		Graphics.Blit(source, renderTexture, this.material, 0);
		this.material.SetVector("offsets", new Vector4(0f, radius / (float)Screen.height, 0f, 0f));
		Graphics.Blit(renderTexture, this.result, this.material, 0);
		base.ReleaseTemporary(renderTexture);
	}
}
