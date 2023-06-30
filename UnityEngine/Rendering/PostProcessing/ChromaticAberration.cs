using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5B RID: 2651
	[PostProcess(typeof(ChromaticAberrationRenderer), "Unity/Chromatic Aberration", true)]
	[Serializable]
	public sealed class ChromaticAberration : PostProcessEffectSettings
	{
		// Token: 0x06003F7B RID: 16251 RVA: 0x00174378 File Offset: 0x00172578
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}

		// Token: 0x040038F5 RID: 14581
		[Tooltip("Shifts the hue of chromatic aberrations.")]
		public TextureParameter spectralLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x040038F6 RID: 14582
		[Range(0f, 1f)]
		[Tooltip("Amount of tangential distortion.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038F7 RID: 14583
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public BoolParameter fastMode = new BoolParameter
		{
			value = false
		};
	}
}
