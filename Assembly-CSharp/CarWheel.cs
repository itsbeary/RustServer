using System;
using UnityEngine;

// Token: 0x0200047C RID: 1148
[Serializable]
public class CarWheel
{
	// Token: 0x04001E4A RID: 7754
	public WheelCollider wheelCollider;

	// Token: 0x04001E4B RID: 7755
	[Range(0.1f, 3f)]
	public float tyreFriction = 1f;

	// Token: 0x04001E4C RID: 7756
	public bool steerWheel;

	// Token: 0x04001E4D RID: 7757
	public bool brakeWheel = true;

	// Token: 0x04001E4E RID: 7758
	public bool powerWheel = true;
}
