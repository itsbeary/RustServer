using System;
using UnityEngine;

// Token: 0x0200097E RID: 2430
public class ViewmodelAspectOffset : MonoBehaviour
{
	// Token: 0x04003477 RID: 13431
	public Vector3 OffsetAmount = Vector3.zero;

	// Token: 0x04003478 RID: 13432
	[Tooltip("What aspect ratio should we start moving the viewmodel? 16:9 = 1.7, 21:9 = 2.3")]
	public float aspectCutoff = 2f;
}
