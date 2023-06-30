using System;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class FlasherLight : IOEntity
{
	// Token: 0x060016AB RID: 5803 RVA: 0x00025634 File Offset: 0x00023834
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x04000EB8 RID: 3768
	public EmissionToggle toggler;

	// Token: 0x04000EB9 RID: 3769
	public Light myLight;

	// Token: 0x04000EBA RID: 3770
	public float flashSpacing = 0.2f;

	// Token: 0x04000EBB RID: 3771
	public float flashBurstSpacing = 0.5f;

	// Token: 0x04000EBC RID: 3772
	public float flashOnTime = 0.1f;

	// Token: 0x04000EBD RID: 3773
	public int numFlashesPerBurst = 5;
}
