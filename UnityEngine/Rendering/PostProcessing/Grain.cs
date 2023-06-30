using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6A RID: 2666
	[PostProcess(typeof(GrainRenderer), "Unity/Grain", true)]
	[Serializable]
	public sealed class Grain : PostProcessEffectSettings
	{
		// Token: 0x06003FA2 RID: 16290 RVA: 0x00176752 File Offset: 0x00174952
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}

		// Token: 0x04003943 RID: 14659
		[Tooltip("Enable the use of colored grain.")]
		public BoolParameter colored = new BoolParameter
		{
			value = true
		};

		// Token: 0x04003944 RID: 14660
		[Range(0f, 1f)]
		[Tooltip("Grain strength. Higher values mean more visible grain.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003945 RID: 14661
		[Range(0.3f, 3f)]
		[Tooltip("Grain particle size.")]
		public FloatParameter size = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003946 RID: 14662
		[Range(0f, 1f)]
		[DisplayName("Luminance Contribution")]
		[Tooltip("Controls the noise response curve based on scene luminance. Lower values mean less noise in dark areas.")]
		public FloatParameter lumContrib = new FloatParameter
		{
			value = 0.8f
		};
	}
}
