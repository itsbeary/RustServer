using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7C RID: 2684
	[PostProcess(typeof(VignetteRenderer), "Unity/Vignette", true)]
	[Serializable]
	public sealed class Vignette : PostProcessEffectSettings
	{
		// Token: 0x06003FF1 RID: 16369 RVA: 0x001797FC File Offset: 0x001779FC
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && ((this.mode.value == VignetteMode.Classic && this.intensity.value > 0f) || (this.mode.value == VignetteMode.Masked && this.opacity.value > 0f && this.mask.value != null));
		}

		// Token: 0x0400398C RID: 14732
		[Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
		public VignetteModeParameter mode = new VignetteModeParameter
		{
			value = VignetteMode.Classic
		};

		// Token: 0x0400398D RID: 14733
		[Tooltip("Vignette color.")]
		public ColorParameter color = new ColorParameter
		{
			value = new Color(0f, 0f, 0f, 1f)
		};

		// Token: 0x0400398E RID: 14734
		[Tooltip("Sets the vignette center point (screen center is [0.5, 0.5]).")]
		public Vector2Parameter center = new Vector2Parameter
		{
			value = new Vector2(0.5f, 0.5f)
		};

		// Token: 0x0400398F RID: 14735
		[Range(0f, 1f)]
		[Tooltip("Amount of vignetting on screen.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003990 RID: 14736
		[Range(0.01f, 1f)]
		[Tooltip("Smoothness of the vignette borders.")]
		public FloatParameter smoothness = new FloatParameter
		{
			value = 0.2f
		};

		// Token: 0x04003991 RID: 14737
		[Range(0f, 1f)]
		[Tooltip("Lower values will make a square-ish vignette.")]
		public FloatParameter roundness = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003992 RID: 14738
		[Tooltip("Set to true to mark the vignette to be perfectly round. False will make its shape dependent on the current aspect ratio.")]
		public BoolParameter rounded = new BoolParameter
		{
			value = false
		};

		// Token: 0x04003993 RID: 14739
		[Tooltip("A black and white mask to use as a vignette.")]
		public TextureParameter mask = new TextureParameter
		{
			value = null
		};

		// Token: 0x04003994 RID: 14740
		[Range(0f, 1f)]
		[Tooltip("Mask opacity.")]
		public FloatParameter opacity = new FloatParameter
		{
			value = 1f
		};
	}
}
