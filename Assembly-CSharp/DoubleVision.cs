using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000636 RID: 1590
[PostProcess(typeof(DoubleVisionRenderer), PostProcessEvent.AfterStack, "Custom/DoubleVision", true)]
[Serializable]
public class DoubleVision : PostProcessEffectSettings
{
	// Token: 0x04002665 RID: 9829
	[Range(0f, 1f)]
	public Vector2Parameter displace = new Vector2Parameter
	{
		value = Vector2.zero
	};

	// Token: 0x04002666 RID: 9830
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};
}
