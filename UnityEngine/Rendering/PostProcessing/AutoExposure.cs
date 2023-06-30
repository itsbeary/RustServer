using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A57 RID: 2647
	[PostProcess(typeof(AutoExposureRenderer), "Unity/Auto Exposure", true)]
	[Serializable]
	public sealed class AutoExposure : PostProcessEffectSettings
	{
		// Token: 0x06003F70 RID: 16240 RVA: 0x001736A8 File Offset: 0x001718A8
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && RenderTextureFormat.RFloat.IsSupported() && context.resources.computeShaders.autoExposure && context.resources.computeShaders.exposureHistogram;
		}

		// Token: 0x040038DD RID: 14557
		[MinMax(1f, 99f)]
		[DisplayName("Filtering (%)")]
		[Tooltip("Filters the bright and dark parts of the histogram when computing the average luminance. This is to avoid very dark pixels and very bright pixels from contributing to the auto exposure. Unit is in percent.")]
		public Vector2Parameter filtering = new Vector2Parameter
		{
			value = new Vector2(50f, 95f)
		};

		// Token: 0x040038DE RID: 14558
		[Range(-9f, 9f)]
		[DisplayName("Minimum (EV)")]
		[Tooltip("Minimum average luminance to consider for auto exposure. Unit is EV.")]
		public FloatParameter minLuminance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038DF RID: 14559
		[Range(-9f, 9f)]
		[DisplayName("Maximum (EV)")]
		[Tooltip("Maximum average luminance to consider for auto exposure. Unit is EV.")]
		public FloatParameter maxLuminance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038E0 RID: 14560
		[Min(0f)]
		[DisplayName("Exposure Compensation")]
		[Tooltip("Use this to scale the global exposure of the scene.")]
		public FloatParameter keyValue = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038E1 RID: 14561
		[DisplayName("Type")]
		[Tooltip("Use \"Progressive\" if you want auto exposure to be animated. Use \"Fixed\" otherwise.")]
		public EyeAdaptationParameter eyeAdaptation = new EyeAdaptationParameter
		{
			value = EyeAdaptation.Progressive
		};

		// Token: 0x040038E2 RID: 14562
		[Min(0f)]
		[Tooltip("Adaptation speed from a dark to a light environment.")]
		public FloatParameter speedUp = new FloatParameter
		{
			value = 2f
		};

		// Token: 0x040038E3 RID: 14563
		[Min(0f)]
		[Tooltip("Adaptation speed from a light to a dark environment.")]
		public FloatParameter speedDown = new FloatParameter
		{
			value = 1f
		};
	}
}
