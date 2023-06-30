using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000640 RID: 1600
[PostProcess(typeof(GodRaysRenderer), PostProcessEvent.BeforeStack, "Custom/GodRays", true)]
[Serializable]
public class GodRays : PostProcessEffectSettings
{
	// Token: 0x0400267E RID: 9854
	public BoolParameter UseDepth = new BoolParameter
	{
		value = true
	};

	// Token: 0x0400267F RID: 9855
	public BlendModeTypeParameter BlendMode = new BlendModeTypeParameter
	{
		value = BlendModeType.Screen
	};

	// Token: 0x04002680 RID: 9856
	public FloatParameter Intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002681 RID: 9857
	public ResolutionTypeParameter Resolution = new ResolutionTypeParameter
	{
		value = ResolutionType.High
	};

	// Token: 0x04002682 RID: 9858
	public IntParameter BlurIterations = new IntParameter
	{
		value = 2
	};

	// Token: 0x04002683 RID: 9859
	public FloatParameter BlurRadius = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x04002684 RID: 9860
	public FloatParameter MaxRadius = new FloatParameter
	{
		value = 0.5f
	};
}
