using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE8 RID: 2792
	[ConsoleSystem.Factory("system")]
	public static class SystemCommands
	{
		// Token: 0x06004329 RID: 17193 RVA: 0x0018CAC4 File Offset: 0x0018ACC4
		[ServerVar]
		[ClientVar]
		public static void cpu_affinity(ConsoleSystem.Arg arg)
		{
			long num = 0L;
			if (!arg.HasArgs(1))
			{
				arg.ReplyWith("Format is 'cpu_affinity {core,core1-core2,etc}'");
				return;
			}
			string[] array = arg.GetString(0, "").Split(new char[] { ',' });
			HashSet<int> hashSet = new HashSet<int>();
			foreach (string text in array)
			{
				int num2;
				if (int.TryParse(text, out num2))
				{
					hashSet.Add(num2);
				}
				else if (text.Contains('-'))
				{
					string[] array3 = text.Split(new char[] { '-' });
					int num3;
					int num4;
					if (array3.Length != 2)
					{
						arg.ReplyWith("Failed to parse section " + text + ", format should be '0-15'");
					}
					else if (!int.TryParse(array3[0], out num3) || !int.TryParse(array3[1], out num4))
					{
						arg.ReplyWith("Core range in section " + text + " are not valid numbers, format should be '0-15'");
					}
					else if (num3 > num4)
					{
						arg.ReplyWith("Core range in section " + text + " are not ordered from least to greatest, format should be '0-15'");
					}
					else
					{
						if (num4 - num3 > 64)
						{
							arg.ReplyWith("Core range in section " + text + " are too big of a range, must be <64");
							return;
						}
						for (int j = num3; j <= num4; j++)
						{
							hashSet.Add(j);
						}
					}
				}
			}
			if (hashSet.Any((int x) => x < 0 || x > 63))
			{
				arg.ReplyWith("Cores provided out of range! Must be in between 0 and 63");
				return;
			}
			for (int k = 0; k < 64; k++)
			{
				if (hashSet.Contains(k))
				{
					num |= 1L << k;
				}
			}
			if (num == 0L)
			{
				arg.ReplyWith("No cores provided (bitmask empty)! Format is 'cpu_affinity {core,core1-core2,etc}'");
				return;
			}
			try
			{
				WindowsAffinityShim.SetProcessAffinityMask(Process.GetCurrentProcess().Handle, new IntPtr(num));
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning(string.Format("Unable to set cpu affinity: {0}", ex));
				return;
			}
			arg.ReplyWith("Successfully changed cpu affinity");
		}

		// Token: 0x0600432A RID: 17194 RVA: 0x0018CCC4 File Offset: 0x0018AEC4
		[ServerVar]
		[ClientVar]
		public static void cpu_priority(ConsoleSystem.Arg arg)
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				arg.ReplyWith("OSX is not a supported platform");
				return;
			}
			string @string = arg.GetString(0, "");
			string text = @string.Replace("-", "").Replace("_", "");
			ProcessPriorityClass processPriorityClass;
			if (!(text == "belownormal"))
			{
				if (!(text == "normal"))
				{
					if (!(text == "abovenormal"))
					{
						if (!(text == "high"))
						{
							arg.ReplyWith("Unknown priority '" + @string + "', possible values: below_normal, normal, above_normal, high");
							return;
						}
						processPriorityClass = ProcessPriorityClass.High;
					}
					else
					{
						processPriorityClass = ProcessPriorityClass.AboveNormal;
					}
				}
				else
				{
					processPriorityClass = ProcessPriorityClass.Normal;
				}
			}
			else
			{
				processPriorityClass = ProcessPriorityClass.BelowNormal;
			}
			try
			{
				WindowsAffinityShim.SetPriorityClass(Process.GetCurrentProcess().Handle, (uint)processPriorityClass);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning(string.Format("Unable to set cpu priority: {0}", ex));
				return;
			}
			arg.ReplyWith("Successfully changed cpu priority to " + processPriorityClass.ToString());
		}
	}
}
