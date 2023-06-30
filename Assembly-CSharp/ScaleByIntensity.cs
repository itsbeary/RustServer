using System;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class ScaleByIntensity : MonoBehaviour
{
	// Token: 0x06001E30 RID: 7728 RVA: 0x000CDEB9 File Offset: 0x000CC0B9
	private void Start()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x000CDECC File Offset: 0x000CC0CC
	private void Update()
	{
		base.transform.localScale = (this.intensitySource.enabled ? (this.initialScale * this.intensitySource.intensity / this.maxIntensity) : Vector3.zero);
	}

	// Token: 0x04001757 RID: 5975
	public Vector3 initialScale = Vector3.zero;

	// Token: 0x04001758 RID: 5976
	public Light intensitySource;

	// Token: 0x04001759 RID: 5977
	public float maxIntensity = 1f;
}
