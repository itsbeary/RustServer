using System;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class ParticleRandomLifetime : MonoBehaviour
{
	// Token: 0x06001DF3 RID: 7667 RVA: 0x000CCEE0 File Offset: 0x000CB0E0
	public void Awake()
	{
		if (!this.mySystem)
		{
			return;
		}
		float num = UnityEngine.Random.Range(this.minScale, this.maxScale);
		this.mySystem.startLifetime = num;
	}

	// Token: 0x040016EF RID: 5871
	public ParticleSystem mySystem;

	// Token: 0x040016F0 RID: 5872
	public float minScale = 0.5f;

	// Token: 0x040016F1 RID: 5873
	public float maxScale = 1f;
}
