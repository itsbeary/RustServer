using System;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;
using Windows;

// Token: 0x02000316 RID: 790
public class ServerConsole : SingletonComponent<ServerConsole>
{
	// Token: 0x06001EF5 RID: 7925 RVA: 0x000D2808 File Offset: 0x000D0A08
	public void OnEnable()
	{
		this.console.Initialize();
		this.input.OnInputText += this.OnInputText;
		Output.OnMessage += this.HandleLog;
		this.input.ClearLine(System.Console.WindowHeight);
		for (int i = 0; i < System.Console.WindowHeight; i++)
		{
			System.Console.WriteLine("");
		}
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000D2872 File Offset: 0x000D0A72
	private void OnDisable()
	{
		Output.OnMessage -= this.HandleLog;
		this.input.OnInputText -= this.OnInputText;
		this.console.Shutdown();
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000D28A7 File Offset: 0x000D0AA7
	private void OnInputText(string obj)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Server, obj, Array.Empty<object>());
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x000D28BC File Offset: 0x000D0ABC
	public static void PrintColoured(params object[] objects)
	{
		if (SingletonComponent<ServerConsole>.Instance == null)
		{
			return;
		}
		SingletonComponent<ServerConsole>.Instance.input.ClearLine(SingletonComponent<ServerConsole>.Instance.input.statusText.Length);
		for (int i = 0; i < objects.Length; i++)
		{
			if (i % 2 == 0)
			{
				System.Console.ForegroundColor = (ConsoleColor)objects[i];
			}
			else
			{
				System.Console.Write((string)objects[i]);
			}
		}
		if (System.Console.CursorLeft != 0)
		{
			System.Console.CursorTop++;
		}
		SingletonComponent<ServerConsole>.Instance.input.RedrawInputLine();
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000D2948 File Offset: 0x000D0B48
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (message.StartsWith("[CHAT]") || message.StartsWith("[TEAM CHAT]") || message.StartsWith("[CARDS CHAT]"))
		{
			return;
		}
		if (type == LogType.Warning)
		{
			if (message.StartsWith("HDR RenderTexture format is not") || message.StartsWith("The image effect") || message.StartsWith("Image Effects are not supported on this platform") || message.StartsWith("[AmplifyColor]") || message.StartsWith("Skipping profile frame.") || message.StartsWith("Kinematic body only supports Speculative Continuous collision detection"))
			{
				return;
			}
			System.Console.ForegroundColor = ConsoleColor.Yellow;
		}
		else if (type == LogType.Error)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Exception)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Assert)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else
		{
			System.Console.ForegroundColor = ConsoleColor.Gray;
		}
		this.input.ClearLine(this.input.statusText.Length);
		System.Console.WriteLine(message);
		this.input.RedrawInputLine();
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000D2A2E File Offset: 0x000D0C2E
	private void Update()
	{
		this.UpdateStatus();
		this.input.Update();
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000D2A44 File Offset: 0x000D0C44
	private void UpdateStatus()
	{
		if (this.nextUpdate > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (Network.Net.sv == null || !Network.Net.sv.IsConnected())
		{
			return;
		}
		this.nextUpdate = UnityEngine.Time.realtimeSinceStartup + 0.33f;
		if (!this.input.valid)
		{
			return;
		}
		string text = ((long)UnityEngine.Time.realtimeSinceStartup).FormatSeconds();
		string text2 = this.currentGameTime.ToString("[H:mm]");
		string text3 = string.Concat(new object[]
		{
			" ",
			text2,
			" [",
			this.currentPlayerCount,
			"/",
			this.maxPlayerCount,
			"] ",
			ConVar.Server.hostname,
			" [",
			ConVar.Server.level,
			"]"
		});
		string text4 = string.Concat(new object[]
		{
			global::Performance.current.frameRate,
			"fps ",
			global::Performance.current.memoryCollections,
			"gc ",
			text
		}) ?? "";
		string text5 = Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond).FormatBytes(true) + "/s in, " + Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond).FormatBytes(true) + "/s out";
		string text6 = text4.PadLeft(this.input.lineWidth - 1);
		text6 = text3 + ((text3.Length < text6.Length) ? text6.Substring(text3.Length) : "");
		string text7 = string.Concat(new string[]
		{
			" ",
			this.currentEntityCount.ToString("n0"),
			" ents, ",
			this.currentSleeperCount.ToString("n0"),
			" slprs"
		});
		string text8 = text5.PadLeft(this.input.lineWidth - 1);
		text8 = text7 + ((text7.Length < text8.Length) ? text8.Substring(text7.Length) : "");
		this.input.statusText[0] = "";
		this.input.statusText[1] = text6;
		this.input.statusText[2] = text8;
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06001EFC RID: 7932 RVA: 0x000D2CA7 File Offset: 0x000D0EA7
	private DateTime currentGameTime
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return DateTime.Now;
			}
			return TOD_Sky.Instance.Cycle.DateTime;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001EFD RID: 7933 RVA: 0x000D2CCA File Offset: 0x000D0ECA
	private int currentPlayerCount
	{
		get
		{
			return BasePlayer.activePlayerList.Count;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001EFE RID: 7934 RVA: 0x000D2CD6 File Offset: 0x000D0ED6
	private int maxPlayerCount
	{
		get
		{
			return ConVar.Server.maxplayers;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001EFF RID: 7935 RVA: 0x000D2CDD File Offset: 0x000D0EDD
	private int currentEntityCount
	{
		get
		{
			return BaseNetworkable.serverEntities.Count;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06001F00 RID: 7936 RVA: 0x000D2CE9 File Offset: 0x000D0EE9
	private int currentSleeperCount
	{
		get
		{
			return BasePlayer.sleepingPlayerList.Count;
		}
	}

	// Token: 0x040017E3 RID: 6115
	private ConsoleWindow console = new ConsoleWindow();

	// Token: 0x040017E4 RID: 6116
	private ConsoleInput input = new ConsoleInput();

	// Token: 0x040017E5 RID: 6117
	private float nextUpdate;
}
