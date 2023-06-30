using System;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A77 RID: 2679
	[Preserve]
	internal sealed class ScreenSpaceReflectionsRenderer : PostProcessEffectRenderer<ScreenSpaceReflections>
	{
		// Token: 0x06003FD4 RID: 16340 RVA: 0x000219AE File Offset: 0x0001FBAE
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x0017884C File Offset: 0x00176A4C
		internal void CheckRT(ref RenderTexture rt, int width, int height, FilterMode filterMode, bool useMipMap)
		{
			if (rt == null || !rt.IsCreated() || rt.width != width || rt.height != height)
			{
				if (rt != null)
				{
					rt.Release();
					RuntimeUtilities.Destroy(rt);
				}
				rt = new RenderTexture(width, height, 0, RuntimeUtilities.defaultHDRRenderTextureFormat)
				{
					filterMode = filterMode,
					useMipMap = useMipMap,
					autoGenerateMips = false,
					hideFlags = HideFlags.HideAndDontSave
				};
				rt.Create();
			}
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x001788D0 File Offset: 0x00176AD0
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Screen-space Reflections");
			if (base.settings.preset.value != ScreenSpaceReflectionPreset.Custom)
			{
				int value = (int)base.settings.preset.value;
				base.settings.maximumIterationCount.value = this.m_Presets[value].maximumIterationCount;
				base.settings.thickness.value = this.m_Presets[value].thickness;
				base.settings.resolution.value = this.m_Presets[value].downsampling;
			}
			base.settings.maximumMarchDistance.value = Mathf.Max(0f, base.settings.maximumMarchDistance.value);
			int num = Mathf.ClosestPowerOfTwo(Mathf.Min(context.width, context.height));
			if (base.settings.resolution.value == ScreenSpaceReflectionResolution.Downsampled)
			{
				num >>= 1;
			}
			else if (base.settings.resolution.value == ScreenSpaceReflectionResolution.Supersampled)
			{
				num <<= 1;
			}
			int num2 = Mathf.FloorToInt(Mathf.Log((float)num, 2f) - 3f);
			num2 = Mathf.Min(num2, 12);
			this.CheckRT(ref this.m_Resolve, num, num, FilterMode.Trilinear, true);
			Texture2D texture2D = context.resources.blueNoise256[0];
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.screenSpaceReflections);
			propertySheet.properties.SetTexture(ShaderIDs.Noise, texture2D);
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetRow(0, new Vector4((float)num * 0.5f, 0f, 0f, (float)num * 0.5f));
			matrix4x.SetRow(1, new Vector4(0f, (float)num * 0.5f, 0f, (float)num * 0.5f));
			matrix4x.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
			matrix4x.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
			matrix4x *= gpuprojectionMatrix;
			propertySheet.properties.SetMatrix(ShaderIDs.ViewMatrix, context.camera.worldToCameraMatrix);
			propertySheet.properties.SetMatrix(ShaderIDs.InverseViewMatrix, context.camera.worldToCameraMatrix.inverse);
			propertySheet.properties.SetMatrix(ShaderIDs.InverseProjectionMatrix, gpuprojectionMatrix.inverse);
			propertySheet.properties.SetMatrix(ShaderIDs.ScreenSpaceProjectionMatrix, matrix4x);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(base.settings.vignette.value, base.settings.distanceFade.value, base.settings.maximumMarchDistance.value, (float)num2));
			propertySheet.properties.SetVector(ShaderIDs.Params2, new Vector4((float)context.width / (float)context.height, (float)num / (float)texture2D.width, base.settings.thickness.value, (float)base.settings.maximumIterationCount.value));
			command.GetTemporaryRT(ShaderIDs.Test, num, num, 0, FilterMode.Point, context.sourceFormat);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.Test, propertySheet, 0, false, null);
			if (context.isSceneView)
			{
				command.BlitFullscreenTriangle(context.source, this.m_Resolve, propertySheet, 1, false, null);
			}
			else
			{
				this.CheckRT(ref this.m_History, num, num, FilterMode.Bilinear, false);
				if (this.m_ResetHistory)
				{
					context.command.BlitFullscreenTriangle(context.source, this.m_History, false, null);
					this.m_ResetHistory = false;
				}
				command.GetTemporaryRT(ShaderIDs.SSRResolveTemp, num, num, 0, FilterMode.Bilinear, context.sourceFormat);
				command.BlitFullscreenTriangle(context.source, ShaderIDs.SSRResolveTemp, propertySheet, 1, false, null);
				propertySheet.properties.SetTexture(ShaderIDs.History, this.m_History);
				command.BlitFullscreenTriangle(ShaderIDs.SSRResolveTemp, this.m_Resolve, propertySheet, 2, false, null);
				command.CopyTexture(this.m_Resolve, 0, 0, this.m_History, 0, 0);
				command.ReleaseTemporaryRT(ShaderIDs.SSRResolveTemp);
			}
			command.ReleaseTemporaryRT(ShaderIDs.Test);
			if (this.m_MipIDs == null || this.m_MipIDs.Length == 0)
			{
				this.m_MipIDs = new int[12];
				for (int i = 0; i < 12; i++)
				{
					this.m_MipIDs[i] = Shader.PropertyToID("_SSRGaussianMip" + i);
				}
			}
			ComputeShader gaussianDownsample = context.resources.computeShaders.gaussianDownsample;
			int num3 = gaussianDownsample.FindKernel("KMain");
			RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this.m_Resolve);
			for (int j = 0; j < num2; j++)
			{
				num >>= 1;
				Assert.IsTrue(num > 0);
				command.GetTemporaryRT(this.m_MipIDs[j], num, num, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Default, 1, true);
				command.SetComputeTextureParam(gaussianDownsample, num3, "_Source", renderTargetIdentifier);
				command.SetComputeTextureParam(gaussianDownsample, num3, "_Result", this.m_MipIDs[j]);
				command.SetComputeVectorParam(gaussianDownsample, "_Size", new Vector4((float)num, (float)num, 1f / (float)num, 1f / (float)num));
				command.DispatchCompute(gaussianDownsample, num3, num / 8, num / 8, 1);
				command.CopyTexture(this.m_MipIDs[j], 0, 0, this.m_Resolve, 0, j + 1);
				renderTargetIdentifier = this.m_MipIDs[j];
			}
			for (int k = 0; k < num2; k++)
			{
				command.ReleaseTemporaryRT(this.m_MipIDs[k]);
			}
			propertySheet.properties.SetTexture(ShaderIDs.Resolve, this.m_Resolve);
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 3, false, null);
			command.EndSample("Screen-space Reflections");
		}

		// Token: 0x06003FD7 RID: 16343 RVA: 0x00178F1D File Offset: 0x0017711D
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_Resolve);
			RuntimeUtilities.Destroy(this.m_History);
			this.m_Resolve = null;
			this.m_History = null;
		}

		// Token: 0x04003975 RID: 14709
		private RenderTexture m_Resolve;

		// Token: 0x04003976 RID: 14710
		private RenderTexture m_History;

		// Token: 0x04003977 RID: 14711
		private int[] m_MipIDs;

		// Token: 0x04003978 RID: 14712
		private readonly ScreenSpaceReflectionsRenderer.QualityPreset[] m_Presets = new ScreenSpaceReflectionsRenderer.QualityPreset[]
		{
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 10,
				thickness = 32f,
				downsampling = ScreenSpaceReflectionResolution.Downsampled
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 16,
				thickness = 32f,
				downsampling = ScreenSpaceReflectionResolution.Downsampled
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 32,
				thickness = 16f,
				downsampling = ScreenSpaceReflectionResolution.Downsampled
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 48,
				thickness = 8f,
				downsampling = ScreenSpaceReflectionResolution.Downsampled
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 16,
				thickness = 32f,
				downsampling = ScreenSpaceReflectionResolution.FullSize
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 48,
				thickness = 16f,
				downsampling = ScreenSpaceReflectionResolution.FullSize
			},
			new ScreenSpaceReflectionsRenderer.QualityPreset
			{
				maximumIterationCount = 128,
				thickness = 12f,
				downsampling = ScreenSpaceReflectionResolution.Supersampled
			}
		};

		// Token: 0x02000F3A RID: 3898
		private class QualityPreset
		{
			// Token: 0x04004F47 RID: 20295
			public int maximumIterationCount;

			// Token: 0x04004F48 RID: 20296
			public float thickness;

			// Token: 0x04004F49 RID: 20297
			public ScreenSpaceReflectionResolution downsampling;
		}

		// Token: 0x02000F3B RID: 3899
		private enum Pass
		{
			// Token: 0x04004F4B RID: 20299
			Test,
			// Token: 0x04004F4C RID: 20300
			Resolve,
			// Token: 0x04004F4D RID: 20301
			Reproject,
			// Token: 0x04004F4E RID: 20302
			Composite
		}
	}
}
