using System;
using System.IO;

namespace ConVar
{
	// Token: 0x02000ADF RID: 2783
	[ConsoleSystem.Factory("profile")]
	public class Profile : ConsoleSystem
	{
		// Token: 0x060042C7 RID: 17095 RVA: 0x0018B2D6 File Offset: 0x001894D6
		private static void NeedProfileFolder()
		{
			if (!Directory.Exists("profile"))
			{
				Directory.CreateDirectory("profile");
			}
		}

		// Token: 0x060042C8 RID: 17096 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		[ServerVar]
		public static void start(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x060042C9 RID: 17097 RVA: 0x000063A5 File Offset: 0x000045A5
		[ServerVar]
		[ClientVar]
		public static void stop(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x060042CA RID: 17098 RVA: 0x000063A5 File Offset: 0x000045A5
		[ServerVar]
		[ClientVar]
		public static void flush_analytics(ConsoleSystem.Arg arg)
		{
		}
	}
}
