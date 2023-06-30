using System;

namespace ConVar
{
	// Token: 0x02000AC2 RID: 2754
	[ConsoleSystem.Factory("file")]
	public class FileConVar : ConsoleSystem
	{
		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x060041E5 RID: 16869 RVA: 0x0018650F File Offset: 0x0018470F
		// (set) Token: 0x060041E6 RID: 16870 RVA: 0x00186516 File Offset: 0x00184716
		[ClientVar]
		public static bool debug
		{
			get
			{
				return FileSystem.LogDebug;
			}
			set
			{
				FileSystem.LogDebug = value;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x060041E7 RID: 16871 RVA: 0x0018651E File Offset: 0x0018471E
		// (set) Token: 0x060041E8 RID: 16872 RVA: 0x00186525 File Offset: 0x00184725
		[ClientVar]
		public static bool time
		{
			get
			{
				return FileSystem.LogTime;
			}
			set
			{
				FileSystem.LogTime = value;
			}
		}
	}
}
