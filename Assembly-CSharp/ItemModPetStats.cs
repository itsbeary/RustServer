using System;
using UnityEngine;

// Token: 0x020005FC RID: 1532
public class ItemModPetStats : ItemMod
{
	// Token: 0x06002DC2 RID: 11714 RVA: 0x001138E8 File Offset: 0x00111AE8
	public void Apply(BasePet pet)
	{
		if (pet == null)
		{
			return;
		}
		pet.SetMaxHealth(pet.MaxHealth() + this.MaxHealthModifier);
		if (pet.Brain != null && pet.Brain.Navigator != null)
		{
			pet.Brain.Navigator.Speed += this.SpeedModifier;
		}
		pet.BaseAttackRate += this.AttackRateModifier;
		pet.BaseAttackDamge += this.AttackDamageModifier;
	}

	// Token: 0x0400256F RID: 9583
	[Tooltip("Speed modifier. Value, not percentage.")]
	public float SpeedModifier;

	// Token: 0x04002570 RID: 9584
	[Tooltip("HP amount to modify max health by. Value, not percentage.")]
	public float MaxHealthModifier;

	// Token: 0x04002571 RID: 9585
	[Tooltip("Damage amount to modify base attack damage by. Value, not percentage.")]
	public float AttackDamageModifier;

	// Token: 0x04002572 RID: 9586
	[Tooltip("Attack rate (seconds) to modify base attack rate by. Value, not percentage.")]
	public float AttackRateModifier;
}
