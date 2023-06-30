using System;
using Rust;
using UnityEngine;

// Token: 0x020005FD RID: 1533
public class ItemModProjectile : MonoBehaviour
{
	// Token: 0x06002DC4 RID: 11716 RVA: 0x00113976 File Offset: 0x00111B76
	public float GetRandomVelocity()
	{
		return this.projectileVelocity + UnityEngine.Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x00113991 File Offset: 0x00111B91
	public float GetSpreadScalar()
	{
		if (this.useCurve)
		{
			return this.spreadScalar.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		return 1f;
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x001139BC File Offset: 0x00111BBC
	public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
	{
		float num2;
		if (shotIndex != -1)
		{
			float num = 1f / (float)maxShots;
			num2 = (float)shotIndex * num;
		}
		else
		{
			num2 = UnityEngine.Random.Range(0f, 1f);
		}
		return this.spreadScalar.Evaluate(num2);
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x001139FF File Offset: 0x00111BFF
	public float GetAverageVelocity()
	{
		return this.projectileVelocity;
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x00113A07 File Offset: 0x00111C07
	public float GetMinVelocity()
	{
		return this.projectileVelocity - this.projectileVelocitySpread;
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x00113A16 File Offset: 0x00111C16
	public float GetMaxVelocity()
	{
		return this.projectileVelocity + this.projectileVelocitySpread;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x00113A25 File Offset: 0x00111C25
	public bool IsAmmo(AmmoTypes ammo)
	{
		return (this.ammoType & ammo) > (AmmoTypes)0;
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x00113A34 File Offset: 0x00111C34
	public virtual void ServerProjectileHit(HitInfo info)
	{
		if (this.mods == null)
		{
			return;
		}
		foreach (ItemModProjectileMod itemModProjectileMod in this.mods)
		{
			if (!(itemModProjectileMod == null))
			{
				itemModProjectileMod.ServerProjectileHit(info);
			}
		}
	}

	// Token: 0x04002573 RID: 9587
	public GameObjectRef projectileObject = new GameObjectRef();

	// Token: 0x04002574 RID: 9588
	public ItemModProjectileMod[] mods;

	// Token: 0x04002575 RID: 9589
	public AmmoTypes ammoType;

	// Token: 0x04002576 RID: 9590
	public int numProjectiles = 1;

	// Token: 0x04002577 RID: 9591
	public float projectileSpread;

	// Token: 0x04002578 RID: 9592
	public float projectileVelocity = 100f;

	// Token: 0x04002579 RID: 9593
	public float projectileVelocitySpread;

	// Token: 0x0400257A RID: 9594
	public bool useCurve;

	// Token: 0x0400257B RID: 9595
	public AnimationCurve spreadScalar;

	// Token: 0x0400257C RID: 9596
	public GameObjectRef attackEffectOverride;

	// Token: 0x0400257D RID: 9597
	public float barrelConditionLoss;

	// Token: 0x0400257E RID: 9598
	public string category = "bullet";
}
