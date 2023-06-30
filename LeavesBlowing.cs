using System;
using UnityEngine;

// Token: 0x02000612 RID: 1554
public class LeavesBlowing : MonoBehaviour
{
	// Token: 0x06002E17 RID: 11799 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x0011542C File Offset: 0x0011362C
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		if (this.m_psLeaves != null)
		{
			this.m_psLeaves.startSpeed = this.m_flSpeed;
			this.m_psLeaves.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psLeaves.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}

	// Token: 0x040025CB RID: 9675
	public ParticleSystem m_psLeaves;

	// Token: 0x040025CC RID: 9676
	public float m_flSwirl;

	// Token: 0x040025CD RID: 9677
	public float m_flSpeed;

	// Token: 0x040025CE RID: 9678
	public float m_flEmissionRate;
}
