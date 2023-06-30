using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A65 RID: 2661
	[PostProcess(typeof(DepthOfFieldRenderer), "Unity/Depth of Field", false)]
	[Serializable]
	public sealed class DepthOfField : PostProcessEffectSettings
	{
		// Token: 0x06003F92 RID: 16274 RVA: 0x00175F6A File Offset: 0x0017416A
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && SystemInfo.graphicsShaderLevel >= 35;
		}

		// Token: 0x04003935 RID: 14645
		[Min(0.1f)]
		[Tooltip("Distance to the point of focus.")]
		public FloatParameter focusDistance = new FloatParameter
		{
			value = 10f
		};

		// Token: 0x04003936 RID: 14646
		[Range(0.05f, 32f)]
		[Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
		public FloatParameter aperture = new FloatParameter
		{
			value = 5.6f
		};

		// Token: 0x04003937 RID: 14647
		[Range(1f, 300f)]
		[Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
		public FloatParameter focalLength = new FloatParameter
		{
			value = 50f
		};

		// Token: 0x04003938 RID: 14648
		[DisplayName("Max Blur Size")]
		[Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects performances (the larger the kernel is, the longer the GPU time is required).")]
		public KernelSizeParameter kernelSize = new KernelSizeParameter
		{
			value = KernelSize.Medium
		};
	}
}
