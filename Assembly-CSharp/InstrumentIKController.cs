using System;
using UnityEngine;

// Token: 0x020003FF RID: 1023
public class InstrumentIKController : MonoBehaviour
{
	// Token: 0x04001AD1 RID: 6865
	public Vector3 HitRotationVector = Vector3.forward;

	// Token: 0x04001AD2 RID: 6866
	public Transform[] LeftHandIkTargets = new Transform[0];

	// Token: 0x04001AD3 RID: 6867
	public Transform[] LeftHandIKTargetHitRotations = new Transform[0];

	// Token: 0x04001AD4 RID: 6868
	public Transform[] RightHandIkTargets = new Transform[0];

	// Token: 0x04001AD5 RID: 6869
	public Transform[] RightHandIKTargetHitRotations = new Transform[0];

	// Token: 0x04001AD6 RID: 6870
	public Transform[] RightFootIkTargets = new Transform[0];

	// Token: 0x04001AD7 RID: 6871
	public AnimationCurve HandHeightCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04001AD8 RID: 6872
	public float HandHeightMultiplier = 1f;

	// Token: 0x04001AD9 RID: 6873
	public float HandMoveLerpSpeed = 50f;

	// Token: 0x04001ADA RID: 6874
	public bool DebugHitRotation;

	// Token: 0x04001ADB RID: 6875
	public AnimationCurve HandHitCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001ADC RID: 6876
	public float NoteHitTime = 0.5f;

	// Token: 0x04001ADD RID: 6877
	[Header("Look IK")]
	public float BodyLookWeight;

	// Token: 0x04001ADE RID: 6878
	public float HeadLookWeight;

	// Token: 0x04001ADF RID: 6879
	public float LookWeightLimit;

	// Token: 0x04001AE0 RID: 6880
	public bool HoldHandsAtPlay;
}
