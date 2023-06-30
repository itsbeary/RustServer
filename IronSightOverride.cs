using System;
using UnityEngine;

// Token: 0x02000971 RID: 2417
public class IronSightOverride : MonoBehaviour
{
	// Token: 0x04003442 RID: 13378
	public IronsightAimPoint aimPoint;

	// Token: 0x04003443 RID: 13379
	public float fieldOfViewOffset = -20f;

	// Token: 0x04003444 RID: 13380
	public float zoomFactor = -1f;

	// Token: 0x04003445 RID: 13381
	[Tooltip("If set to 1, the FOV is set to what this override is set to. If set to 0.5 it's half way between the weapon iconsights default and this scope.")]
	public float fovBias = 0.5f;
}
