using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000645 RID: 1605
public class LensDirtinessRenderer : PostProcessEffectRenderer<LensDirtinessEffect>
{
	// Token: 0x06002EC9 RID: 11977 RVA: 0x0011929A File Offset: 0x0011749A
	public override void Init()
	{
		base.Init();
		this.lensDirtinessShader = Shader.Find("Hidden/PostProcessing/LensDirtiness");
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x001192B4 File Offset: 0x001174B4
	public override void Render(PostProcessRenderContext context)
	{
		float value = base.settings.bloomSize.value;
		float value2 = base.settings.gain.value;
		float value3 = base.settings.threshold.value;
		float value4 = base.settings.dirtiness.value;
		Color value5 = base.settings.bloomColor.value;
		Texture value6 = base.settings.dirtinessTexture.value;
		bool value7 = base.settings.sceneTintsBloom.value;
		CommandBuffer command = context.command;
		command.BeginSample("LensDirtinessEffect");
		if (value7)
		{
			command.EnableShaderKeyword("_SCENE_TINTS_BLOOM");
		}
		PropertySheet propertySheet = context.propertySheets.Get(this.lensDirtinessShader);
		RenderTargetIdentifier source = context.source;
		RenderTargetIdentifier destination = context.destination;
		int width = context.width;
		int height = context.height;
		int num = Shader.PropertyToID("_RTT_BloomThreshold");
		int num2 = Shader.PropertyToID("_RTT_1");
		int num3 = Shader.PropertyToID("_RTT_2");
		int num4 = Shader.PropertyToID("_RTT_3");
		int num5 = Shader.PropertyToID("_RTT_4");
		int num6 = Shader.PropertyToID("_RTT_Bloom_1");
		int num7 = Shader.PropertyToID("_RTT_Bloom_2");
		propertySheet.properties.SetFloat("_Gain", value2);
		propertySheet.properties.SetFloat("_Threshold", value3);
		command.GetTemporaryRT(num, width / 2, height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(source, num, propertySheet, 0, false, null);
		propertySheet.properties.SetVector("_Offset", new Vector4(1f / (float)width, 1f / (float)height, 0f, 0f) * 2f);
		command.GetTemporaryRT(num2, width / 2, height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(num, num2, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(num);
		command.GetTemporaryRT(num3, width / 4, height / 4, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(num2, num3, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(num2);
		command.GetTemporaryRT(num4, width / 8, height / 8, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(num3, num4, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(num3);
		command.GetTemporaryRT(num5, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(num4, num5, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(num4);
		command.GetTemporaryRT(num6, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.GetTemporaryRT(num7, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(num5, num6, false, null);
		command.ReleaseTemporaryRT(num5);
		for (int i = 1; i <= 8; i++)
		{
			float num8 = value * (float)i / (float)width;
			float num9 = value * (float)i / (float)height;
			propertySheet.properties.SetVector("_Offset", new Vector4(num8, num9, 0f, 0f));
			command.BlitFullscreenTriangle(num6, num7, propertySheet, 1, false, null);
			command.BlitFullscreenTriangle(num7, num6, propertySheet, 1, false, null);
		}
		command.SetGlobalTexture("_Bloom", num7);
		propertySheet.properties.SetFloat("_Dirtiness", value4);
		propertySheet.properties.SetColor("_BloomColor", value5);
		propertySheet.properties.SetTexture("_DirtinessTexture", value6);
		command.BlitFullscreenTriangle(source, destination, propertySheet, 2, false, null);
		command.ReleaseTemporaryRT(num6);
		command.ReleaseTemporaryRT(num7);
		command.EndSample("LensDirtinessEffect");
	}

	// Token: 0x04002699 RID: 9881
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x0400269A RID: 9882
	private Shader lensDirtinessShader;

	// Token: 0x02000DB4 RID: 3508
	private enum Pass
	{
		// Token: 0x0400490B RID: 18699
		Threshold,
		// Token: 0x0400490C RID: 18700
		Kawase,
		// Token: 0x0400490D RID: 18701
		Compose
	}
}
