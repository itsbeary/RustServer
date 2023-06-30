using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A71 RID: 2673
	[Preserve]
	[Serializable]
	internal sealed class ScalableAO : IAmbientOcclusionMethod
	{
		// Token: 0x06003FC8 RID: 16328 RVA: 0x00178298 File Offset: 0x00176498
		public ScalableAO(AmbientOcclusion settings)
		{
			this.m_Settings = settings;
		}

		// Token: 0x06003FC9 RID: 16329 RVA: 0x000037BE File Offset: 0x000019BE
		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
		}

		// Token: 0x06003FCA RID: 16330 RVA: 0x001782F0 File Offset: 0x001764F0
		private void DoLazyInitialization(PostProcessRenderContext context)
		{
			this.m_PropertySheet = context.propertySheets.Get(context.resources.shaders.scalableAO);
			bool flag = false;
			if (this.m_Result == null || !this.m_Result.IsCreated())
			{
				this.m_Result = context.GetScreenSpaceTemporaryRT(0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 0, 0);
				this.m_Result.hideFlags = HideFlags.DontSave;
				this.m_Result.filterMode = FilterMode.Bilinear;
				flag = true;
			}
			else if (this.m_Result.width != context.width || this.m_Result.height != context.height)
			{
				this.m_Result.Release();
				this.m_Result.width = context.width;
				this.m_Result.height = context.height;
				flag = true;
			}
			if (flag)
			{
				this.m_Result.Create();
			}
		}

		// Token: 0x06003FCB RID: 16331 RVA: 0x001783D0 File Offset: 0x001765D0
		private void Render(PostProcessRenderContext context, CommandBuffer cmd, int occlusionSource)
		{
			this.DoLazyInitialization(context);
			this.m_Settings.radius.value = Mathf.Max(this.m_Settings.radius.value, 0.0001f);
			bool flag = this.m_Settings.quality.value < AmbientOcclusionQuality.High;
			float value = this.m_Settings.intensity.value;
			float value2 = this.m_Settings.radius.value;
			float num = (flag ? 0.5f : 1f);
			float num2 = (float)this.m_SampleCount[(int)this.m_Settings.quality.value];
			PropertySheet propertySheet = this.m_PropertySheet;
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.AOParams, new Vector4(value, value2, num, num2));
			propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - this.m_Settings.color.value);
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			int num3 = (flag ? 2 : 1);
			int occlusionTexture = ShaderIDs.OcclusionTexture1;
			int num4 = context.width / num3;
			int num5 = context.height / num3;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, num4, num5);
			cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, occlusionTexture, propertySheet, occlusionSource, false, null);
			int occlusionTexture2 = ShaderIDs.OcclusionTexture2;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			cmd.BlitFullscreenTriangle(occlusionTexture, occlusionTexture2, propertySheet, 2 + occlusionSource, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture);
			cmd.BlitFullscreenTriangle(occlusionTexture2, this.m_Result, propertySheet, 4, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture2);
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(cmd, this.m_Result, propertySheet, 7);
			}
		}

		// Token: 0x06003FCC RID: 16332 RVA: 0x001785F8 File Offset: 0x001767F8
		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			this.Render(context, command, 0);
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 5, RenderBufferLoadAction.Load, null);
			command.EndSample("Ambient Occlusion");
		}

		// Token: 0x06003FCD RID: 16333 RVA: 0x00178664 File Offset: 0x00176864
		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			this.Render(context, command, 1);
			command.EndSample("Ambient Occlusion Render");
		}

		// Token: 0x06003FCE RID: 16334 RVA: 0x00178698 File Offset: 0x00176898
		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_MRT, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 6, false, null);
			command.EndSample("Ambient Occlusion Composite");
		}

		// Token: 0x06003FCF RID: 16335 RVA: 0x001786FF File Offset: 0x001768FF
		public void Release()
		{
			RuntimeUtilities.Destroy(this.m_Result);
			this.m_Result = null;
		}

		// Token: 0x0400395C RID: 14684
		private RenderTexture m_Result;

		// Token: 0x0400395D RID: 14685
		private PropertySheet m_PropertySheet;

		// Token: 0x0400395E RID: 14686
		private AmbientOcclusion m_Settings;

		// Token: 0x0400395F RID: 14687
		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		// Token: 0x04003960 RID: 14688
		private readonly int[] m_SampleCount = new int[] { 4, 6, 10, 8, 12 };

		// Token: 0x02000F39 RID: 3897
		private enum Pass
		{
			// Token: 0x04004F3F RID: 20287
			OcclusionEstimationForward,
			// Token: 0x04004F40 RID: 20288
			OcclusionEstimationDeferred,
			// Token: 0x04004F41 RID: 20289
			HorizontalBlurForward,
			// Token: 0x04004F42 RID: 20290
			HorizontalBlurDeferred,
			// Token: 0x04004F43 RID: 20291
			VerticalBlur,
			// Token: 0x04004F44 RID: 20292
			CompositionForward,
			// Token: 0x04004F45 RID: 20293
			CompositionDeferred,
			// Token: 0x04004F46 RID: 20294
			DebugOverlay
		}
	}
}
