using System;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class LightPulse : MonoBehaviour, IClientComponent
{
	// Token: 0x04001881 RID: 6273
	public Light TargetLight;

	// Token: 0x04001882 RID: 6274
	public float PulseSpeed = 1f;

	// Token: 0x04001883 RID: 6275
	public float Lifetime = 3f;

	// Token: 0x04001884 RID: 6276
	public float MaxIntensity = 3f;

	// Token: 0x04001885 RID: 6277
	public float FadeOutSpeed = 2f;
}
