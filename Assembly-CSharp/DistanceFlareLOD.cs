using System;

// Token: 0x020008FF RID: 2303
public class DistanceFlareLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x0400332E RID: 13102
	public bool isDynamic;

	// Token: 0x0400332F RID: 13103
	public float minEnabledDistance = 100f;

	// Token: 0x04003330 RID: 13104
	public float maxEnabledDistance = 600f;

	// Token: 0x04003331 RID: 13105
	public bool toggleFade;

	// Token: 0x04003332 RID: 13106
	public float toggleFadeDuration = 0.5f;
}
