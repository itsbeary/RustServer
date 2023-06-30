using System;
using System.Collections.Generic;

namespace Rust
{
	// Token: 0x02000B0F RID: 2831
	public class DamageTypeList
	{
		// Token: 0x06004505 RID: 17669 RVA: 0x00194854 File Offset: 0x00192A54
		public void Set(DamageType index, float amount)
		{
			this.types[(int)index] = amount;
		}

		// Token: 0x06004506 RID: 17670 RVA: 0x0019485F File Offset: 0x00192A5F
		public float Get(DamageType index)
		{
			return this.types[(int)index];
		}

		// Token: 0x06004507 RID: 17671 RVA: 0x00194869 File Offset: 0x00192A69
		public void Add(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) + amount);
		}

		// Token: 0x06004508 RID: 17672 RVA: 0x0019487B File Offset: 0x00192A7B
		public void Scale(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) * amount);
		}

		// Token: 0x06004509 RID: 17673 RVA: 0x0019488D File Offset: 0x00192A8D
		public bool Has(DamageType index)
		{
			return this.Get(index) > 0f;
		}

		// Token: 0x0600450A RID: 17674 RVA: 0x001948A0 File Offset: 0x00192AA0
		public float Total()
		{
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x0600450B RID: 17675 RVA: 0x001948E4 File Offset: 0x00192AE4
		public void Clear()
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.types[i] = 0f;
			}
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x00194914 File Offset: 0x00192B14
		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry damageTypeEntry in entries)
			{
				this.Add(damageTypeEntry.type, damageTypeEntry.amount);
			}
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x00194970 File Offset: 0x00192B70
		public void ScaleAll(float amount)
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.Scale((DamageType)i, amount);
			}
		}

		// Token: 0x0600450E RID: 17678 RVA: 0x00194998 File Offset: 0x00192B98
		public DamageType GetMajorityDamageType()
		{
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num3 = this.types[i];
				if (!float.IsNaN(num3) && !float.IsInfinity(num3) && num3 >= num2)
				{
					num = i;
					num2 = num3;
				}
			}
			return (DamageType)num;
		}

		// Token: 0x0600450F RID: 17679 RVA: 0x001949E2 File Offset: 0x00192BE2
		public bool IsMeleeType()
		{
			return this.GetMajorityDamageType().IsMeleeType();
		}

		// Token: 0x06004510 RID: 17680 RVA: 0x001949EF File Offset: 0x00192BEF
		public bool IsBleedCausing()
		{
			return this.GetMajorityDamageType().IsBleedCausing();
		}

		// Token: 0x06004511 RID: 17681 RVA: 0x001949FC File Offset: 0x00192BFC
		public bool IsConsideredAnAttack()
		{
			return this.GetMajorityDamageType().IsConsideredAnAttack();
		}

		// Token: 0x04003D8D RID: 15757
		public float[] types = new float[25];
	}
}
