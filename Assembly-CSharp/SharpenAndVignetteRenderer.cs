using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200064D RID: 1613
public class SharpenAndVignetteRenderer : PostProcessEffectRenderer<SharpenAndVignette>
{
	// Token: 0x06002ED6 RID: 11990 RVA: 0x00119AEB File Offset: 0x00117CEB
	public override void Init()
	{
		base.Init();
		this.sharpenAndVigenetteShader = Shader.Find("Hidden/PostProcessing/SharpenAndVignette");
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x00119B04 File Offset: 0x00117D04
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("SharpenAndVignette");
		PropertySheet propertySheet = context.propertySheets.Get(this.sharpenAndVigenetteShader);
		propertySheet.properties.Clear();
		bool value = base.settings.applySharpen.value;
		bool value2 = base.settings.applyVignette.value;
		if (value)
		{
			propertySheet.properties.SetFloat("_px", 1f / (float)Screen.width);
			propertySheet.properties.SetFloat("_py", 1f / (float)Screen.height);
			propertySheet.properties.SetFloat("_strength", base.settings.strength.value);
			propertySheet.properties.SetFloat("_clamp", base.settings.clamp.value);
		}
		if (value2)
		{
			propertySheet.properties.SetFloat("_sharpness", base.settings.sharpness.value * 0.01f);
			propertySheet.properties.SetFloat("_darkness", base.settings.darkness.value * 0.02f);
		}
		if (value && !value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		else if (value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1, false, null);
		}
		else if (!value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 2, false, null);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		command.EndSample("SharpenAndVignette");
	}

	// Token: 0x040026B2 RID: 9906
	private Shader sharpenAndVigenetteShader;
}
