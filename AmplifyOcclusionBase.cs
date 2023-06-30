using System;
using UnityEngine;

// Token: 0x02000989 RID: 2441
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
	// Token: 0x0400349B RID: 13467
	[Header("Ambient Occlusion")]
	public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;

	// Token: 0x0400349C RID: 13468
	[Tooltip("Number of samples per pass.")]
	public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;

	// Token: 0x0400349D RID: 13469
	public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;

	// Token: 0x0400349E RID: 13470
	[Tooltip("Final applied intensity of the occlusion effect.")]
	[Range(0f, 1f)]
	public float Intensity = 1f;

	// Token: 0x0400349F RID: 13471
	public Color Tint = Color.black;

	// Token: 0x040034A0 RID: 13472
	[Tooltip("Radius spread of the occlusion.")]
	[Range(0f, 32f)]
	public float Radius = 2f;

	// Token: 0x040034A1 RID: 13473
	[Tooltip("Max sampling range in pixels.")]
	[Range(32f, 1024f)]
	[NonSerialized]
	public int PixelRadiusLimit = 512;

	// Token: 0x040034A2 RID: 13474
	[Tooltip("Occlusion contribution amount on relation to radius.")]
	[Range(0f, 2f)]
	[NonSerialized]
	public float RadiusIntensity = 1f;

	// Token: 0x040034A3 RID: 13475
	[Tooltip("Power exponent attenuation of the occlusion.")]
	[Range(0f, 16f)]
	public float PowerExponent = 1.8f;

	// Token: 0x040034A4 RID: 13476
	[Tooltip("Controls the initial occlusion contribution offset.")]
	[Range(0f, 0.99f)]
	public float Bias = 0.05f;

	// Token: 0x040034A5 RID: 13477
	[Tooltip("Controls the thickness occlusion contribution.")]
	[Range(0f, 1f)]
	public float Thickness = 1f;

	// Token: 0x040034A6 RID: 13478
	[Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
	public bool Downsample = true;

	// Token: 0x040034A7 RID: 13479
	[Header("Distance Fade")]
	[Tooltip("Control parameters at faraway.")]
	public bool FadeEnabled;

	// Token: 0x040034A8 RID: 13480
	[Tooltip("Distance in Unity unities that start to fade.")]
	public float FadeStart = 100f;

	// Token: 0x040034A9 RID: 13481
	[Tooltip("Length distance to performe the transition.")]
	public float FadeLength = 50f;

	// Token: 0x040034AA RID: 13482
	[Tooltip("Final Intensity parameter.")]
	[Range(0f, 1f)]
	public float FadeToIntensity;

	// Token: 0x040034AB RID: 13483
	public Color FadeToTint = Color.black;

	// Token: 0x040034AC RID: 13484
	[Tooltip("Final Radius parameter.")]
	[Range(0f, 32f)]
	public float FadeToRadius = 2f;

	// Token: 0x040034AD RID: 13485
	[Tooltip("Final PowerExponent parameter.")]
	[Range(0f, 16f)]
	public float FadeToPowerExponent = 1.8f;

	// Token: 0x040034AE RID: 13486
	[Tooltip("Final Thickness parameter.")]
	[Range(0f, 1f)]
	public float FadeToThickness = 1f;

	// Token: 0x040034AF RID: 13487
	[Header("Bilateral Blur")]
	public bool BlurEnabled = true;

	// Token: 0x040034B0 RID: 13488
	[Tooltip("Radius in screen pixels.")]
	[Range(1f, 4f)]
	public int BlurRadius = 3;

	// Token: 0x040034B1 RID: 13489
	[Tooltip("Number of times that the Blur will repeat.")]
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x040034B2 RID: 13490
	[Tooltip("0 - Blured, 1 - Sharpened.")]
	[Range(0f, 20f)]
	public float BlurSharpness = 10f;

	// Token: 0x040034B3 RID: 13491
	[Header("Temporal Filter")]
	[Tooltip("Accumulates the effect over the time.")]
	public bool FilterEnabled = true;

	// Token: 0x040034B4 RID: 13492
	[Tooltip("Controls the accumulation decayment. 0 - Faster update, more flicker. 1 - Slow update (ghosting on moving objects), less flicker.")]
	[Range(0f, 1f)]
	public float FilterBlending = 0.5f;

	// Token: 0x040034B5 RID: 13493
	[Tooltip("Controls the discard sensibility based on the motion of the scene and objects. 0 - Discard less, reuse more (more ghost effect). 1 - Discard more, reuse less (less ghost effect).")]
	[Range(0f, 1f)]
	public float FilterResponse = 0.5f;

	// Token: 0x040034B6 RID: 13494
	[Tooltip("Enables directional variations.")]
	[NonSerialized]
	public bool TemporalDirections = true;

	// Token: 0x040034B7 RID: 13495
	[Tooltip("Enables offset variations.")]
	[NonSerialized]
	public bool TemporalOffsets = true;

	// Token: 0x040034B8 RID: 13496
	[Tooltip("Reduces ghosting effect near the objects's edges while moving.")]
	[NonSerialized]
	public bool TemporalDilation;

	// Token: 0x040034B9 RID: 13497
	[Tooltip("Uses the object movement information for calc new areas of occlusion.")]
	[NonSerialized]
	public bool UseMotionVectors = true;

	// Token: 0x02000EDC RID: 3804
	public enum ApplicationMethod
	{
		// Token: 0x04004D9A RID: 19866
		PostEffect,
		// Token: 0x04004D9B RID: 19867
		Deferred,
		// Token: 0x04004D9C RID: 19868
		Debug
	}

	// Token: 0x02000EDD RID: 3805
	public enum PerPixelNormalSource
	{
		// Token: 0x04004D9E RID: 19870
		None,
		// Token: 0x04004D9F RID: 19871
		Camera,
		// Token: 0x04004DA0 RID: 19872
		GBuffer,
		// Token: 0x04004DA1 RID: 19873
		GBufferOctaEncoded
	}

	// Token: 0x02000EDE RID: 3806
	public enum SampleCountLevel
	{
		// Token: 0x04004DA3 RID: 19875
		Low,
		// Token: 0x04004DA4 RID: 19876
		Medium,
		// Token: 0x04004DA5 RID: 19877
		High,
		// Token: 0x04004DA6 RID: 19878
		VeryHigh
	}
}
