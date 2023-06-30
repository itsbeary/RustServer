using System;
using System.IO;
using Network;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ABD RID: 2749
	[ConsoleSystem.Factory("demo")]
	public class Demo : ConsoleSystem
	{
		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x060041BB RID: 16827 RVA: 0x001855C6 File Offset: 0x001837C6
		// (set) Token: 0x060041BC RID: 16828 RVA: 0x001855CD File Offset: 0x001837CD
		[ServerVar(Saved = true, Help = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist")]
		public static int recordlistmode
		{
			get
			{
				return Demo._recordListModeValue;
			}
			set
			{
				Demo._recordListModeValue = Mathf.Clamp(value, 0, 1);
			}
		}

		// Token: 0x060041BD RID: 16829 RVA: 0x001855DC File Offset: 0x001837DC
		[ServerVar]
		public static string record(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (playerOrSleeper.net.connection.IsRecording)
			{
				return "Player already recording a demo";
			}
			playerOrSleeper.StartDemoRecording();
			return null;
		}

		// Token: 0x060041BE RID: 16830 RVA: 0x00185634 File Offset: 0x00183834
		[ServerVar]
		public static string stop(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (!playerOrSleeper.net.connection.IsRecording)
			{
				return "Player not recording a demo";
			}
			playerOrSleeper.StopDemoRecording();
			return null;
		}

		// Token: 0x04003BA1 RID: 15265
		public static uint Version = 3U;

		// Token: 0x04003BA2 RID: 15266
		[ServerVar]
		public static float splitseconds = 3600f;

		// Token: 0x04003BA3 RID: 15267
		[ServerVar]
		public static float splitmegabytes = 200f;

		// Token: 0x04003BA4 RID: 15268
		[ServerVar(Saved = true)]
		public static string recordlist = "";

		// Token: 0x04003BA5 RID: 15269
		private static int _recordListModeValue = 0;

		// Token: 0x02000F64 RID: 3940
		public class Header : DemoHeader, IDemoHeader
		{
			// Token: 0x17000738 RID: 1848
			// (get) Token: 0x060054AB RID: 21675 RVA: 0x001B5DD7 File Offset: 0x001B3FD7
			// (set) Token: 0x060054AC RID: 21676 RVA: 0x001B5DDF File Offset: 0x001B3FDF
			long IDemoHeader.Length
			{
				get
				{
					return this.length;
				}
				set
				{
					this.length = value;
				}
			}

			// Token: 0x060054AD RID: 21677 RVA: 0x001B5DE8 File Offset: 0x001B3FE8
			public void Write(BinaryWriter writer)
			{
				byte[] array = base.ToProtoBytes();
				writer.Write("RUST DEMO FORMAT");
				writer.Write(array.Length);
				writer.Write(array);
				writer.Write('\0');
			}
		}
	}
}
