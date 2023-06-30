using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB4 RID: 2740
	[ConsoleSystem.Factory("console")]
	public class Console : ConsoleSystem
	{
		// Token: 0x06004191 RID: 16785 RVA: 0x001845F4 File Offset: 0x001827F4
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Output.Entry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Output.HistoryOutput.Count - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Output.HistoryOutput.Skip(num);
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x0018462C File Offset: 0x0018282C
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Output.Entry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Output.Entry>();
			}
			return Output.HistoryOutput.Where((Output.Entry x) => x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase));
		}
	}
}
