using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A79 RID: 2681
	[Preserve]
	[Serializable]
	public sealed class TemporalAntialiasing
	{
		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06003FDC RID: 16348 RVA: 0x001791D0 File Offset: 0x001773D0
		// (set) Token: 0x06003FDD RID: 16349 RVA: 0x001791D8 File Offset: 0x001773D8
		public Vector2 jitter { get; private set; }

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06003FDE RID: 16350 RVA: 0x001791E1 File Offset: 0x001773E1
		// (set) Token: 0x06003FDF RID: 16351 RVA: 0x001791E9 File Offset: 0x001773E9
		public Vector2 jitterRaw { get; private set; }

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06003FE0 RID: 16352 RVA: 0x001791F2 File Offset: 0x001773F2
		// (set) Token: 0x06003FE1 RID: 16353 RVA: 0x001791FA File Offset: 0x001773FA
		public int sampleIndex { get; private set; }

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06003FE2 RID: 16354 RVA: 0x00179203 File Offset: 0x00177403
		// (set) Token: 0x06003FE3 RID: 16355 RVA: 0x0017920B File Offset: 0x0017740B
		public int sampleCount { get; set; }

		// Token: 0x06003FE4 RID: 16356 RVA: 0x00179214 File Offset: 0x00177414
		public bool IsSupported()
		{
			return SystemInfo.supportedRenderTargetCount >= 2 && SystemInfo.supportsMotionVectors && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x000219AE File Offset: 0x0001FBAE
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x00179232 File Offset: 0x00177432
		internal void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x0017923C File Offset: 0x0017743C
		private Vector2 GenerateRandomOffset()
		{
			Vector2 vector = new Vector2(HaltonSeq.Get((this.sampleIndex & 1023) + 1, 2) - 0.5f, HaltonSeq.Get((this.sampleIndex & 1023) + 1, 3) - 0.5f);
			int num = this.sampleIndex + 1;
			this.sampleIndex = num;
			if (num >= this.sampleCount)
			{
				this.sampleIndex = 0;
			}
			return vector;
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x001792A4 File Offset: 0x001774A4
		public Matrix4x4 GetJitteredProjectionMatrix(Camera camera)
		{
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			Matrix4x4 matrix4x;
			if (this.jitteredMatrixFunc != null)
			{
				matrix4x = this.jitteredMatrixFunc(camera, this.jitter);
			}
			else
			{
				matrix4x = (camera.orthographic ? RuntimeUtilities.GetJitteredOrthographicProjectionMatrix(camera, this.jitter) : RuntimeUtilities.GetJitteredPerspectiveProjectionMatrix(camera, this.jitter));
			}
			this.jitterRaw = this.jitter;
			this.jitter = new Vector2(this.jitter.x / (float)camera.pixelWidth, this.jitter.y / (float)camera.pixelHeight);
			return matrix4x;
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x00179354 File Offset: 0x00177554
		public void ConfigureJitteredProjectionMatrix(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			camera.nonJitteredProjectionMatrix = camera.projectionMatrix;
			camera.projectionMatrix = this.GetJitteredProjectionMatrix(camera);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x00179388 File Offset: 0x00177588
		public void ConfigureStereoJitteredProjectionMatrices(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			for (Camera.StereoscopicEye stereoscopicEye = Camera.StereoscopicEye.Left; stereoscopicEye <= Camera.StereoscopicEye.Right; stereoscopicEye++)
			{
				context.camera.CopyStereoDeviceProjectionMatrixToNonJittered(stereoscopicEye);
				Matrix4x4 stereoNonJitteredProjectionMatrix = context.camera.GetStereoNonJitteredProjectionMatrix(stereoscopicEye);
				Matrix4x4 matrix4x = RuntimeUtilities.GenerateJitteredProjectionMatrixFromOriginal(context, stereoNonJitteredProjectionMatrix, this.jitter);
				context.camera.SetStereoProjectionMatrix(stereoscopicEye, matrix4x);
			}
			this.jitter = new Vector2(this.jitter.x / (float)context.screenWidth, this.jitter.y / (float)context.screenHeight);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x00179438 File Offset: 0x00177638
		private void GenerateHistoryName(RenderTexture rt, int id, PostProcessRenderContext context)
		{
			rt.name = "Temporal Anti-aliasing History id #" + id;
			if (context.stereoActive)
			{
				rt.name = rt.name + " for eye " + context.xrActiveEye;
			}
		}

		// Token: 0x06003FEC RID: 16364 RVA: 0x00179484 File Offset: 0x00177684
		private RenderTexture CheckHistory(int id, PostProcessRenderContext context)
		{
			int xrActiveEye = context.xrActiveEye;
			if (this.m_HistoryTextures[xrActiveEye] == null)
			{
				this.m_HistoryTextures[xrActiveEye] = new RenderTexture[2];
			}
			RenderTexture renderTexture = this.m_HistoryTextures[xrActiveEye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated())
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(renderTexture, id, context);
				renderTexture.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = renderTexture;
				context.command.BlitFullscreenTriangle(context.source, renderTexture, false, null);
			}
			else if (renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture screenSpaceTemporaryRT = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(screenSpaceTemporaryRT, id, context);
				screenSpaceTemporaryRT.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = screenSpaceTemporaryRT;
				context.command.BlitFullscreenTriangle(renderTexture, screenSpaceTemporaryRT, false, null);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			return this.m_HistoryTextures[xrActiveEye][id];
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x001795A4 File Offset: 0x001777A4
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.temporalAntialiasing);
			CommandBuffer command = context.command;
			command.BeginSample("TemporalAntialiasing");
			int num = this.m_HistoryPingPong[context.xrActiveEye];
			RenderTexture renderTexture = this.CheckHistory(++num % 2, context);
			RenderTexture renderTexture2 = this.CheckHistory(++num % 2, context);
			this.m_HistoryPingPong[context.xrActiveEye] = (num + 1) % 2;
			propertySheet.properties.SetVector(ShaderIDs.Jitter, this.jitter);
			propertySheet.properties.SetFloat(ShaderIDs.Sharpness, this.sharpness);
			propertySheet.properties.SetVector(ShaderIDs.FinalBlendParameters, new Vector4(this.stationaryBlending, this.motionBlending, 6000f, 0f));
			propertySheet.properties.SetTexture(ShaderIDs.HistoryTex, renderTexture);
			int num2 = (context.camera.orthographic ? 1 : 0);
			this.m_Mrt[0] = context.destination;
			this.m_Mrt[1] = renderTexture2;
			command.BlitFullscreenTriangle(context.source, this.m_Mrt, context.source, propertySheet, num2, false, null);
			command.EndSample("TemporalAntialiasing");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003FEE RID: 16366 RVA: 0x001796FC File Offset: 0x001778FC
		internal void Release()
		{
			if (this.m_HistoryTextures != null)
			{
				for (int i = 0; i < this.m_HistoryTextures.Length; i++)
				{
					if (this.m_HistoryTextures[i] != null)
					{
						for (int j = 0; j < this.m_HistoryTextures[i].Length; j++)
						{
							RenderTexture.ReleaseTemporary(this.m_HistoryTextures[i][j]);
							this.m_HistoryTextures[i][j] = null;
						}
						this.m_HistoryTextures[i] = null;
					}
				}
			}
			this.sampleIndex = 0;
			this.m_HistoryPingPong[0] = 0;
			this.m_HistoryPingPong[1] = 0;
			this.ResetHistory();
		}

		// Token: 0x0400397A RID: 14714
		[Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable, but blurrier, output.")]
		[Range(0.1f, 1f)]
		public float jitterSpread = 0.75f;

		// Token: 0x0400397B RID: 14715
		[Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
		[Range(0f, 3f)]
		public float sharpness = 0.25f;

		// Token: 0x0400397C RID: 14716
		[Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float stationaryBlending = 0.95f;

		// Token: 0x0400397D RID: 14717
		[Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float motionBlending = 0.85f;

		// Token: 0x0400397E RID: 14718
		public Func<Camera, Vector2, Matrix4x4> jitteredMatrixFunc;

		// Token: 0x04003981 RID: 14721
		private readonly RenderTargetIdentifier[] m_Mrt = new RenderTargetIdentifier[2];

		// Token: 0x04003982 RID: 14722
		private bool m_ResetHistory = true;

		// Token: 0x04003985 RID: 14725
		private const int k_NumEyes = 2;

		// Token: 0x04003986 RID: 14726
		private const int k_NumHistoryTextures = 2;

		// Token: 0x04003987 RID: 14727
		private readonly RenderTexture[][] m_HistoryTextures = new RenderTexture[2][];

		// Token: 0x04003988 RID: 14728
		private readonly int[] m_HistoryPingPong = new int[2];

		// Token: 0x02000F3E RID: 3902
		private enum Pass
		{
			// Token: 0x04004F58 RID: 20312
			SolverDilate,
			// Token: 0x04004F59 RID: 20313
			SolverNoDilate
		}
	}
}
