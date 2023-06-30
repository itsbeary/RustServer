using System;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	// Token: 0x02000AC5 RID: 2757
	[ConsoleSystem.Factory("gc")]
	public class GC : ConsoleSystem
	{
		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x060041F3 RID: 16883 RVA: 0x001867FC File Offset: 0x001849FC
		// (set) Token: 0x060041F4 RID: 16884 RVA: 0x00186803 File Offset: 0x00184A03
		[ClientVar]
		public static int buffer
		{
			get
			{
				return ConVar.GC.m_buffer;
			}
			set
			{
				ConVar.GC.m_buffer = Mathf.Clamp(value, 64, 4096);
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x060041F5 RID: 16885 RVA: 0x00186817 File Offset: 0x00184A17
		// (set) Token: 0x060041F6 RID: 16886 RVA: 0x0018681E File Offset: 0x00184A1E
		[ServerVar]
		[ClientVar]
		public static bool incremental_enabled
		{
			get
			{
				return GarbageCollector.isIncremental;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.incremental as it is read only");
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x060041F7 RID: 16887 RVA: 0x0018682A File Offset: 0x00184A2A
		// (set) Token: 0x060041F8 RID: 16888 RVA: 0x00186839 File Offset: 0x00184A39
		[ServerVar]
		[ClientVar]
		public static int incremental_milliseconds
		{
			get
			{
				return (int)(GarbageCollector.incrementalTimeSliceNanoseconds / 1000000UL);
			}
			set
			{
				GarbageCollector.incrementalTimeSliceNanoseconds = (ulong)(1000000L * (long)Mathf.Max(value, 0));
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x060041F9 RID: 16889 RVA: 0x0018684F File Offset: 0x00184A4F
		// (set) Token: 0x060041FA RID: 16890 RVA: 0x00186856 File Offset: 0x00184A56
		[ServerVar]
		[ClientVar]
		public static bool enabled
		{
			get
			{
				return Rust.GC.Enabled;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.enabled as it is read only");
			}
		}

		// Token: 0x060041FB RID: 16891 RVA: 0x00186862 File Offset: 0x00184A62
		[ServerVar]
		[ClientVar]
		public static void collect()
		{
			Rust.GC.Collect();
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x00186869 File Offset: 0x00184A69
		[ServerVar]
		[ClientVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}

		// Token: 0x060041FD RID: 16893 RVA: 0x00186874 File Offset: 0x00184A74
		[ServerVar]
		[ClientVar]
		public static void alloc(ConsoleSystem.Arg args)
		{
			byte[] array = new byte[args.GetInt(0, 1048576)];
			args.ReplyWith("Allocated " + array.Length + " bytes");
		}

		// Token: 0x04003BA8 RID: 15272
		[ClientVar]
		public static bool buffer_enabled = true;

		// Token: 0x04003BA9 RID: 15273
		[ClientVar]
		public static int debuglevel = 1;

		// Token: 0x04003BAA RID: 15274
		private static int m_buffer = 256;
	}
}
