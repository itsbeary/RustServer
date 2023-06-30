using System;

namespace ConVar
{
	// Token: 0x02000ACA RID: 2762
	[ConsoleSystem.Factory("harmony")]
	public class Harmony : ConsoleSystem
	{
		// Token: 0x06004255 RID: 16981 RVA: 0x00187FA1 File Offset: 0x001861A1
		[ServerVar(Name = "load")]
		public static void Load(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryLoadMod(args.GetString(0, ""));
		}

		// Token: 0x06004256 RID: 16982 RVA: 0x00187FB5 File Offset: 0x001861B5
		[ServerVar(Name = "unload")]
		public static void Unload(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryUnloadMod(args.GetString(0, ""));
		}
	}
}
