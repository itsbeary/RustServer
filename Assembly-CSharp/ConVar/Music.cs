using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AD6 RID: 2774
	[ConsoleSystem.Factory("music")]
	public class Music : ConsoleSystem
	{
		// Token: 0x0600428C RID: 17036 RVA: 0x00189B50 File Offset: 0x00187D50
		[ClientVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<MusicManager>.Instance == null)
			{
				stringBuilder.Append("No music manager was found");
			}
			else
			{
				stringBuilder.Append("Current music info: ");
				stringBuilder.AppendLine();
				stringBuilder.Append("  theme: " + SingletonComponent<MusicManager>.Instance.currentTheme);
				stringBuilder.AppendLine();
				stringBuilder.Append("  intensity: " + SingletonComponent<MusicManager>.Instance.intensity);
				stringBuilder.AppendLine();
				stringBuilder.Append("  next music: " + SingletonComponent<MusicManager>.Instance.nextMusic);
				stringBuilder.AppendLine();
				stringBuilder.Append("  current time: " + Time.time);
				stringBuilder.AppendLine();
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x04003BED RID: 15341
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003BEE RID: 15342
		[ClientVar]
		public static int songGapMin = 240;

		// Token: 0x04003BEF RID: 15343
		[ClientVar]
		public static int songGapMax = 480;
	}
}
