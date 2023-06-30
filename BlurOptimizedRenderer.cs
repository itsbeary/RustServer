using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000630 RID: 1584
public class BlurOptimizedRenderer : PostProcessEffectRenderer<BlurOptimized>
{
	// Token: 0x06002EA2 RID: 11938 RVA: 0x00117DA1 File Offset: 0x00115FA1
	public override void Init()
	{
		base.Init();
		this.blurShader = Shader.Find("Hidden/PostProcessing/BlurOptimized");
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x00117DBC File Offset: 0x00115FBC
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("BlurOptimized");
		int value = base.settings.downsample.value;
		float value2 = base.settings.fadeToBlurDistance.value;
		float value3 = base.settings.blurSize.value;
		int value4 = base.settings.blurIterations.value;
		bool value5 = base.settings.blurType.value != BlurType.StandardGauss;
		float num = 1f / (1f * (float)(1 << value));
		float num2 = 1f / Mathf.Clamp(value2, 0.001f, 10000f);
		PropertySheet propertySheet = context.propertySheets.Get(this.blurShader);
		propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num, -value3 * num, num2, 0f));
		int num3 = context.width >> value;
		int num4 = context.height >> value;
		int num5 = Shader.PropertyToID("_BlurRT1");
		int num6 = Shader.PropertyToID("_BlurRT2");
		command.GetTemporaryRT(num5, num3, num4, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Default);
		command.BlitFullscreenTriangle(context.source, num5, propertySheet, 0, false, null);
		int num7 = ((!value5) ? 0 : 2);
		for (int i = 0; i < value4; i++)
		{
			float num8 = (float)i * 1f;
			propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num + num8, -value3 * num - num8, num2, 0f));
			command.GetTemporaryRT(num6, num3, num4, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(num5, num6, propertySheet, 1 + num7, false, null);
			command.ReleaseTemporaryRT(num5);
			command.GetTemporaryRT(num5, num3, num4, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(num6, num5, propertySheet, 2 + num7, false, null);
			command.ReleaseTemporaryRT(num6);
		}
		if (value2 <= 0f)
		{
			command.BlitFullscreenTriangle(num5, context.destination, false, null);
		}
		else
		{
			command.SetGlobalTexture("_Source", context.source);
			command.BlitFullscreenTriangle(num5, context.destination, propertySheet, 5, false, null);
		}
		command.ReleaseTemporaryRT(num5);
		command.EndSample("BlurOptimized");
	}

	// Token: 0x04002653 RID: 9811
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x04002654 RID: 9812
	private Shader blurShader;
}
