using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000641 RID: 1601
public class GodRaysRenderer : PostProcessEffectRenderer<GodRays>
{
	// Token: 0x06002EBF RID: 11967 RVA: 0x00118A84 File Offset: 0x00116C84
	public override void Init()
	{
		if (!this.GodRayShader)
		{
			this.GodRayShader = Shader.Find("Hidden/PostProcessing/GodRays");
		}
		if (!this.ScreenClearShader)
		{
			this.ScreenClearShader = Shader.Find("Hidden/PostProcessing/ScreenClear");
		}
		if (!this.SkyMaskShader)
		{
			this.SkyMaskShader = Shader.Find("Hidden/PostProcessing/SkyMask");
		}
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x00118AE8 File Offset: 0x00116CE8
	private void DrawBorder(PostProcessRenderContext context, RenderTargetIdentifier buffer1)
	{
		PropertySheet propertySheet = context.propertySheets.Get(this.ScreenClearShader);
		Rect rect = new Rect(0f, (float)(context.height - 1), (float)context.width, 1f);
		Rect rect2 = new Rect(0f, 0f, (float)context.width, 1f);
		Rect rect3 = new Rect(0f, 0f, 1f, (float)context.height);
		Rect rect4 = new Rect((float)(context.width - 1), 0f, 1f, (float)context.height);
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(rect));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(rect2));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(rect3));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(rect4));
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x00118BF0 File Offset: 0x00116DF0
	private int GetSkyMask(PostProcessRenderContext context, ResolutionType resolution, Vector3 lightPos, int blurIterations, float blurRadius, float maxRadius)
	{
		CommandBuffer command = context.command;
		Camera camera = context.camera;
		PropertySheet propertySheet = context.propertySheets.Get(this.SkyMaskShader);
		command.BeginSample("GodRays");
		int num;
		int num2;
		int num3;
		if (resolution == ResolutionType.High)
		{
			num = context.screenWidth;
			num2 = context.screenHeight;
			num3 = 0;
		}
		else if (resolution == ResolutionType.Normal)
		{
			num = context.screenWidth / 2;
			num2 = context.screenHeight / 2;
			num3 = 0;
		}
		else
		{
			num = context.screenWidth / 4;
			num2 = context.screenHeight / 4;
			num3 = 0;
		}
		int num4 = Shader.PropertyToID("buffer1");
		int num5 = Shader.PropertyToID("buffer2");
		command.GetTemporaryRT(num4, num, num2, num3);
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * blurRadius);
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		if ((camera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
		{
			command.BlitFullscreenTriangle(context.source, num4, propertySheet, 1, false, null);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, num4, propertySheet, 2, false, null);
		}
		if (camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
		{
			this.DrawBorder(context, num4);
		}
		float num6 = blurRadius * 0.0013020834f;
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		for (int i = 0; i < blurIterations; i++)
		{
			command.GetTemporaryRT(num5, num, num2, num3);
			command.BlitFullscreenTriangle(num4, num5, propertySheet, 0, false, null);
			command.ReleaseTemporaryRT(num4);
			num6 = blurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
			command.GetTemporaryRT(num4, num, num2, num3);
			command.BlitFullscreenTriangle(num5, num4, propertySheet, 0, false, null);
			command.ReleaseTemporaryRT(num5);
			num6 = blurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		}
		command.EndSample("GodRays");
		return num4;
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x00118EC0 File Offset: 0x001170C0
	public override void Render(PostProcessRenderContext context)
	{
		Camera camera = context.camera;
		TOD_Sky instance = TOD_Sky.Instance;
		if (instance == null)
		{
			return;
		}
		Vector3 vector = camera.WorldToViewportPoint(instance.Components.LightTransform.position);
		CommandBuffer command = context.command;
		PropertySheet propertySheet = context.propertySheets.Get(this.GodRayShader);
		int skyMask = this.GetSkyMask(context, base.settings.Resolution.value, vector, base.settings.BlurIterations.value, base.settings.BlurRadius.value, base.settings.MaxRadius.value);
		Color color = Color.black;
		if ((double)vector.z >= 0.0)
		{
			if (instance.IsDay)
			{
				color = base.settings.Intensity.value * instance.SunVisibility * instance.SunRayColor;
			}
			else
			{
				color = base.settings.Intensity.value * instance.MoonVisibility * instance.MoonRayColor;
			}
		}
		propertySheet.properties.SetColor("_LightColor", color);
		command.SetGlobalTexture("_SkyMask", skyMask);
		if (base.settings.BlendMode.value == BlendModeType.Screen)
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		else
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1, false, null);
		}
		command.ReleaseTemporaryRT(skyMask);
	}

	// Token: 0x04002685 RID: 9861
	private const int PASS_SCREEN = 0;

	// Token: 0x04002686 RID: 9862
	private const int PASS_ADD = 1;

	// Token: 0x04002687 RID: 9863
	public Shader GodRayShader;

	// Token: 0x04002688 RID: 9864
	public Shader ScreenClearShader;

	// Token: 0x04002689 RID: 9865
	public Shader SkyMaskShader;
}
