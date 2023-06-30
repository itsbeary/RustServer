using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5A RID: 2650
	[Preserve]
	internal sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
	{
		// Token: 0x06003F78 RID: 16248 RVA: 0x00173CDC File Offset: 0x00171EDC
		public override void Init()
		{
			this.m_Pyramid = new BloomRenderer.Level[16];
			for (int i = 0; i < 16; i++)
			{
				this.m_Pyramid[i] = new BloomRenderer.Level
				{
					down = Shader.PropertyToID("_BloomMipDown" + i),
					up = Shader.PropertyToID("_BloomMipUp" + i)
				};
			}
		}

		// Token: 0x06003F79 RID: 16249 RVA: 0x00173D50 File Offset: 0x00171F50
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("BloomPyramid");
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			float num = Mathf.Clamp(base.settings.anamorphicRatio, -1f, 1f);
			float num2 = ((num < 0f) ? (-num) : 0f);
			float num3 = ((num > 0f) ? num : 0f);
			int num4 = Mathf.FloorToInt((float)context.screenWidth / (2f - num2));
			int num5 = Mathf.FloorToInt((float)context.screenHeight / (2f - num3));
			bool flag = context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass && context.camera.stereoTargetEye == StereoTargetEyeMask.Both;
			int num6 = (flag ? (num4 * 2) : num4);
			float num7 = Mathf.Log((float)Mathf.Max(num4, num5), 2f) + Mathf.Min(base.settings.diffusion.value, 10f) - 10f;
			int num8 = Mathf.FloorToInt(num7);
			int num9 = Mathf.Clamp(num8, 1, 16);
			float num10 = 0.5f + num7 - (float)num8;
			propertySheet.properties.SetFloat(ShaderIDs.SampleScale, num10);
			float num11 = Mathf.GammaToLinearSpace(base.settings.threshold.value);
			float num12 = num11 * base.settings.softKnee.value + 1E-05f;
			Vector4 vector = new Vector4(num11, num11 - num12, num12 * 2f, 0.25f / num12);
			propertySheet.properties.SetVector(ShaderIDs.Threshold, vector);
			float num13 = Mathf.GammaToLinearSpace(base.settings.clamp.value);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(num13, 0f, 0f, 0f));
			int num14 = (base.settings.fastMode ? 1 : 0);
			RenderTargetIdentifier renderTargetIdentifier = context.source;
			for (int i = 0; i < num9; i++)
			{
				int down = this.m_Pyramid[i].down;
				int up = this.m_Pyramid[i].up;
				int num15 = ((i == 0) ? num14 : (2 + num14));
				context.GetScreenSpaceTemporaryRT(command, down, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, num6, num5);
				context.GetScreenSpaceTemporaryRT(command, up, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, num6, num5);
				command.BlitFullscreenTriangle(renderTargetIdentifier, down, propertySheet, num15, false, null);
				renderTargetIdentifier = down;
				num6 = ((flag && num6 / 2 % 2 > 0) ? (1 + num6 / 2) : (num6 / 2));
				num6 = Mathf.Max(num6, 1);
				num5 = Mathf.Max(num5 / 2, 1);
			}
			int num16 = this.m_Pyramid[num9 - 1].down;
			for (int j = num9 - 2; j >= 0; j--)
			{
				int down2 = this.m_Pyramid[j].down;
				int up2 = this.m_Pyramid[j].up;
				command.SetGlobalTexture(ShaderIDs.BloomTex, down2);
				command.BlitFullscreenTriangle(num16, up2, propertySheet, 4 + num14, false, null);
				num16 = up2;
			}
			Color linear = base.settings.color.value.linear;
			float num17 = RuntimeUtilities.Exp2(base.settings.intensity.value / 10f) - 1f;
			Vector4 vector2 = new Vector4(num10, num17, base.settings.dirtIntensity.value, (float)num9);
			if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 6);
			}
			else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
			{
				propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num17));
				context.PushDebugOverlay(command, this.m_Pyramid[0].up, propertySheet, 7 + num14);
			}
			Texture texture = ((base.settings.dirtTexture.value == null) ? RuntimeUtilities.blackTexture : base.settings.dirtTexture.value);
			float num18 = (float)texture.width / (float)texture.height;
			float num19 = (float)context.screenWidth / (float)context.screenHeight;
			Vector4 vector3 = new Vector4(1f, 1f, 0f, 0f);
			if (num18 > num19)
			{
				vector3.x = num19 / num18;
				vector3.z = (1f - vector3.x) * 0.5f;
			}
			else if (num19 > num18)
			{
				vector3.y = num18 / num19;
				vector3.w = (1f - vector3.y) * 0.5f;
			}
			PropertySheet uberSheet = context.uberSheet;
			if (base.settings.fastMode)
			{
				uberSheet.EnableKeyword("BLOOM_LOW");
			}
			else
			{
				uberSheet.EnableKeyword("BLOOM");
			}
			uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, vector3);
			uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, vector2);
			uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
			uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, texture);
			command.SetGlobalTexture(ShaderIDs.BloomTex, num16);
			for (int k = 0; k < num9; k++)
			{
				if (this.m_Pyramid[k].down != num16)
				{
					command.ReleaseTemporaryRT(this.m_Pyramid[k].down);
				}
				if (this.m_Pyramid[k].up != num16)
				{
					command.ReleaseTemporaryRT(this.m_Pyramid[k].up);
				}
			}
			command.EndSample("BloomPyramid");
			context.bloomBufferNameID = num16;
		}

		// Token: 0x040038F3 RID: 14579
		private BloomRenderer.Level[] m_Pyramid;

		// Token: 0x040038F4 RID: 14580
		private const int k_MaxPyramidSize = 16;

		// Token: 0x02000F32 RID: 3890
		private enum Pass
		{
			// Token: 0x04004F10 RID: 20240
			Prefilter13,
			// Token: 0x04004F11 RID: 20241
			Prefilter4,
			// Token: 0x04004F12 RID: 20242
			Downsample13,
			// Token: 0x04004F13 RID: 20243
			Downsample4,
			// Token: 0x04004F14 RID: 20244
			UpsampleTent,
			// Token: 0x04004F15 RID: 20245
			UpsampleBox,
			// Token: 0x04004F16 RID: 20246
			DebugOverlayThreshold,
			// Token: 0x04004F17 RID: 20247
			DebugOverlayTent,
			// Token: 0x04004F18 RID: 20248
			DebugOverlayBox
		}

		// Token: 0x02000F33 RID: 3891
		private struct Level
		{
			// Token: 0x04004F19 RID: 20249
			internal int down;

			// Token: 0x04004F1A RID: 20250
			internal int up;
		}
	}
}
