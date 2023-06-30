using System;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
[Serializable]
public class VisualCarWheel : CarWheel
{
	// Token: 0x04002077 RID: 8311
	public Transform visualWheel;

	// Token: 0x04002078 RID: 8312
	public Transform visualWheelSteering;

	// Token: 0x04002079 RID: 8313
	public bool visualPowerWheel = true;

	// Token: 0x0400207A RID: 8314
	public ParticleSystem snowFX;

	// Token: 0x0400207B RID: 8315
	public ParticleSystem sandFX;

	// Token: 0x0400207C RID: 8316
	public ParticleSystem dirtFX;

	// Token: 0x0400207D RID: 8317
	public ParticleSystem asphaltFX;

	// Token: 0x0400207E RID: 8318
	public ParticleSystem waterFX;

	// Token: 0x0400207F RID: 8319
	public ParticleSystem snowSpinFX;

	// Token: 0x04002080 RID: 8320
	public ParticleSystem sandSpinFX;

	// Token: 0x04002081 RID: 8321
	public ParticleSystem dirtSpinFX;

	// Token: 0x04002082 RID: 8322
	public ParticleSystem asphaltSpinFX;
}
