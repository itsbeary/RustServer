using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class TriggerRadiation : TriggerBase
{
	// Token: 0x06002BE2 RID: 11234 RVA: 0x0010A01A File Offset: 0x0010821A
	private float GetRadiationSize()
	{
		if (!this.sphereCollider)
		{
			this.sphereCollider = base.GetComponent<SphereCollider>();
		}
		return this.sphereCollider.radius * base.transform.localScale.Max();
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x0010A054 File Offset: 0x00108254
	public float GetRadiationAmount()
	{
		if (this.RadiationAmountOverride > 0f)
		{
			return this.RadiationAmountOverride;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
		{
			return 10f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x0010A0B8 File Offset: 0x001082B8
	public float GetRadiation(Vector3 position, float radProtection)
	{
		float radiationSize = this.GetRadiationSize();
		float radiationAmount = this.GetRadiationAmount();
		float num = Vector3.Distance(base.gameObject.transform.position, position);
		float num2 = Mathf.InverseLerp(radiationSize, radiationSize * (1f - this.falloff), num);
		return Mathf.Clamp(radiationAmount - radProtection, 0f, radiationAmount) * num2;
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x0010A110 File Offset: 0x00108310
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x0010A160 File Offset: 0x00108360
	public void OnDrawGizmosSelected()
	{
		float radiationSize = this.GetRadiationSize();
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize * (1f - this.falloff));
	}

	// Token: 0x040023B3 RID: 9139
	public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;

	// Token: 0x040023B4 RID: 9140
	public float RadiationAmountOverride;

	// Token: 0x040023B5 RID: 9141
	public float falloff = 0.1f;

	// Token: 0x040023B6 RID: 9142
	private SphereCollider sphereCollider;

	// Token: 0x02000D7B RID: 3451
	public enum RadiationTier
	{
		// Token: 0x04004811 RID: 18449
		MINIMAL,
		// Token: 0x04004812 RID: 18450
		LOW,
		// Token: 0x04004813 RID: 18451
		MEDIUM,
		// Token: 0x04004814 RID: 18452
		HIGH
	}
}
