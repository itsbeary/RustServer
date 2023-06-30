using System;
using UnityEngine;

// Token: 0x02000912 RID: 2322
public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06003807 RID: 14343 RVA: 0x0014DFD4 File Offset: 0x0014C1D4
	protected void OnEnable()
	{
		base.GetComponent<ParticleSystem>().enableEmission = true;
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x0014DFE2 File Offset: 0x0014C1E2
	public void OnParentDestroying()
	{
		base.GetComponent<ParticleSystem>().enableEmission = false;
	}
}
