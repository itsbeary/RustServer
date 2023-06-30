using System;
using UnityEngine;

// Token: 0x02000990 RID: 2448
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
	// Token: 0x06003A1F RID: 14879 RVA: 0x001572E3 File Offset: 0x001554E3
	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x0015730B File Offset: 0x0015550B
	private void Start()
	{
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x0015731B File Offset: 0x0015551B
	public bool IsActive()
	{
		return base.enabled;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x00157324 File Offset: 0x00155524
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}

	// Token: 0x040034D1 RID: 13521
	public Shader shader;

	// Token: 0x040034D2 RID: 13522
	private Material mat;
}
