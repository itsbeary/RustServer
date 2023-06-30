using System;

namespace ConVar
{
	// Token: 0x02000AE4 RID: 2788
	[ConsoleSystem.Factory("SSS")]
	public class SSS : ConsoleSystem
	{
		// Token: 0x04003C8F RID: 15503
		[ClientVar(Saved = true)]
		public static bool enabled = true;

		// Token: 0x04003C90 RID: 15504
		[ClientVar(Saved = true)]
		public static int quality = 0;

		// Token: 0x04003C91 RID: 15505
		[ClientVar(Saved = true)]
		public static bool halfres = true;

		// Token: 0x04003C92 RID: 15506
		[ClientVar(Saved = true)]
		public static float scale = 1f;
	}
}
