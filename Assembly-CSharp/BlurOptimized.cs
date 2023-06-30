using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062F RID: 1583
[PostProcess(typeof(BlurOptimizedRenderer), PostProcessEvent.AfterStack, "Custom/BlurOptimized", true)]
[Serializable]
public class BlurOptimized : PostProcessEffectSettings
{
	// Token: 0x0400264E RID: 9806
	[Range(0f, 2f)]
	public FixedIntParameter downsample = new FixedIntParameter
	{
		value = 0
	};

	// Token: 0x0400264F RID: 9807
	[Range(1f, 4f)]
	public FixedIntParameter blurIterations = new FixedIntParameter
	{
		value = 1
	};

	// Token: 0x04002650 RID: 9808
	[Range(0f, 10f)]
	public FloatParameter blurSize = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002651 RID: 9809
	public FloatParameter fadeToBlurDistance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002652 RID: 9810
	public BlurTypeParameter blurType = new BlurTypeParameter
	{
		value = BlurType.StandardGauss
	};
}
