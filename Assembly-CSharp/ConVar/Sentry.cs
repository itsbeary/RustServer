using System;

namespace ConVar
{
	// Token: 0x02000AE1 RID: 2785
	[ConsoleSystem.Factory("sentry")]
	public class Sentry : ConsoleSystem
	{
		// Token: 0x04003C03 RID: 15363
		[ServerVar(Help = "target everyone regardless of authorization")]
		public static bool targetall = false;

		// Token: 0x04003C04 RID: 15364
		[ServerVar(Help = "how long until something is considered hostile after it attacked")]
		public static float hostileduration = 120f;
	}
}
