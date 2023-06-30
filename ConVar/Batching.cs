using System;

namespace ConVar
{
	// Token: 0x02000AB0 RID: 2736
	[ConsoleSystem.Factory("batching")]
	public class Batching : ConsoleSystem
	{
		// Token: 0x04003B6C RID: 15212
		[ClientVar]
		public static bool renderers = true;

		// Token: 0x04003B6D RID: 15213
		[ClientVar]
		public static bool renderer_threading = true;

		// Token: 0x04003B6E RID: 15214
		[ClientVar]
		public static int renderer_capacity = 30000;

		// Token: 0x04003B6F RID: 15215
		[ClientVar]
		public static int renderer_vertices = 1000;

		// Token: 0x04003B70 RID: 15216
		[ClientVar]
		public static int renderer_submeshes = 1;

		// Token: 0x04003B71 RID: 15217
		[ServerVar]
		[ClientVar]
		public static int verbose = 0;
	}
}
