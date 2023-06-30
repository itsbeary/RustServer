using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6F RID: 2671
	[Preserve]
	internal sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
	{
		// Token: 0x06003FAE RID: 16302 RVA: 0x000219AE File Offset: 0x0001FBAE
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003FAF RID: 16303 RVA: 0x00176D0C File Offset: 0x00174F0C
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			if (this.m_ResetHistory)
			{
				command.BlitFullscreenTriangle(context.source, context.destination, false, null);
				this.m_ResetHistory = false;
				return;
			}
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.RGHalf;
			RenderTextureFormat renderTextureFormat2 = (RenderTextureFormat.ARGB2101010.IsSupported() ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
			command.BeginSample("MotionBlur");
			int num = (int)(5f * (float)context.height / 100f);
			int num2 = ((num - 1) / 8 + 1) * 8;
			float num3 = base.settings.shutterAngle / 360f;
			propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, num3);
			propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, (float)num);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
			int velocityTex = ShaderIDs.VelocityTex;
			command.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, renderTextureFormat2, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, velocityTex, propertySheet, 0, false, null);
			int tile2RT = ShaderIDs.Tile2RT;
			command.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(velocityTex, tile2RT, propertySheet, 1, false, null);
			int tile4RT = ShaderIDs.Tile4RT;
			command.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile2RT, tile4RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile2RT);
			int tile8RT = ShaderIDs.Tile8RT;
			command.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile4RT, tile8RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile4RT);
			Vector2 vector = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
			propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, vector);
			propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (float)((int)((float)num2 / 8f)));
			int tileVRT = ShaderIDs.TileVRT;
			command.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile8RT, tileVRT, propertySheet, 3, false, null);
			command.ReleaseTemporaryRT(tile8RT);
			int neighborMaxTex = ShaderIDs.NeighborMaxTex;
			int num4 = context.width / num2;
			int num5 = context.height / num2;
			command.GetTemporaryRT(neighborMaxTex, num4, num5, 0, FilterMode.Point, renderTextureFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tileVRT, neighborMaxTex, propertySheet, 4, false, null);
			command.ReleaseTemporaryRT(tileVRT);
			propertySheet.properties.SetFloat(ShaderIDs.LoopCount, (float)Mathf.Clamp(base.settings.sampleCount / 2, 1, 64));
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5, false, null);
			command.ReleaseTemporaryRT(velocityTex);
			command.ReleaseTemporaryRT(neighborMaxTex);
			command.EndSample("MotionBlur");
		}

		// Token: 0x02000F36 RID: 3894
		private enum Pass
		{
			// Token: 0x04004F2B RID: 20267
			VelocitySetup,
			// Token: 0x04004F2C RID: 20268
			TileMax1,
			// Token: 0x04004F2D RID: 20269
			TileMax2,
			// Token: 0x04004F2E RID: 20270
			TileMaxV,
			// Token: 0x04004F2F RID: 20271
			NeighborMax,
			// Token: 0x04004F30 RID: 20272
			Reconstruction
		}
	}
}
