using System;

namespace ConVar
{
	// Token: 0x02000AE9 RID: 2793
	[ConsoleSystem.Factory("terrain")]
	public class Terrain : ConsoleSystem
	{
		// Token: 0x04003C9B RID: 15515
		[ClientVar(Saved = true)]
		public static float quality = 100f;
	}
}
