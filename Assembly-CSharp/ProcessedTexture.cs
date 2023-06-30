using System;
using UnityEngine;

// Token: 0x02000933 RID: 2355
public class ProcessedTexture
{
	// Token: 0x0600386F RID: 14447 RVA: 0x001502BC File Offset: 0x0014E4BC
	public void Dispose()
	{
		this.DestroyRenderTexture(ref this.result);
		this.DestroyMaterial(ref this.material);
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x001502D6 File Offset: 0x0014E4D6
	protected RenderTexture CreateRenderTexture(string name, int width, int height, bool linear)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.anisoLevel = 0;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x0015030D File Offset: 0x0014E50D
	protected void DestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(rt);
		rt = null;
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x00150324 File Offset: 0x0014E524
	protected RenderTexture CreateTemporary()
	{
		return RenderTexture.GetTemporary(this.result.width, this.result.height, this.result.depth, this.result.format, this.result.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x00150373 File Offset: 0x0014E573
	protected void ReleaseTemporary(RenderTexture rt)
	{
		RenderTexture.ReleaseTemporary(rt);
	}

	// Token: 0x06003874 RID: 14452 RVA: 0x0015037B File Offset: 0x0014E57B
	protected Material CreateMaterial(string shader)
	{
		return this.CreateMaterial(Shader.Find(shader));
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x00150389 File Offset: 0x0014E589
	protected Material CreateMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x0015030D File Offset: 0x0014E50D
	protected void DestroyMaterial(ref Material mat)
	{
		if (mat == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(mat);
		mat = null;
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x00150399 File Offset: 0x0014E599
	public static implicit operator Texture(ProcessedTexture t)
	{
		return t.result;
	}

	// Token: 0x0400339D RID: 13213
	protected RenderTexture result;

	// Token: 0x0400339E RID: 13214
	protected Material material;
}
