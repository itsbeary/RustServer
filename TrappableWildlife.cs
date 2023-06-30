using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200046C RID: 1132
[CreateAssetMenu(menuName = "Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
	// Token: 0x04001DA8 RID: 7592
	public GameObjectRef worldObject;

	// Token: 0x04001DA9 RID: 7593
	public ItemDefinition inventoryObject;

	// Token: 0x04001DAA RID: 7594
	public int minToCatch;

	// Token: 0x04001DAB RID: 7595
	public int maxToCatch;

	// Token: 0x04001DAC RID: 7596
	public List<TrappableWildlife.BaitType> baitTypes;

	// Token: 0x04001DAD RID: 7597
	public int caloriesForInterest = 20;

	// Token: 0x04001DAE RID: 7598
	public float successRate = 1f;

	// Token: 0x04001DAF RID: 7599
	public float xpScale = 1f;

	// Token: 0x02000D0C RID: 3340
	[Serializable]
	public class BaitType
	{
		// Token: 0x04004690 RID: 18064
		public float successRate = 1f;

		// Token: 0x04004691 RID: 18065
		public ItemDefinition bait;

		// Token: 0x04004692 RID: 18066
		public int minForInterest = 1;

		// Token: 0x04004693 RID: 18067
		public int maxToConsume = 1;
	}
}
