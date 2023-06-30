using System;
using UnityEngine;

// Token: 0x02000353 RID: 851
public class Sandstorm : MonoBehaviour
{
	// Token: 0x06001F68 RID: 8040 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000D4B90 File Offset: 0x000D2D90
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = -7f + Mathf.Sin(Time.time * 2.5f) * 7f;
		base.transform.eulerAngles = eulerAngles;
		if (this.m_psSandStorm != null)
		{
			this.m_psSandStorm.startSpeed = this.m_flSpeed;
			this.m_psSandStorm.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psSandStorm.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}

	// Token: 0x040018AB RID: 6315
	public ParticleSystem m_psSandStorm;

	// Token: 0x040018AC RID: 6316
	public float m_flSpeed;

	// Token: 0x040018AD RID: 6317
	public float m_flSwirl;

	// Token: 0x040018AE RID: 6318
	public float m_flEmissionRate;
}
