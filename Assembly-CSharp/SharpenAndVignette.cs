using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200064C RID: 1612
[PostProcess(typeof(SharpenAndVignetteRenderer), PostProcessEvent.AfterStack, "Custom/SharpenAndVignette", true)]
[Serializable]
public class SharpenAndVignette : PostProcessEffectSettings
{
	// Token: 0x040026AC RID: 9900
	[Header("Sharpen")]
	public BoolParameter applySharpen = new BoolParameter
	{
		value = true
	};

	// Token: 0x040026AD RID: 9901
	[Range(0f, 5f)]
	public FloatParameter strength = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040026AE RID: 9902
	[Range(0f, 1f)]
	public FloatParameter clamp = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040026AF RID: 9903
	[Header("Vignette")]
	public BoolParameter applyVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x040026B0 RID: 9904
	[Range(-100f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040026B1 RID: 9905
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
