using System;

namespace ConVar
{
	// Token: 0x02000AF0 RID: 2800
	[ConsoleSystem.Factory("water")]
	public class Water : ConsoleSystem
	{
		// Token: 0x04003CAF RID: 15535
		[ClientVar(Saved = true)]
		public static int quality = 1;

		// Token: 0x04003CB0 RID: 15536
		public static int MaxQuality = 2;

		// Token: 0x04003CB1 RID: 15537
		public static int MinQuality = 0;

		// Token: 0x04003CB2 RID: 15538
		[ClientVar(Saved = true)]
		public static int reflections = 1;

		// Token: 0x04003CB3 RID: 15539
		public static int MaxReflections = 2;

		// Token: 0x04003CB4 RID: 15540
		public static int MinReflections = 0;

		// Token: 0x04003CB5 RID: 15541
		[ClientVar(ClientAdmin = true, Default = "0")]
		public static bool scaled_time = false;
	}
}
