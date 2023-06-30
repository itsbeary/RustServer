using System;
using UnityEngine;

// Token: 0x02000761 RID: 1889
[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	// Token: 0x06003494 RID: 13460 RVA: 0x00144BDF File Offset: 0x00142DDF
	public RecoilProperties GetRecoil()
	{
		if (!(this.newRecoilOverride != null))
		{
			return this;
		}
		return this.newRecoilOverride;
	}

	// Token: 0x04002B07 RID: 11015
	public float recoilYawMin;

	// Token: 0x04002B08 RID: 11016
	public float recoilYawMax;

	// Token: 0x04002B09 RID: 11017
	public float recoilPitchMin;

	// Token: 0x04002B0A RID: 11018
	public float recoilPitchMax;

	// Token: 0x04002B0B RID: 11019
	public float timeToTakeMin;

	// Token: 0x04002B0C RID: 11020
	public float timeToTakeMax = 0.1f;

	// Token: 0x04002B0D RID: 11021
	public float ADSScale = 0.5f;

	// Token: 0x04002B0E RID: 11022
	public float movementPenalty;

	// Token: 0x04002B0F RID: 11023
	public float clampPitch = float.NegativeInfinity;

	// Token: 0x04002B10 RID: 11024
	public AnimationCurve pitchCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002B11 RID: 11025
	public AnimationCurve yawCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002B12 RID: 11026
	public bool useCurves;

	// Token: 0x04002B13 RID: 11027
	public bool curvesAsScalar;

	// Token: 0x04002B14 RID: 11028
	public int shotsUntilMax = 30;

	// Token: 0x04002B15 RID: 11029
	public float maxRecoilRadius = 5f;

	// Token: 0x04002B16 RID: 11030
	[Header("AimCone")]
	public bool overrideAimconeWithCurve;

	// Token: 0x04002B17 RID: 11031
	public float aimconeCurveScale = 1f;

	// Token: 0x04002B18 RID: 11032
	[Tooltip("How much to scale aimcone by based on how far into the shot sequence we are (shots v shotsUntilMax)")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002B19 RID: 11033
	[Tooltip("Randomly select how much to scale final aimcone by per shot, you can use this to weigh a fraction of shots closer to the center")]
	public AnimationCurve aimconeProbabilityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(0.5f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002B1A RID: 11034
	public RecoilProperties newRecoilOverride;
}
