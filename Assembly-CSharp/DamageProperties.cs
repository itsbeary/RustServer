using System;
using UnityEngine;

// Token: 0x02000562 RID: 1378
[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	// Token: 0x06002A90 RID: 10896 RVA: 0x001036A8 File Offset: 0x001018A8
	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < this.bones.Length; i++)
		{
			DamageProperties.HitAreaProperty hitAreaProperty = this.bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		if (!this.fallback)
		{
			return 1f;
		}
		return this.fallback.GetMultiplier(area);
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x00103700 File Offset: 0x00101900
	public void ScaleDamage(HitInfo info)
	{
		HitArea boneArea = info.boneArea;
		if (boneArea == (HitArea)(-1) || boneArea == (HitArea)0)
		{
			return;
		}
		info.damageTypes.ScaleAll(this.GetMultiplier(boneArea));
	}

	// Token: 0x040022CE RID: 8910
	public DamageProperties fallback;

	// Token: 0x040022CF RID: 8911
	[Horizontal(1, 0)]
	public DamageProperties.HitAreaProperty[] bones;

	// Token: 0x02000D61 RID: 3425
	[Serializable]
	public class HitAreaProperty
	{
		// Token: 0x040047B8 RID: 18360
		public HitArea area = HitArea.Head;

		// Token: 0x040047B9 RID: 18361
		public float damage = 1f;
	}
}
