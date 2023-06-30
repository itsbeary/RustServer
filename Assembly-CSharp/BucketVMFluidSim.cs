using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class BucketVMFluidSim : MonoBehaviour
{
	// Token: 0x04001836 RID: 6198
	public Animator waterbucketAnim;

	// Token: 0x04001837 RID: 6199
	public ParticleSystem waterPour;

	// Token: 0x04001838 RID: 6200
	public ParticleSystem waterTurbulence;

	// Token: 0x04001839 RID: 6201
	public ParticleSystem waterFill;

	// Token: 0x0400183A RID: 6202
	public float waterLevel;

	// Token: 0x0400183B RID: 6203
	public float targetWaterLevel;

	// Token: 0x0400183C RID: 6204
	public AudioSource waterSpill;

	// Token: 0x0400183D RID: 6205
	private float PlayerEyePitch;

	// Token: 0x0400183E RID: 6206
	private float turb_forward;

	// Token: 0x0400183F RID: 6207
	private float turb_side;

	// Token: 0x04001840 RID: 6208
	private Vector3 lastPosition;

	// Token: 0x04001841 RID: 6209
	protected Vector3 groundSpeedLast;

	// Token: 0x04001842 RID: 6210
	private Vector3 lastAngle;

	// Token: 0x04001843 RID: 6211
	protected Vector3 vecAngleSpeedLast;

	// Token: 0x04001844 RID: 6212
	private Vector3 initialPosition;
}
