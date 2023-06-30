using System;
using UnityEngine;

// Token: 0x020005AD RID: 1453
public class WearableHolsterOffset : MonoBehaviour
{
	// Token: 0x040023FB RID: 9211
	public WearableHolsterOffset.offsetInfo[] Offsets;

	// Token: 0x02000D84 RID: 3460
	[Serializable]
	public class offsetInfo
	{
		// Token: 0x0400484A RID: 18506
		public HeldEntity.HolsterInfo.HolsterSlot type;

		// Token: 0x0400484B RID: 18507
		public Vector3 offset;

		// Token: 0x0400484C RID: 18508
		public Vector3 rotationOffset;

		// Token: 0x0400484D RID: 18509
		public int priority;
	}
}
