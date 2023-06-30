using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar
{
	// Token: 0x02000AD4 RID: 2772
	[ConsoleSystem.Factory("memsnap")]
	public class MemSnap : ConsoleSystem
	{
		// Token: 0x06004286 RID: 17030 RVA: 0x00189A68 File Offset: 0x00187C68
		private static string NeedProfileFolder()
		{
			string text = "profile";
			if (!Directory.Exists(text))
			{
				return Directory.CreateDirectory(text).FullName;
			}
			return new DirectoryInfo(text).FullName;
		}

		// Token: 0x06004287 RID: 17031 RVA: 0x00189A9C File Offset: 0x00187C9C
		[ClientVar]
		[ServerVar]
		public static void managed(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects);
		}

		// Token: 0x06004288 RID: 17032 RVA: 0x00189AD8 File Offset: 0x00187CD8
		[ClientVar]
		[ServerVar]
		public static void native(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.NativeObjects);
		}

		// Token: 0x06004289 RID: 17033 RVA: 0x00189B14 File Offset: 0x00187D14
		[ClientVar]
		[ServerVar]
		public static void full(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces);
		}
	}
}
