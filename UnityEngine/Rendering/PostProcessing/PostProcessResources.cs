using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9C RID: 2716
	public sealed class PostProcessResources : ScriptableObject
	{
		// Token: 0x040039F3 RID: 14835
		public Texture2D[] blueNoise64;

		// Token: 0x040039F4 RID: 14836
		public Texture2D[] blueNoise256;

		// Token: 0x040039F5 RID: 14837
		public PostProcessResources.SMAALuts smaaLuts;

		// Token: 0x040039F6 RID: 14838
		public PostProcessResources.Shaders shaders;

		// Token: 0x040039F7 RID: 14839
		public PostProcessResources.ComputeShaders computeShaders;

		// Token: 0x02000F44 RID: 3908
		[Serializable]
		public sealed class Shaders
		{
			// Token: 0x06005450 RID: 21584 RVA: 0x001B5237 File Offset: 0x001B3437
			public PostProcessResources.Shaders Clone()
			{
				return (PostProcessResources.Shaders)base.MemberwiseClone();
			}

			// Token: 0x04004F6B RID: 20331
			public Shader bloom;

			// Token: 0x04004F6C RID: 20332
			public Shader copy;

			// Token: 0x04004F6D RID: 20333
			public Shader copyStd;

			// Token: 0x04004F6E RID: 20334
			public Shader copyStdFromTexArray;

			// Token: 0x04004F6F RID: 20335
			public Shader copyStdFromDoubleWide;

			// Token: 0x04004F70 RID: 20336
			public Shader discardAlpha;

			// Token: 0x04004F71 RID: 20337
			public Shader depthOfField;

			// Token: 0x04004F72 RID: 20338
			public Shader finalPass;

			// Token: 0x04004F73 RID: 20339
			public Shader grainBaker;

			// Token: 0x04004F74 RID: 20340
			public Shader motionBlur;

			// Token: 0x04004F75 RID: 20341
			public Shader temporalAntialiasing;

			// Token: 0x04004F76 RID: 20342
			public Shader subpixelMorphologicalAntialiasing;

			// Token: 0x04004F77 RID: 20343
			public Shader texture2dLerp;

			// Token: 0x04004F78 RID: 20344
			public Shader uber;

			// Token: 0x04004F79 RID: 20345
			public Shader lut2DBaker;

			// Token: 0x04004F7A RID: 20346
			public Shader lightMeter;

			// Token: 0x04004F7B RID: 20347
			public Shader gammaHistogram;

			// Token: 0x04004F7C RID: 20348
			public Shader waveform;

			// Token: 0x04004F7D RID: 20349
			public Shader vectorscope;

			// Token: 0x04004F7E RID: 20350
			public Shader debugOverlays;

			// Token: 0x04004F7F RID: 20351
			public Shader deferredFog;

			// Token: 0x04004F80 RID: 20352
			public Shader scalableAO;

			// Token: 0x04004F81 RID: 20353
			public Shader multiScaleAO;

			// Token: 0x04004F82 RID: 20354
			public Shader screenSpaceReflections;
		}

		// Token: 0x02000F45 RID: 3909
		[Serializable]
		public sealed class ComputeShaders
		{
			// Token: 0x06005452 RID: 21586 RVA: 0x001B5244 File Offset: 0x001B3444
			public PostProcessResources.ComputeShaders Clone()
			{
				return (PostProcessResources.ComputeShaders)base.MemberwiseClone();
			}

			// Token: 0x04004F83 RID: 20355
			public ComputeShader autoExposure;

			// Token: 0x04004F84 RID: 20356
			public ComputeShader exposureHistogram;

			// Token: 0x04004F85 RID: 20357
			public ComputeShader lut3DBaker;

			// Token: 0x04004F86 RID: 20358
			public ComputeShader texture3dLerp;

			// Token: 0x04004F87 RID: 20359
			public ComputeShader gammaHistogram;

			// Token: 0x04004F88 RID: 20360
			public ComputeShader waveform;

			// Token: 0x04004F89 RID: 20361
			public ComputeShader vectorscope;

			// Token: 0x04004F8A RID: 20362
			public ComputeShader multiScaleAODownsample1;

			// Token: 0x04004F8B RID: 20363
			public ComputeShader multiScaleAODownsample2;

			// Token: 0x04004F8C RID: 20364
			public ComputeShader multiScaleAORender;

			// Token: 0x04004F8D RID: 20365
			public ComputeShader multiScaleAOUpsample;

			// Token: 0x04004F8E RID: 20366
			public ComputeShader gaussianDownsample;
		}

		// Token: 0x02000F46 RID: 3910
		[Serializable]
		public sealed class SMAALuts
		{
			// Token: 0x04004F8F RID: 20367
			public Texture2D area;

			// Token: 0x04004F90 RID: 20368
			public Texture2D search;
		}
	}
}
