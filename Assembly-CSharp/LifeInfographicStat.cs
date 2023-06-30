using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000867 RID: 2151
public class LifeInfographicStat : MonoBehaviour
{
	// Token: 0x04003092 RID: 12434
	public LifeInfographicStat.DataType dataSource;

	// Token: 0x04003093 RID: 12435
	[Header("Generic Stats")]
	public string genericStatKey;

	// Token: 0x04003094 RID: 12436
	[Header("Weapon Info")]
	public string targetWeaponName;

	// Token: 0x04003095 RID: 12437
	public LifeInfographicStat.WeaponInfoType weaponInfoType;

	// Token: 0x04003096 RID: 12438
	public TextMeshProUGUI targetText;

	// Token: 0x04003097 RID: 12439
	public Image StatImage;

	// Token: 0x02000E9F RID: 3743
	public enum DataType
	{
		// Token: 0x04004CA3 RID: 19619
		None,
		// Token: 0x04004CA4 RID: 19620
		AliveTime_Short,
		// Token: 0x04004CA5 RID: 19621
		SleepingTime_Short,
		// Token: 0x04004CA6 RID: 19622
		KillerName,
		// Token: 0x04004CA7 RID: 19623
		KillerWeapon,
		// Token: 0x04004CA8 RID: 19624
		AliveTime_Long,
		// Token: 0x04004CA9 RID: 19625
		KillerDistance,
		// Token: 0x04004CAA RID: 19626
		GenericStat,
		// Token: 0x04004CAB RID: 19627
		DistanceTravelledWalk,
		// Token: 0x04004CAC RID: 19628
		DistanceTravelledRun,
		// Token: 0x04004CAD RID: 19629
		DamageTaken,
		// Token: 0x04004CAE RID: 19630
		DamageHealed,
		// Token: 0x04004CAF RID: 19631
		WeaponInfo,
		// Token: 0x04004CB0 RID: 19632
		SecondsWilderness,
		// Token: 0x04004CB1 RID: 19633
		SecondsSwimming,
		// Token: 0x04004CB2 RID: 19634
		SecondsInBase,
		// Token: 0x04004CB3 RID: 19635
		SecondsInMonument,
		// Token: 0x04004CB4 RID: 19636
		SecondsFlying,
		// Token: 0x04004CB5 RID: 19637
		SecondsBoating,
		// Token: 0x04004CB6 RID: 19638
		PlayersKilled,
		// Token: 0x04004CB7 RID: 19639
		ScientistsKilled,
		// Token: 0x04004CB8 RID: 19640
		AnimalsKilled,
		// Token: 0x04004CB9 RID: 19641
		SecondsDriving
	}

	// Token: 0x02000EA0 RID: 3744
	public enum WeaponInfoType
	{
		// Token: 0x04004CBB RID: 19643
		TotalShots,
		// Token: 0x04004CBC RID: 19644
		ShotsHit,
		// Token: 0x04004CBD RID: 19645
		ShotsMissed,
		// Token: 0x04004CBE RID: 19646
		AccuracyPercentage
	}
}
