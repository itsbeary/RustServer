using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A52 RID: 2642
	[PostProcess(typeof(AmbientOcclusionRenderer), "Unity/Ambient Occlusion", true)]
	[Serializable]
	public sealed class AmbientOcclusion : PostProcessEffectSettings
	{
		// Token: 0x06003F5F RID: 16223 RVA: 0x0017336C File Offset: 0x0017156C
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			bool flag = this.enabled.value && this.intensity.value > 0f;
			if (this.mode.value == AmbientOcclusionMode.ScalableAmbientObscurance)
			{
				flag &= !RuntimeUtilities.scriptableRenderPipelineActive;
				if (context != null)
				{
					flag &= context.resources.shaders.scalableAO && context.resources.shaders.scalableAO.isSupported;
				}
			}
			else if (this.mode.value == AmbientOcclusionMode.MultiScaleVolumetricObscurance)
			{
				if (context != null)
				{
					flag &= context.resources.shaders.multiScaleAO && context.resources.shaders.multiScaleAO.isSupported && context.resources.computeShaders.multiScaleAODownsample1 && context.resources.computeShaders.multiScaleAODownsample2 && context.resources.computeShaders.multiScaleAORender && context.resources.computeShaders.multiScaleAOUpsample;
				}
				flag &= SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && RenderTextureFormat.RFloat.IsSupported() && RenderTextureFormat.RHalf.IsSupported() && RenderTextureFormat.R8.IsSupported();
			}
			return flag;
		}

		// Token: 0x040038CE RID: 14542
		[Tooltip("The ambient occlusion method to use. \"Multi Scale Volumetric Obscurance\" is higher quality and faster on desktop & console platforms but requires compute shader support.")]
		public AmbientOcclusionModeParameter mode = new AmbientOcclusionModeParameter
		{
			value = AmbientOcclusionMode.MultiScaleVolumetricObscurance
		};

		// Token: 0x040038CF RID: 14543
		[Range(0f, 4f)]
		[Tooltip("The degree of darkness added by ambient occlusion. Higher values produce darker areas.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038D0 RID: 14544
		[ColorUsage(false)]
		[Tooltip("The custom color to use for the ambient occlusion. The default is black.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.black
		};

		// Token: 0x040038D1 RID: 14545
		[Tooltip("Check this box to mark this Volume as to only affect ambient lighting. This mode is only available with the Deferred rendering path and HDR rendering. Objects rendered with the Forward rendering path won't get any ambient occlusion.")]
		public BoolParameter ambientOnly = new BoolParameter
		{
			value = true
		};

		// Token: 0x040038D2 RID: 14546
		[Range(-8f, 0f)]
		public FloatParameter noiseFilterTolerance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038D3 RID: 14547
		[Range(-8f, -1f)]
		public FloatParameter blurTolerance = new FloatParameter
		{
			value = -4.6f
		};

		// Token: 0x040038D4 RID: 14548
		[Range(-12f, -1f)]
		public FloatParameter upsampleTolerance = new FloatParameter
		{
			value = -12f
		};

		// Token: 0x040038D5 RID: 14549
		[Range(1f, 10f)]
		[Tooltip("This modifies the thickness of occluders. It increases the size of dark areas and also introduces a dark halo around objects.")]
		public FloatParameter thicknessModifier = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038D6 RID: 14550
		[Range(0f, 1f)]
		[Tooltip("Modifies the influence of direct lighting on ambient occlusion.")]
		public FloatParameter directLightingStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038D7 RID: 14551
		[Tooltip("The radius of sample points. This affects the size of darkened areas.")]
		public FloatParameter radius = new FloatParameter
		{
			value = 0.25f
		};

		// Token: 0x040038D8 RID: 14552
		[Tooltip("The number of sample points. This affects both quality and performance. For \"Lowest\", \"Low\", and \"Medium\", passes are downsampled. For \"High\" and \"Ultra\", they are not and therefore you should only \"High\" and \"Ultra\" on high-end hardware.")]
		public AmbientOcclusionQualityParameter quality = new AmbientOcclusionQualityParameter
		{
			value = AmbientOcclusionQuality.Medium
		};
	}
}
