using System;

namespace ConVar
{
	// Token: 0x02000AD7 RID: 2775
	[ConsoleSystem.Factory("net")]
	public class Net : ConsoleSystem
	{
		// Token: 0x04003BF0 RID: 15344
		[ServerVar]
		public static bool visdebug = false;

		// Token: 0x04003BF1 RID: 15345
		[ClientVar]
		public static bool debug = false;

		// Token: 0x04003BF2 RID: 15346
		[ServerVar]
		public static int visibilityRadiusFarOverride = -1;

		// Token: 0x04003BF3 RID: 15347
		[ServerVar]
		public static int visibilityRadiusNearOverride = -1;
	}
}
