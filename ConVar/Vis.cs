using System;

namespace ConVar
{
	// Token: 0x02000AEE RID: 2798
	[ConsoleSystem.Factory("vis")]
	public class Vis : ConsoleSystem
	{
		// Token: 0x04003CA4 RID: 15524
		[ClientVar]
		[Help("Turns on debug display of lerp")]
		public static bool lerp;

		// Token: 0x04003CA5 RID: 15525
		[ServerVar]
		[Help("Turns on debug display of damages")]
		public static bool damage;

		// Token: 0x04003CA6 RID: 15526
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of attacks")]
		public static bool attack;

		// Token: 0x04003CA7 RID: 15527
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of protection")]
		public static bool protection;

		// Token: 0x04003CA8 RID: 15528
		[ServerVar]
		[Help("Turns on debug display of weakspots")]
		public static bool weakspots;

		// Token: 0x04003CA9 RID: 15529
		[ServerVar]
		[Help("Show trigger entries")]
		public static bool triggers;

		// Token: 0x04003CAA RID: 15530
		[ServerVar]
		[Help("Turns on debug display of hitboxes")]
		public static bool hitboxes;

		// Token: 0x04003CAB RID: 15531
		[ServerVar]
		[Help("Turns on debug display of line of sight checks")]
		public static bool lineofsight;

		// Token: 0x04003CAC RID: 15532
		[ServerVar]
		[Help("Turns on debug display of senses, which are received by Ai")]
		public static bool sense;
	}
}
