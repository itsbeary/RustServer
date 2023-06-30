using System;
using UnityEngine;

// Token: 0x0200047B RID: 1147
[Serializable]
public class CarSettings
{
	// Token: 0x04001E37 RID: 7735
	[Header("Vehicle Setup")]
	[Range(0f, 1f)]
	public float rollingResistance = 0.05f;

	// Token: 0x04001E38 RID: 7736
	[Range(0f, 1f)]
	public float antiRoll;

	// Token: 0x04001E39 RID: 7737
	public bool canSleep = true;

	// Token: 0x04001E3A RID: 7738
	[Header("Wheels")]
	public bool tankSteering;

	// Token: 0x04001E3B RID: 7739
	[Range(0f, 50f)]
	public float maxSteerAngle = 35f;

	// Token: 0x04001E3C RID: 7740
	public bool steeringAssist = true;

	// Token: 0x04001E3D RID: 7741
	[Range(0f, 1f)]
	public float steeringAssistRatio = 0.5f;

	// Token: 0x04001E3E RID: 7742
	public bool steeringLimit;

	// Token: 0x04001E3F RID: 7743
	[Range(0f, 50f)]
	public float minSteerLimitAngle = 6f;

	// Token: 0x04001E40 RID: 7744
	[Range(10f, 50f)]
	public float minSteerLimitSpeed = 30f;

	// Token: 0x04001E41 RID: 7745
	[Range(0f, 1f)]
	public float rearWheelSteer = 1f;

	// Token: 0x04001E42 RID: 7746
	public float steerMinLerpSpeed = 75f;

	// Token: 0x04001E43 RID: 7747
	public float steerMaxLerpSpeed = 150f;

	// Token: 0x04001E44 RID: 7748
	public float steerReturnLerpSpeed = 200f;

	// Token: 0x04001E45 RID: 7749
	[Header("Motor")]
	public float maxDriveSlip = 4f;

	// Token: 0x04001E46 RID: 7750
	public float driveForceToMaxSlip = 1000f;

	// Token: 0x04001E47 RID: 7751
	public float reversePercentSpeed = 0.3f;

	// Token: 0x04001E48 RID: 7752
	[Header("Brakes")]
	public float brakeForceMultiplier = 1000f;

	// Token: 0x04001E49 RID: 7753
	[Header("Front/Rear Vehicle Balance")]
	[Range(0f, 1f)]
	public float handlingBias = 0.5f;
}
