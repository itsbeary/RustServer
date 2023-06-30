using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000638 RID: 1592
[PostProcess(typeof(FlashbangEffectRenderer), PostProcessEvent.AfterStack, "Custom/FlashbangEffect", false)]
[Serializable]
public class FlashbangEffect : PostProcessEffectSettings
{
	// Token: 0x0400266A RID: 9834
	[Range(0f, 1f)]
	public FloatParameter burnIntensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400266B RID: 9835
	[Range(0f, 1f)]
	public FloatParameter whiteoutIntensity = new FloatParameter
	{
		value = 0f
	};
}
