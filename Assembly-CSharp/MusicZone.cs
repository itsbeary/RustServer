using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class MusicZone : MonoBehaviour, IClientComponent
{
	// Token: 0x0400147F RID: 5247
	public List<MusicTheme> themes;

	// Token: 0x04001480 RID: 5248
	public float priority;

	// Token: 0x04001481 RID: 5249
	public bool suppressAutomaticMusic;
}
