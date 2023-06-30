using System;
using UnityEngine;

// Token: 0x020002CC RID: 716
public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x0400169F RID: 5791
	public Vector3 centerPosition;

	// Token: 0x040016A0 RID: 5792
	public bool worldSpace;

	// Token: 0x040016A1 RID: 5793
	public float scaleX = 1f;

	// Token: 0x040016A2 RID: 5794
	public float timeScaleX = 1f;

	// Token: 0x040016A3 RID: 5795
	public AnimationCurve movementX = new AnimationCurve();

	// Token: 0x040016A4 RID: 5796
	public float scaleY = 1f;

	// Token: 0x040016A5 RID: 5797
	public float timeScaleY = 1f;

	// Token: 0x040016A6 RID: 5798
	public AnimationCurve movementY = new AnimationCurve();

	// Token: 0x040016A7 RID: 5799
	public float scaleZ = 1f;

	// Token: 0x040016A8 RID: 5800
	public float timeScaleZ = 1f;

	// Token: 0x040016A9 RID: 5801
	public AnimationCurve movementZ = new AnimationCurve();
}
