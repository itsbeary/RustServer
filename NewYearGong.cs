using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class NewYearGong : BaseCombatEntity
{
	// Token: 0x06000F4E RID: 3918 RVA: 0x00080B68 File Offset: 0x0007ED68
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NewYearGong.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x00080BA8 File Offset: 0x0007EDA8
	public override void Hurt(HitInfo info)
	{
		if (!info.damageTypes.IsMeleeType() && !info.damageTypes.Has(DamageType.Bullet) && !info.damageTypes.Has(DamageType.Arrow))
		{
			base.Hurt(info);
			return;
		}
		Vector3 vector = this.gongCentre.InverseTransformPoint(info.HitPositionWorld);
		vector.z = 0f;
		float num = Vector3.Distance(vector, Vector3.zero);
		if (num < this.gongRadius)
		{
			if (Time.time - this.lastSound > this.minTimeBetweenSounds)
			{
				this.lastSound = Time.time;
				base.ClientRPC<float>(null, "PlaySound", Mathf.Clamp01(num / this.gongRadius));
				return;
			}
		}
		else
		{
			base.Hurt(info);
		}
	}

	// Token: 0x04000A16 RID: 2582
	public SoundDefinition gongSound;

	// Token: 0x04000A17 RID: 2583
	public float minTimeBetweenSounds = 0.25f;

	// Token: 0x04000A18 RID: 2584
	public GameObject soundRoot;

	// Token: 0x04000A19 RID: 2585
	public Transform gongCentre;

	// Token: 0x04000A1A RID: 2586
	public float gongRadius = 1f;

	// Token: 0x04000A1B RID: 2587
	public AnimationCurve pitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000A1C RID: 2588
	public Animator gongAnimator;

	// Token: 0x04000A1D RID: 2589
	private float lastSound;
}
