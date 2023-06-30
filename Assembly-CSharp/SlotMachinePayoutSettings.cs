using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
[CreateAssetMenu(menuName = "Rust/Slot Machine Payouts")]
public class SlotMachinePayoutSettings : ScriptableObject
{
	// Token: 0x04000F4D RID: 3917
	public ItemAmount SpinCost;

	// Token: 0x04000F4E RID: 3918
	public SlotMachinePayoutSettings.PayoutInfo[] Payouts;

	// Token: 0x04000F4F RID: 3919
	public int[] VirtualFaces = new int[16];

	// Token: 0x04000F50 RID: 3920
	public SlotMachinePayoutSettings.IndividualPayouts[] FacePayouts = new SlotMachinePayoutSettings.IndividualPayouts[0];

	// Token: 0x04000F51 RID: 3921
	public int TotalStops;

	// Token: 0x04000F52 RID: 3922
	public GameObjectRef DefaultWinEffect;

	// Token: 0x02000C38 RID: 3128
	[Serializable]
	public struct PayoutInfo
	{
		// Token: 0x040042FE RID: 17150
		public ItemAmount Item;

		// Token: 0x040042FF RID: 17151
		[Range(0f, 15f)]
		public int Result1;

		// Token: 0x04004300 RID: 17152
		[Range(0f, 15f)]
		public int Result2;

		// Token: 0x04004301 RID: 17153
		[Range(0f, 15f)]
		public int Result3;

		// Token: 0x04004302 RID: 17154
		public GameObjectRef OverrideWinEffect;
	}

	// Token: 0x02000C39 RID: 3129
	[Serializable]
	public struct IndividualPayouts
	{
		// Token: 0x04004303 RID: 17155
		public ItemAmount Item;

		// Token: 0x04004304 RID: 17156
		[Range(0f, 15f)]
		public int Result;
	}
}
