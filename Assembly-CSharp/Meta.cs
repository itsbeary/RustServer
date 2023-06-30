using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000302 RID: 770
[ConsoleSystem.Factory("meta")]
public class Meta : ConsoleSystem
{
	// Token: 0x06001E9D RID: 7837 RVA: 0x000CFDE8 File Offset: 0x000CDFE8
	[ServerVar(Clientside = true, Help = "add <convar> <amount> - adds amount to convar")]
	public static void add(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		float @float = args.GetFloat(1, 0.1f);
		ConsoleSystem.Command command = Meta.Find(@string);
		if (command == null)
		{
			args.ReplyWith("Convar not found: " + (@string ?? "<null>"));
			return;
		}
		if (args.IsClientside && command.Replicated)
		{
			args.ReplyWith("Cannot set replicated convars from the client (use sv to do this)");
			return;
		}
		if (args.IsServerside && command.ServerAdmin && !args.IsAdmin)
		{
			args.ReplyWith("Permission denied");
			return;
		}
		float num;
		if (!float.TryParse(command.String, out num))
		{
			args.ReplyWith("Convar value cannot be parsed as a number");
			return;
		}
		command.Set(num + @float);
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000CFE98 File Offset: 0x000CE098
	[ClientVar(Help = "if_true <command> <condition> - runs a command if the condition is true")]
	public static void if_true(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, false);
		if (@bool)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, @string, new object[] { @bool });
		}
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000CFEDC File Offset: 0x000CE0DC
	[ClientVar(Help = "if_false <command> <condition> - runs a command if the condition is false")]
	public static void if_false(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, true);
		if (!@bool)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, @string, new object[] { @bool });
		}
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000CFF20 File Offset: 0x000CE120
	[ClientVar(Help = "reset_cycle <key> - resets a cycled bind to the beginning")]
	public static void reset_cycle(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		List<KeyCode> list;
		KeyCombos.TryParse(ref @string, out list);
		Facepunch.Input.Button button = Facepunch.Input.GetButton(@string);
		if (button == null)
		{
			args.ReplyWith("Button not found");
			return;
		}
		if (!button.Cycle)
		{
			args.ReplyWith("Button does not have a cycled bind");
			return;
		}
		button.CycleIndex = 0;
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000CFF78 File Offset: 0x000CE178
	[ClientVar(Help = "exec [command_1] ... - runs all of the commands passed as arguments (also, if the last argument is true/false then that will flow into each command's arguments)")]
	public static void exec(ConsoleSystem.Arg args)
	{
		List<string> list = Pool.GetList<string>();
		for (int i = 0; i < 32; i++)
		{
			string @string = args.GetString(i, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				break;
			}
			list.Add(@string);
		}
		if (list.Count > 0)
		{
			string text = null;
			string text2 = list[list.Count - 1];
			bool flag;
			if (bool.TryParse(text2, out flag))
			{
				text = text2;
				list.RemoveAt(list.Count - 1);
			}
			foreach (string text3 in list)
			{
				if (text != null)
				{
					ConsoleSystem.Run(ConsoleSystem.Option.Client, text3, new object[] { text });
				}
				else
				{
					ConsoleSystem.Run(ConsoleSystem.Option.Client, text3, Array.Empty<object>());
				}
			}
		}
		Pool.FreeList<string>(ref list);
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x000D0060 File Offset: 0x000CE260
	private static ConsoleSystem.Command Find(string name)
	{
		ConsoleSystem.Command command = ConsoleSystem.Index.Server.Find(name);
		if (command != null)
		{
			return command;
		}
		return null;
	}
}
