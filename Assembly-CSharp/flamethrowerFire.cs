using System;
using UnityEngine;

// Token: 0x02000988 RID: 2440
public class flamethrowerFire : MonoBehaviour
{
	// Token: 0x060039F9 RID: 14841 RVA: 0x00156B2C File Offset: 0x00154D2C
	public void PilotLightOn()
	{
		this.pilotLightFX.enableEmission = true;
		this.SetFlameStatus(false);
	}

	// Token: 0x060039FA RID: 14842 RVA: 0x00156B44 File Offset: 0x00154D44
	public void SetFlameStatus(bool status)
	{
		ParticleSystem[] array = this.flameFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = status;
		}
	}

	// Token: 0x060039FB RID: 14843 RVA: 0x00156B6F File Offset: 0x00154D6F
	public void ShutOff()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(false);
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x00156B84 File Offset: 0x00154D84
	public void FlameOn()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(true);
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x00156B9C File Offset: 0x00154D9C
	private void Start()
	{
		this.previousflameState = (this.flameState = flamethrowerState.OFF);
	}

	// Token: 0x060039FE RID: 14846 RVA: 0x00156BBC File Offset: 0x00154DBC
	private void Update()
	{
		if (this.previousflameState != this.flameState)
		{
			switch (this.flameState)
			{
			case flamethrowerState.OFF:
				this.ShutOff();
				break;
			case flamethrowerState.PILOT_LIGHT:
				this.PilotLightOn();
				break;
			case flamethrowerState.FLAME_ON:
				this.FlameOn();
				break;
			}
			this.previousflameState = this.flameState;
			this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
		}
	}

	// Token: 0x04003491 RID: 13457
	public ParticleSystem pilotLightFX;

	// Token: 0x04003492 RID: 13458
	public ParticleSystem[] flameFX;

	// Token: 0x04003493 RID: 13459
	public FlameJet jet;

	// Token: 0x04003494 RID: 13460
	public AudioSource oneShotSound;

	// Token: 0x04003495 RID: 13461
	public AudioSource loopSound;

	// Token: 0x04003496 RID: 13462
	public AudioClip pilotlightIdle;

	// Token: 0x04003497 RID: 13463
	public AudioClip flameLoop;

	// Token: 0x04003498 RID: 13464
	public AudioClip flameStart;

	// Token: 0x04003499 RID: 13465
	public flamethrowerState flameState;

	// Token: 0x0400349A RID: 13466
	private flamethrowerState previousflameState;
}
