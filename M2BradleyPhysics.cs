using System;
using UnityEngine;

// Token: 0x0200041B RID: 1051
public class M2BradleyPhysics : MonoBehaviour
{
	// Token: 0x04001B83 RID: 7043
	private m2bradleyAnimator m2Animator;

	// Token: 0x04001B84 RID: 7044
	public WheelCollider[] Wheels;

	// Token: 0x04001B85 RID: 7045
	public WheelCollider[] TurningWheels;

	// Token: 0x04001B86 RID: 7046
	public Rigidbody mainRigidbody;

	// Token: 0x04001B87 RID: 7047
	public Transform[] waypoints;

	// Token: 0x04001B88 RID: 7048
	private Vector3 currentWaypoint;

	// Token: 0x04001B89 RID: 7049
	private Vector3 nextWaypoint;
}
