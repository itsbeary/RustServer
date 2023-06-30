using System;
using UnityEngine;

// Token: 0x0200039E RID: 926
public class DiscoFloor : AudioVisualisationEntity
{
	// Token: 0x0400199D RID: 6557
	public float GradientDuration = 3f;

	// Token: 0x0400199E RID: 6558
	public float VolumeSensitivityMultiplier = 3f;

	// Token: 0x0400199F RID: 6559
	public float BaseSpeed;

	// Token: 0x040019A0 RID: 6560
	public Light[] LightSources;

	// Token: 0x040019A1 RID: 6561
	public DiscoFloorMesh FloorMesh;
}
