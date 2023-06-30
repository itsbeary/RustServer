using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200064E RID: 1614
[PostProcess(typeof(UnderWaterEffectRenderer), PostProcessEvent.AfterStack, "Custom/UnderWaterEffect", false)]
[Serializable]
public class UnderwaterPostEffect : PostProcessEffectSettings
{
	// Token: 0x040026B3 RID: 9907
	[Header("Wiggle")]
	public BoolParameter wiggle = new BoolParameter();

	// Token: 0x040026B4 RID: 9908
	public FloatParameter speed = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040026B5 RID: 9909
	public FloatParameter scale = new FloatParameter
	{
		value = 12f
	};

	// Token: 0x040026B6 RID: 9910
	[Header("Water Line")]
	public ColorParameter waterLineColor = new ColorParameter();

	// Token: 0x040026B7 RID: 9911
	[Range(1f, 4f)]
	public FixedIntParameter waterLineBlurIterations = new FixedIntParameter
	{
		value = 1
	};

	// Token: 0x040026B8 RID: 9912
	[Range(0f, 10f)]
	public FloatParameter waterLineBlurSize = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040026B9 RID: 9913
	[Header("Blur")]
	[Range(0f, 2f)]
	public FixedIntParameter downsample = new FixedIntParameter
	{
		value = 0
	};

	// Token: 0x040026BA RID: 9914
	[Range(1f, 4f)]
	public FixedIntParameter blurIterations = new FixedIntParameter
	{
		value = 1
	};

	// Token: 0x040026BB RID: 9915
	[Range(0f, 10f)]
	public FloatParameter blurSize = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040026BC RID: 9916
	public FloatParameter fadeToBlurDistance = new FloatParameter
	{
		value = 0f
	};
}
