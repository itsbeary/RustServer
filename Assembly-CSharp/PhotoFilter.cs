using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000646 RID: 1606
[PostProcess(typeof(PhotoFilterRenderer), PostProcessEvent.AfterStack, "Custom/PhotoFilter", true)]
[Serializable]
public class PhotoFilter : PostProcessEffectSettings
{
	// Token: 0x0400269B RID: 9883
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x0400269C RID: 9884
	[Range(0f, 1f)]
	public FloatParameter density = new FloatParameter
	{
		value = 0f
	};
}
