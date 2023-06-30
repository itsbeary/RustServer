using System;

// Token: 0x020008F7 RID: 2295
public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x060037BD RID: 14269 RVA: 0x000CC4D6 File Offset: 0x000CA6D6
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x04003316 RID: 13078
	public bool isDynamic;

	// Token: 0x04003317 RID: 13079
	public float enabledRadius = 20f;

	// Token: 0x04003318 RID: 13080
	public bool toggleFade;

	// Token: 0x04003319 RID: 13081
	public float toggleFadeDuration = 0.5f;
}
