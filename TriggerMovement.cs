using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class TriggerMovement : TriggerBase, IClientComponent
{
	// Token: 0x04001784 RID: 6020
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x04001785 RID: 6021
	public BaseEntity.MovementModify movementModify;
}
