using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063B RID: 1595
public class FrostRenderer : PostProcessEffectRenderer<Frost>
{
	// Token: 0x06002EB9 RID: 11961 RVA: 0x0011889B File Offset: 0x00116A9B
	public override void Init()
	{
		base.Init();
		this.frostShader = Shader.Find("Hidden/PostProcessing/Frost");
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x001188B4 File Offset: 0x00116AB4
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Frost");
		PropertySheet propertySheet = context.propertySheets.Get(this.frostShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		propertySheet.properties.SetFloat(this.sharpnessProperty, base.settings.sharpness.value * 0.01f);
		propertySheet.properties.SetFloat(this.darknessProperty, base.settings.darkness.value * 0.02f);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, base.settings.enableVignette.value ? 1 : 0, false, null);
		command.EndSample("Frost");
	}

	// Token: 0x04002673 RID: 9843
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x04002674 RID: 9844
	private int sharpnessProperty = Shader.PropertyToID("_sharpness");

	// Token: 0x04002675 RID: 9845
	private int darknessProperty = Shader.PropertyToID("_darkness");

	// Token: 0x04002676 RID: 9846
	private Shader frostShader;
}
