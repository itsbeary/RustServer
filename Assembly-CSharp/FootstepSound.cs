using System;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class FootstepSound : MonoBehaviour, IClientComponent
{
	// Token: 0x04001420 RID: 5152
	public SoundDefinition lightSound;

	// Token: 0x04001421 RID: 5153
	public SoundDefinition medSound;

	// Token: 0x04001422 RID: 5154
	public SoundDefinition hardSound;

	// Token: 0x04001423 RID: 5155
	private const float panAmount = 0.05f;

	// Token: 0x02000C8E RID: 3214
	public enum Hardness
	{
		// Token: 0x0400442B RID: 17451
		Light = 1,
		// Token: 0x0400442C RID: 17452
		Medium,
		// Token: 0x0400442D RID: 17453
		Hard
	}
}
