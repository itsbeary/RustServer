using System;
using UnityEngine;

// Token: 0x02000972 RID: 2418
public class IronSights : MonoBehaviour
{
	// Token: 0x04003446 RID: 13382
	public bool Enabled;

	// Token: 0x04003447 RID: 13383
	[Header("View Setup")]
	public IronsightAimPoint aimPoint;

	// Token: 0x04003448 RID: 13384
	public float fieldOfViewOffset = -20f;

	// Token: 0x04003449 RID: 13385
	public float zoomFactor = 1f;

	// Token: 0x0400344A RID: 13386
	[Header("Animation")]
	public float introSpeed = 1f;

	// Token: 0x0400344B RID: 13387
	public AnimationCurve introCurve = new AnimationCurve();

	// Token: 0x0400344C RID: 13388
	public float outroSpeed = 1f;

	// Token: 0x0400344D RID: 13389
	public AnimationCurve outroCurve = new AnimationCurve();

	// Token: 0x0400344E RID: 13390
	[Header("Sounds")]
	public SoundDefinition upSound;

	// Token: 0x0400344F RID: 13391
	public SoundDefinition downSound;

	// Token: 0x04003450 RID: 13392
	[Header("Info")]
	public IronSightOverride ironsightsOverride;

	// Token: 0x04003451 RID: 13393
	public bool processUltrawideOffset;
}
