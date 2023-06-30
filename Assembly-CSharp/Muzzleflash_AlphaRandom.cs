using System;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class Muzzleflash_AlphaRandom : MonoBehaviour
{
	// Token: 0x06001F5D RID: 8029 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000D4858 File Offset: 0x000D2A58
	private void OnEnable()
	{
		this.gck[0].color = Color.white;
		this.gck[0].time = 0f;
		this.gck[1].color = Color.white;
		this.gck[1].time = 0.6f;
		this.gck[2].color = Color.black;
		this.gck[2].time = 0.75f;
		float num = UnityEngine.Random.Range(0.2f, 0.85f);
		this.gak[0].alpha = num;
		this.gak[0].time = 0f;
		this.gak[1].alpha = num;
		this.gak[1].time = 0.45f;
		this.gak[2].alpha = 0f;
		this.gak[2].time = 0.5f;
		this.grad.SetKeys(this.gck, this.gak);
		foreach (ParticleSystem particleSystem in this.muzzleflashParticles)
		{
			if (particleSystem == null)
			{
				Debug.LogWarning("Muzzleflash_AlphaRandom : null particle system in " + base.gameObject.name);
			}
			else
			{
				particleSystem.colorOverLifetime.color = this.grad;
			}
		}
	}

	// Token: 0x0400188A RID: 6282
	public ParticleSystem[] muzzleflashParticles;

	// Token: 0x0400188B RID: 6283
	private Gradient grad = new Gradient();

	// Token: 0x0400188C RID: 6284
	private GradientColorKey[] gck = new GradientColorKey[3];

	// Token: 0x0400188D RID: 6285
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];
}
