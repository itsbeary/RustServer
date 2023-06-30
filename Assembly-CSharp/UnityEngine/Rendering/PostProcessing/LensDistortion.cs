using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6C RID: 2668
	[PostProcess(typeof(LensDistortionRenderer), "Unity/Lens Distortion", true)]
	[Serializable]
	public sealed class LensDistortion : PostProcessEffectSettings
	{
		// Token: 0x06003FA8 RID: 16296 RVA: 0x00176A68 File Offset: 0x00174C68
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && !Mathf.Approximately(this.intensity, 0f) && (this.intensityX > 0f || this.intensityY > 0f) && !RuntimeUtilities.isVREnabled;
		}

		// Token: 0x0400394A RID: 14666
		[Range(-100f, 100f)]
		[Tooltip("Total distortion amount.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400394B RID: 14667
		[Range(0f, 1f)]
		[DisplayName("X Multiplier")]
		[Tooltip("Intensity multiplier on the x-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityX = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400394C RID: 14668
		[Range(0f, 1f)]
		[DisplayName("Y Multiplier")]
		[Tooltip("Intensity multiplier on the y-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityY = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400394D RID: 14669
		[Space]
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (x-axis).")]
		public FloatParameter centerX = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400394E RID: 14670
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (y-axis).")]
		public FloatParameter centerY = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400394F RID: 14671
		[Space]
		[Range(0.01f, 5f)]
		[Tooltip("Global screen scaling.")]
		public FloatParameter scale = new FloatParameter
		{
			value = 1f
		};
	}
}
