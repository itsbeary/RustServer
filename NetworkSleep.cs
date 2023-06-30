using System;
using UnityEngine;

// Token: 0x0200090D RID: 2317
public class NetworkSleep : MonoBehaviour
{
	// Token: 0x04003357 RID: 13143
	public static int totalBehavioursDisabled;

	// Token: 0x04003358 RID: 13144
	public static int totalCollidersDisabled;

	// Token: 0x04003359 RID: 13145
	public Behaviour[] behaviours;

	// Token: 0x0400335A RID: 13146
	public Collider[] colliders;

	// Token: 0x0400335B RID: 13147
	internal int BehavioursDisabled;

	// Token: 0x0400335C RID: 13148
	internal int CollidersDisabled;
}
