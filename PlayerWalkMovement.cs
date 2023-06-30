using System;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class PlayerWalkMovement : BaseMovement
{
	// Token: 0x04001D3C RID: 7484
	public const float WaterLevelHead = 0.75f;

	// Token: 0x04001D3D RID: 7485
	public const float WaterLevelNeck = 0.65f;

	// Token: 0x04001D3E RID: 7486
	public PhysicMaterial zeroFrictionMaterial;

	// Token: 0x04001D3F RID: 7487
	public PhysicMaterial highFrictionMaterial;

	// Token: 0x04001D40 RID: 7488
	public float capsuleHeight = 1f;

	// Token: 0x04001D41 RID: 7489
	public float capsuleCenter = 1f;

	// Token: 0x04001D42 RID: 7490
	public float capsuleHeightDucked = 1f;

	// Token: 0x04001D43 RID: 7491
	public float capsuleCenterDucked = 1f;

	// Token: 0x04001D44 RID: 7492
	public float capsuleHeightCrawling = 0.5f;

	// Token: 0x04001D45 RID: 7493
	public float capsuleCenterCrawling = 0.5f;

	// Token: 0x04001D46 RID: 7494
	public float gravityTestRadius = 0.2f;

	// Token: 0x04001D47 RID: 7495
	public float gravityMultiplier = 2.5f;

	// Token: 0x04001D48 RID: 7496
	public float gravityMultiplierSwimming = 0.1f;

	// Token: 0x04001D49 RID: 7497
	public float maxAngleWalking = 50f;

	// Token: 0x04001D4A RID: 7498
	public float maxAngleClimbing = 60f;

	// Token: 0x04001D4B RID: 7499
	public float maxAngleSliding = 90f;

	// Token: 0x04001D4C RID: 7500
	public float maxStepHeight = 0.25f;
}
