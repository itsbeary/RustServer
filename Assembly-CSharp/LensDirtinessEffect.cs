using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000644 RID: 1604
[PostProcess(typeof(LensDirtinessRenderer), PostProcessEvent.AfterStack, "Custom/LensDirtiness", true)]
[Serializable]
public class LensDirtinessEffect : PostProcessEffectSettings
{
	// Token: 0x04002692 RID: 9874
	public TextureParameter dirtinessTexture = new TextureParameter();

	// Token: 0x04002693 RID: 9875
	public BoolParameter sceneTintsBloom = new BoolParameter
	{
		value = false
	};

	// Token: 0x04002694 RID: 9876
	public FloatParameter gain = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002695 RID: 9877
	public FloatParameter threshold = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002696 RID: 9878
	public FloatParameter bloomSize = new FloatParameter
	{
		value = 5f
	};

	// Token: 0x04002697 RID: 9879
	public FloatParameter dirtiness = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002698 RID: 9880
	public ColorParameter bloomColor = new ColorParameter
	{
		value = Color.white
	};
}
