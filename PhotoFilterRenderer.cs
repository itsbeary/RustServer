using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000647 RID: 1607
public class PhotoFilterRenderer : PostProcessEffectRenderer<PhotoFilter>
{
	// Token: 0x06002ECD RID: 11981 RVA: 0x00119752 File Offset: 0x00117952
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/PhotoFilter");
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x0011976C File Offset: 0x0011796C
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("PhotoFilter");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetColor(this.rgbProperty, base.settings.color.value);
		propertySheet.properties.SetFloat(this.densityProperty, base.settings.density.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("PhotoFilter");
	}

	// Token: 0x0400269D RID: 9885
	private int rgbProperty = Shader.PropertyToID("_rgb");

	// Token: 0x0400269E RID: 9886
	private int densityProperty = Shader.PropertyToID("_density");

	// Token: 0x0400269F RID: 9887
	private Shader greyScaleShader;
}
