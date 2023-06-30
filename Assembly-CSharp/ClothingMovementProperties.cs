using System;
using UnityEngine;

// Token: 0x02000561 RID: 1377
[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
	// Token: 0x040022CB RID: 8907
	public float speedReduction;

	// Token: 0x040022CC RID: 8908
	[Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
	public float minSpeedReduction;

	// Token: 0x040022CD RID: 8909
	public float waterSpeedBonus;
}
