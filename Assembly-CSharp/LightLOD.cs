using System;
using UnityEngine;

// Token: 0x02000904 RID: 2308
public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	// Token: 0x060037DE RID: 14302 RVA: 0x000CC4D6 File Offset: 0x000CA6D6
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x04003337 RID: 13111
	public float DistanceBias;

	// Token: 0x04003338 RID: 13112
	public bool ToggleLight;

	// Token: 0x04003339 RID: 13113
	public bool ToggleShadows = true;
}
