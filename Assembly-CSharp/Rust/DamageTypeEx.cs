using System;

namespace Rust
{
	// Token: 0x02000B11 RID: 2833
	public static class DamageTypeEx
	{
		// Token: 0x06004514 RID: 17684 RVA: 0x00194A1E File Offset: 0x00192C1E
		public static bool IsMeleeType(this DamageType damageType)
		{
			return damageType == DamageType.Blunt || damageType == DamageType.Slash || damageType == DamageType.Stab;
		}

		// Token: 0x06004515 RID: 17685 RVA: 0x00194A31 File Offset: 0x00192C31
		public static bool IsBleedCausing(this DamageType damageType)
		{
			return damageType == DamageType.Bite || damageType == DamageType.Slash || damageType == DamageType.Stab || damageType == DamageType.Bullet || damageType == DamageType.Arrow;
		}

		// Token: 0x06004516 RID: 17686 RVA: 0x00194A4E File Offset: 0x00192C4E
		public static bool IsConsideredAnAttack(this DamageType damageType)
		{
			return damageType != DamageType.Decay && damageType != DamageType.Collision;
		}
	}
}
