using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A66 RID: 2662
	[Preserve]
	internal sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
	{
		// Token: 0x06003F94 RID: 16276 RVA: 0x00175FF0 File Offset: 0x001741F0
		public DepthOfFieldRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_CoCHistoryTextures[i] = new RenderTexture[2];
				this.m_HistoryPingPong[i] = 0;
			}
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x0000441C File Offset: 0x0000261C
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x0017603E File Offset: 0x0017423E
		private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
		{
			if (primary.IsSupported())
			{
				return primary;
			}
			if (secondary.IsSupported())
			{
				return secondary;
			}
			return RenderTextureFormat.Default;
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x00176058 File Offset: 0x00174258
		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.settings.kernelSize.value * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x00176094 File Offset: 0x00174294
		private RenderTexture CheckHistory(int eye, int id, PostProcessRenderContext context, RenderTextureFormat format)
		{
			RenderTexture renderTexture = this.m_CoCHistoryTextures[eye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated() || renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, format, RenderTextureReadWrite.Linear, 0, 0);
				renderTexture.name = string.Concat(new object[] { "CoC History, Eye: ", eye, ", ID: ", id });
				renderTexture.filterMode = FilterMode.Bilinear;
				renderTexture.Create();
				this.m_CoCHistoryTextures[eye][id] = renderTexture;
			}
			return renderTexture;
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x00176144 File Offset: 0x00174344
		public override void Render(PostProcessRenderContext context)
		{
			RenderTextureFormat sourceFormat = context.sourceFormat;
			RenderTextureFormat renderTextureFormat = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
			float num = 0.024f * ((float)context.height / 1080f);
			float num2 = base.settings.focalLength.value / 1000f;
			float num3 = Mathf.Max(base.settings.focusDistance.value, num2);
			float num4 = (float)context.screenWidth / (float)context.screenHeight;
			float num5 = num2 * num2 / (base.settings.aperture.value * (num3 - num2) * num * 2f);
			float num6 = this.CalculateMaxCoCRadius(context.screenHeight);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Distance, num3);
			propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, num5);
			propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, num6);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / num6);
			propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num4);
			CommandBuffer command = context.command;
			command.BeginSample("DepthOfField");
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.CoCTex, 0, renderTextureFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, ShaderIDs.CoCTex, propertySheet, 0, false, null);
			if (context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				float motionBlending = context.temporalAntialiasing.motionBlending;
				float num7 = (this.m_ResetHistory ? 0f : motionBlending);
				Vector2 jitter = context.temporalAntialiasing.jitter;
				propertySheet.properties.SetVector(ShaderIDs.TaaParams, new Vector3(jitter.x, jitter.y, num7));
				int num8 = this.m_HistoryPingPong[context.xrActiveEye];
				RenderTexture renderTexture = this.CheckHistory(context.xrActiveEye, ++num8 % 2, context, renderTextureFormat);
				RenderTexture renderTexture2 = this.CheckHistory(context.xrActiveEye, ++num8 % 2, context, renderTextureFormat);
				this.m_HistoryPingPong[context.xrActiveEye] = (num8 + 1) % 2;
				command.BlitFullscreenTriangle(renderTexture, renderTexture2, propertySheet, 1, false, null);
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
				command.SetGlobalTexture(ShaderIDs.CoCTex, renderTexture2);
			}
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTex, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.DepthOfFieldTex, propertySheet, 2, false, null);
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTemp, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTex, ShaderIDs.DepthOfFieldTemp, propertySheet, (int)(3 + base.settings.kernelSize.value), false, null);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTemp, ShaderIDs.DepthOfFieldTex, propertySheet, 7, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
			if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 9);
			}
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
			if (!context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
			}
			command.EndSample("DepthOfField");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x0017651C File Offset: 0x0017471C
		public override void Release()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < this.m_CoCHistoryTextures[i].Length; j++)
				{
					RenderTexture.ReleaseTemporary(this.m_CoCHistoryTextures[i][j]);
					this.m_CoCHistoryTextures[i][j] = null;
				}
				this.m_HistoryPingPong[i] = 0;
			}
			this.ResetHistory();
		}

		// Token: 0x04003939 RID: 14649
		private const int k_NumEyes = 2;

		// Token: 0x0400393A RID: 14650
		private const int k_NumCoCHistoryTextures = 2;

		// Token: 0x0400393B RID: 14651
		private readonly RenderTexture[][] m_CoCHistoryTextures = new RenderTexture[2][];

		// Token: 0x0400393C RID: 14652
		private int[] m_HistoryPingPong = new int[2];

		// Token: 0x0400393D RID: 14653
		private const float k_FilmHeight = 0.024f;

		// Token: 0x02000F35 RID: 3893
		private enum Pass
		{
			// Token: 0x04004F20 RID: 20256
			CoCCalculation,
			// Token: 0x04004F21 RID: 20257
			CoCTemporalFilter,
			// Token: 0x04004F22 RID: 20258
			DownsampleAndPrefilter,
			// Token: 0x04004F23 RID: 20259
			BokehSmallKernel,
			// Token: 0x04004F24 RID: 20260
			BokehMediumKernel,
			// Token: 0x04004F25 RID: 20261
			BokehLargeKernel,
			// Token: 0x04004F26 RID: 20262
			BokehVeryLargeKernel,
			// Token: 0x04004F27 RID: 20263
			PostFilter,
			// Token: 0x04004F28 RID: 20264
			Combine,
			// Token: 0x04004F29 RID: 20265
			DebugOverlay
		}
	}
}
