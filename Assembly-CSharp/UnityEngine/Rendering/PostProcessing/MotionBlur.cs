using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6E RID: 2670
	[PostProcess(typeof(MotionBlurRenderer), "Unity/Motion Blur", false)]
	[Serializable]
	public sealed class MotionBlur : PostProcessEffectSettings
	{
		// Token: 0x06003FAC RID: 16300 RVA: 0x00176C9D File Offset: 0x00174E9D
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.shutterAngle.value > 0f && SystemInfo.supportsMotionVectors && RenderTextureFormat.RGHalf.IsSupported() && !RuntimeUtilities.isVREnabled;
		}

		// Token: 0x04003950 RID: 14672
		[Range(0f, 360f)]
		[Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
		public FloatParameter shutterAngle = new FloatParameter
		{
			value = 270f
		};

		// Token: 0x04003951 RID: 14673
		[Range(4f, 32f)]
		[Tooltip("The amount of sample points. This affects quality and performance.")]
		public IntParameter sampleCount = new IntParameter
		{
			value = 10
		};
	}
}
