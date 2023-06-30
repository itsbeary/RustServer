using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class FlybySound : MonoBehaviour, IClientComponent
{
	// Token: 0x0400141C RID: 5148
	public SoundDefinition flybySound;

	// Token: 0x0400141D RID: 5149
	public float flybySoundDistance = 7f;

	// Token: 0x0400141E RID: 5150
	public SoundDefinition closeFlybySound;

	// Token: 0x0400141F RID: 5151
	public float closeFlybyDistance = 3f;
}
