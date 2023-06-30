using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000634 RID: 1588
[PostProcess(typeof(DepthOfFieldEffectRenderer), "Unity/Depth of Field (Custom)", false)]
[Serializable]
public class DepthOfFieldEffect : PostProcessEffectSettings
{
	// Token: 0x0400265B RID: 9819
	public FloatParameter focalLength = new FloatParameter
	{
		value = 10f
	};

	// Token: 0x0400265C RID: 9820
	public FloatParameter focalSize = new FloatParameter
	{
		value = 0.05f
	};

	// Token: 0x0400265D RID: 9821
	public FloatParameter aperture = new FloatParameter
	{
		value = 11.5f
	};

	// Token: 0x0400265E RID: 9822
	public FloatParameter maxBlurSize = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x0400265F RID: 9823
	public BoolParameter highResolution = new BoolParameter
	{
		value = true
	};

	// Token: 0x04002660 RID: 9824
	public DOFBlurSampleCountParameter blurSampleCount = new DOFBlurSampleCountParameter
	{
		value = DOFBlurSampleCount.Low
	};

	// Token: 0x04002661 RID: 9825
	public Transform focalTransform;
}
