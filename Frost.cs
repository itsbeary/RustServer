using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063A RID: 1594
[PostProcess(typeof(FrostRenderer), PostProcessEvent.AfterStack, "Custom/Frost", true)]
[Serializable]
public class Frost : PostProcessEffectSettings
{
	// Token: 0x0400266F RID: 9839
	[Range(0f, 16f)]
	public FloatParameter scale = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002670 RID: 9840
	public BoolParameter enableVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x04002671 RID: 9841
	[Range(0f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002672 RID: 9842
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
