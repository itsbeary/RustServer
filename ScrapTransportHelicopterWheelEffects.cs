using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class ScrapTransportHelicopterWheelEffects : MonoBehaviour, IServerComponent
{
	// Token: 0x0600012F RID: 303 RVA: 0x00007E54 File Offset: 0x00006054
	public void Update()
	{
		bool isGrounded = this.wheelCollider.isGrounded;
		if (isGrounded && !this.wasGrounded)
		{
			this.DoImpactEffect();
		}
		this.wasGrounded = isGrounded;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00007E88 File Offset: 0x00006088
	private void DoImpactEffect()
	{
		if (!this.impactEffect.isValid)
		{
			return;
		}
		if (Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		Effect.server.Run(this.impactEffect.resourcePath, base.transform.position, base.transform.up, null, false);
		this.lastEffectPlayed = Time.time;
		this.lastCollisionPos = base.transform.position;
	}

	// Token: 0x04000162 RID: 354
	public WheelCollider wheelCollider;

	// Token: 0x04000163 RID: 355
	public GameObjectRef impactEffect;

	// Token: 0x04000164 RID: 356
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x04000165 RID: 357
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x04000166 RID: 358
	private bool wasGrounded;

	// Token: 0x04000167 RID: 359
	private float lastEffectPlayed;

	// Token: 0x04000168 RID: 360
	private Vector3 lastCollisionPos;
}
