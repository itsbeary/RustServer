using System;
using ConVar;
using UnityEngine;

// Token: 0x02000351 RID: 849
public class PhysicsEffects : MonoBehaviour
{
	// Token: 0x06001F64 RID: 8036 RVA: 0x000D4A22 File Offset: 0x000D2C22
	public void OnEnable()
	{
		this.enabledAt = UnityEngine.Time.time;
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000D4A30 File Offset: 0x000D2C30
	public void OnCollisionEnter(Collision collision)
	{
		if (!ConVar.Physics.sendeffects)
		{
			return;
		}
		if (UnityEngine.Time.time < this.enabledAt + this.enableDelay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if (((1 << collision.gameObject.layer) & this.ignoreLayers) != 0)
		{
			return;
		}
		float num = collision.relativeVelocity.magnitude;
		num = num * 0.055f * this.hardnessScale;
		if (num <= this.ignoreImpactThreshold)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, num.ToString(), null);
		}
		this.lastEffectPlayed = UnityEngine.Time.time;
		this.lastCollisionPos = base.transform.position;
	}

	// Token: 0x0400189B RID: 6299
	public BaseEntity entity;

	// Token: 0x0400189C RID: 6300
	public SoundDefinition physImpactSoundDef;

	// Token: 0x0400189D RID: 6301
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x0400189E RID: 6302
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x0400189F RID: 6303
	public float hardnessScale = 1f;

	// Token: 0x040018A0 RID: 6304
	public float lowMedThreshold = 0.4f;

	// Token: 0x040018A1 RID: 6305
	public float medHardThreshold = 0.7f;

	// Token: 0x040018A2 RID: 6306
	public float enableDelay = 0.1f;

	// Token: 0x040018A3 RID: 6307
	public LayerMask ignoreLayers;

	// Token: 0x040018A4 RID: 6308
	private float lastEffectPlayed;

	// Token: 0x040018A5 RID: 6309
	private float enabledAt = float.PositiveInfinity;

	// Token: 0x040018A6 RID: 6310
	private float ignoreImpactThreshold = 0.02f;

	// Token: 0x040018A7 RID: 6311
	private Vector3 lastCollisionPos;
}
