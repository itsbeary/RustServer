using System;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class LocomotiveExtraVisuals : MonoBehaviour
{
	// Token: 0x04001FF9 RID: 8185
	[Header("Gauges")]
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04001FFA RID: 8186
	[SerializeField]
	private Transform needleA;

	// Token: 0x04001FFB RID: 8187
	[SerializeField]
	private Transform needleB;

	// Token: 0x04001FFC RID: 8188
	[SerializeField]
	private Transform needleC;

	// Token: 0x04001FFD RID: 8189
	[SerializeField]
	private float maxAngle = 240f;

	// Token: 0x04001FFE RID: 8190
	[SerializeField]
	private float speedoMoveSpeed = 75f;

	// Token: 0x04001FFF RID: 8191
	[SerializeField]
	private float pressureMoveSpeed = 25f;

	// Token: 0x04002000 RID: 8192
	[SerializeField]
	private float fanAcceleration = 50f;

	// Token: 0x04002001 RID: 8193
	[SerializeField]
	private float fanMaxSpeed = 1000f;

	// Token: 0x04002002 RID: 8194
	[SerializeField]
	private float speedoMax = 80f;

	// Token: 0x04002003 RID: 8195
	[Header("Fans")]
	[SerializeField]
	private Transform[] engineFans;
}
