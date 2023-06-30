using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000650 RID: 1616
[PostProcess(typeof(WiggleRenderer), PostProcessEvent.AfterStack, "Custom/Wiggle", true)]
[Serializable]
public class Wiggle : PostProcessEffectSettings
{
	// Token: 0x040026BD RID: 9917
	public FloatParameter speed = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040026BE RID: 9918
	public FloatParameter scale = new FloatParameter
	{
		value = 12f
	};
}
