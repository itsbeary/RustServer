using System;
using Sonar;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class SubmarineDuo : BaseSubmarine
{
	// Token: 0x04001FC6 RID: 8134
	[Header("Duo Sub Seating & Controls")]
	[SerializeField]
	private Transform steeringWheel;

	// Token: 0x04001FC7 RID: 8135
	[SerializeField]
	private Transform steeringWheelLeftGrip;

	// Token: 0x04001FC8 RID: 8136
	[SerializeField]
	private Transform steeringWheelRightGrip;

	// Token: 0x04001FC9 RID: 8137
	[SerializeField]
	private Transform leftPedal;

	// Token: 0x04001FCA RID: 8138
	[SerializeField]
	private Transform rightPedal;

	// Token: 0x04001FCB RID: 8139
	[SerializeField]
	private Transform driverLeftFoot;

	// Token: 0x04001FCC RID: 8140
	[SerializeField]
	private Transform driverRightFoot;

	// Token: 0x04001FCD RID: 8141
	[SerializeField]
	private Transform mphNeedle;

	// Token: 0x04001FCE RID: 8142
	[SerializeField]
	private Transform fuelNeedle;

	// Token: 0x04001FCF RID: 8143
	[SerializeField]
	private Transform waterDepthNeedle;

	// Token: 0x04001FD0 RID: 8144
	[SerializeField]
	private Transform ammoFlag;

	// Token: 0x04001FD1 RID: 8145
	[SerializeField]
	private SonarSystem sonar;

	// Token: 0x04001FD2 RID: 8146
	[SerializeField]
	private Transform torpedoTubeHatch;
}
