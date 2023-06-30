using System;

namespace ConVar
{
	// Token: 0x02000AB5 RID: 2741
	[ConsoleSystem.Factory("construct")]
	public class Construct : ConsoleSystem
	{
		// Token: 0x04003B7D RID: 15229
		[ServerVar]
		[Help("How many minutes before a placed frame gets destroyed")]
		public static float frameminutes = 30f;
	}
}
