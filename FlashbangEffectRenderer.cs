using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000639 RID: 1593
public class FlashbangEffectRenderer : PostProcessEffectRenderer<FlashbangEffect>
{
	// Token: 0x06002EB2 RID: 11954 RVA: 0x00118646 File Offset: 0x00116846
	public override void Init()
	{
		base.Init();
		this.flashbangEffectShader = Shader.Find("Hidden/PostProcessing/FlashbangEffect");
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x00118660 File Offset: 0x00116860
	public override void Render(PostProcessRenderContext context)
	{
		if (!Application.isPlaying)
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, false, null);
			return;
		}
		CommandBuffer command = context.command;
		FlashbangEffectRenderer.CheckCreateRenderTexture(ref this.screenRT, "Flashbang", context.width, context.height, context.sourceFormat);
		command.BeginSample("FlashbangEffect");
		if (FlashbangEffectRenderer.needsCapture)
		{
			command.CopyTexture(context.source, this.screenRT);
			FlashbangEffectRenderer.needsCapture = false;
		}
		PropertySheet propertySheet = context.propertySheets.Get(this.flashbangEffectShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat("_BurnIntensity", base.settings.burnIntensity.value);
		propertySheet.properties.SetFloat("_WhiteoutIntensity", base.settings.whiteoutIntensity.value);
		if (this.screenRT)
		{
			propertySheet.properties.SetTexture("_BurnOverlay", this.screenRT);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("FlashbangEffect");
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x0011879C File Offset: 0x0011699C
	public override void Release()
	{
		base.Release();
		FlashbangEffectRenderer.SafeDestroyRenderTexture(ref this.screenRT);
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x001187B0 File Offset: 0x001169B0
	private static void CheckCreateRenderTexture(ref RenderTexture rt, string name, int width, int height, RenderTextureFormat format)
	{
		if (rt == null || rt.width != width || rt.height != height)
		{
			FlashbangEffectRenderer.SafeDestroyRenderTexture(ref rt);
			rt = new RenderTexture(width, height, 0, format)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.Create();
		}
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x0011880D File Offset: 0x00116A0D
	private static void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	// Token: 0x0400266C RID: 9836
	public static bool needsCapture;

	// Token: 0x0400266D RID: 9837
	private Shader flashbangEffectShader;

	// Token: 0x0400266E RID: 9838
	private RenderTexture screenRT;
}
