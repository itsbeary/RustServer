using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000635 RID: 1589
public class DepthOfFieldEffectRenderer : PostProcessEffectRenderer<DepthOfFieldEffect>
{
	// Token: 0x06002EA8 RID: 11944 RVA: 0x001180F3 File Offset: 0x001162F3
	public override void Init()
	{
		this.dofShader = Shader.Find("Hidden/PostProcessing/DepthOfFieldEffect");
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x00118108 File Offset: 0x00116308
	private float FocalDistance01(Camera cam, float worldDist)
	{
		return cam.WorldToViewportPoint((worldDist - cam.nearClipPlane) * cam.transform.forward + cam.transform.position).z / (cam.farClipPlane - cam.nearClipPlane);
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x00118158 File Offset: 0x00116358
	private void WriteCoc(PostProcessRenderContext context, PropertySheet sheet)
	{
		CommandBuffer command = context.command;
		RenderTargetIdentifier source = context.source;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		float num = 1f;
		int num2 = context.width / 2;
		int num3 = context.height / 2;
		int num4 = Shader.PropertyToID("DOFtemp1");
		int num5 = Shader.PropertyToID("DOFtemp2");
		command.GetTemporaryRT(num5, num2, num3, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(source, num5, sheet, 1, false, null);
		float num6 = this.internalBlurWidth * num;
		sheet.properties.SetVector("_Offsets", new Vector4(0f, num6, 0f, num6));
		command.GetTemporaryRT(num4, num2, num3, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(num5, num4, sheet, 0, false, null);
		command.ReleaseTemporaryRT(num5);
		sheet.properties.SetVector("_Offsets", new Vector4(num6, 0f, 0f, num6));
		command.GetTemporaryRT(num5, num2, num3, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(num4, num5, sheet, 0, false, null);
		command.ReleaseTemporaryRT(num4);
		command.SetGlobalTexture("_FgOverlap", num5);
		command.BlitFullscreenTriangle(source, source, sheet, 3, RenderBufferLoadAction.Load, null);
		command.ReleaseTemporaryRT(num5);
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x001182C0 File Offset: 0x001164C0
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(this.dofShader);
		CommandBuffer command = context.command;
		int width = context.width;
		int height = context.height;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		bool value = base.settings.highResolution.value;
		DOFBlurSampleCountParameter blurSampleCount = base.settings.blurSampleCount;
		float num = base.settings.focalSize.value;
		float value2 = base.settings.focalLength.value;
		float num2 = base.settings.aperture.value;
		float num3 = base.settings.maxBlurSize.value;
		int num4 = Shader.PropertyToID("DOFrtLow");
		int num5 = Shader.PropertyToID("DOFrtLow2");
		num2 = Math.Max(num2, 0f);
		num3 = Math.Max(num3, 0.1f);
		num = Mathf.Clamp(num, 0f, 2f);
		this.internalBlurWidth = Mathf.Max(num3, 0f);
		this.focalDistance01 = this.FocalDistance01(context.camera, value2);
		propertySheet.properties.SetVector("_CurveParams", new Vector4(1f, num, num2 / 10f, this.focalDistance01));
		if (value)
		{
			this.internalBlurWidth *= 2f;
		}
		this.WriteCoc(context, propertySheet);
		if (ConVar.Graphics.dof_debug)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5, false, null);
			return;
		}
		command.GetTemporaryRT(num4, width >> 1, height >> 1, 0, FilterMode.Bilinear, sourceFormat);
		command.GetTemporaryRT(num5, width >> 1, height >> 1, 0, FilterMode.Bilinear, sourceFormat);
		int num6 = 2;
		propertySheet.properties.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0.025f, this.internalBlurWidth));
		propertySheet.properties.SetInt("_BlurCountMode", (int)blurSampleCount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, num6, false, null);
		command.ReleaseTemporaryRT(num4);
		command.ReleaseTemporaryRT(num5);
	}

	// Token: 0x04002662 RID: 9826
	private float focalDistance01 = 10f;

	// Token: 0x04002663 RID: 9827
	private float internalBlurWidth = 1f;

	// Token: 0x04002664 RID: 9828
	private Shader dofShader;
}
