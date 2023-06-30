using System;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class ReliableEventSender : StateMachineBehaviour
{
	// Token: 0x040015FA RID: 5626
	[Header("State Enter")]
	public string StateEnter;

	// Token: 0x040015FB RID: 5627
	[Header("Mid State")]
	public string MidStateEvent;

	// Token: 0x040015FC RID: 5628
	[Range(0f, 1f)]
	public float TargetEventTime;
}
