using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000642 RID: 1602
[PostProcess(typeof(GreyScaleRenderer), PostProcessEvent.AfterStack, "Custom/GreyScale", true)]
[Serializable]
public class GreyScale : PostProcessEffectSettings
{
	// Token: 0x0400268A RID: 9866
	[Range(0f, 1f)]
	public FloatParameter redLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400268B RID: 9867
	[Range(0f, 1f)]
	public FloatParameter greenLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400268C RID: 9868
	[Range(0f, 1f)]
	public FloatParameter blueLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400268D RID: 9869
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400268E RID: 9870
	[ColorUsage(false, true)]
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};
}
