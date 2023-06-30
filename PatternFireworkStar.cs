using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class PatternFireworkStar : MonoBehaviour, IClientComponent
{
	// Token: 0x06000071 RID: 113 RVA: 0x00003DF4 File Offset: 0x00001FF4
	public void Initialize(Color color)
	{
		if (this.Pixel != null)
		{
			this.Pixel.SetActive(true);
		}
		if (this.Explosion != null)
		{
			this.Explosion.SetActive(false);
		}
		if (this.ParticleSystems != null)
		{
			foreach (ParticleSystem particleSystem in this.ParticleSystems)
			{
				if (!(particleSystem == null))
				{
					particleSystem.main.startColor = new ParticleSystem.MinMaxGradient(color);
				}
			}
		}
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00003E73 File Offset: 0x00002073
	public void Explode()
	{
		if (this.Pixel != null)
		{
			this.Pixel.SetActive(false);
		}
		if (this.Explosion != null)
		{
			this.Explosion.SetActive(true);
		}
	}

	// Token: 0x04000066 RID: 102
	public GameObject Pixel;

	// Token: 0x04000067 RID: 103
	public GameObject Explosion;

	// Token: 0x04000068 RID: 104
	public ParticleSystem[] ParticleSystems;
}
