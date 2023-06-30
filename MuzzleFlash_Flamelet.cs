using System;
using UnityEngine;

// Token: 0x0200034B RID: 843
public class MuzzleFlash_Flamelet : MonoBehaviour
{
	// Token: 0x06001F5B RID: 8027 RVA: 0x000D47EC File Offset: 0x000D29EC
	private void OnEnable()
	{
		this.flameletParticle.shape.angle = (float)UnityEngine.Random.Range(6, 13);
		float num = UnityEngine.Random.Range(7f, 9f);
		this.flameletParticle.startSpeed = UnityEngine.Random.Range(2.5f, num);
		this.flameletParticle.startSize = UnityEngine.Random.Range(0.05f, num * 0.015f);
	}

	// Token: 0x04001889 RID: 6281
	public ParticleSystem flameletParticle;
}
