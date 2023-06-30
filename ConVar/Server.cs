using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Reports;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE2 RID: 2786
	[ConsoleSystem.Factory("server")]
	public class Server : ConsoleSystem
	{
		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x060042CF RID: 17103 RVA: 0x0018B301 File Offset: 0x00189501
		// (set) Token: 0x060042D0 RID: 17104 RVA: 0x0018B308 File Offset: 0x00189508
		[ServerVar]
		public static int anticheatlog
		{
			get
			{
				return (int)EOS.LogLevel;
			}
			set
			{
				EOS.LogLevel = (LogLevel)value;
			}
		}

		// Token: 0x060042D1 RID: 17105 RVA: 0x0018B310 File Offset: 0x00189510
		public static float TickDelta()
		{
			return 1f / (float)Server.tickrate;
		}

		// Token: 0x060042D2 RID: 17106 RVA: 0x0018B31E File Offset: 0x0018951E
		public static float TickTime(uint tick)
		{
			return (float)((double)Server.TickDelta() * tick);
		}

		// Token: 0x060042D3 RID: 17107 RVA: 0x0018B32C File Offset: 0x0018952C
		[ServerVar(Help = "Show holstered items on player bodies")]
		public static void setshowholstereditems(ConsoleSystem.Arg arg)
		{
			Server.showHolsteredItems = arg.GetBool(0, Server.showHolsteredItems);
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.inventory.UpdatedVisibleHolsteredItems();
			}
			foreach (BasePlayer basePlayer2 in BasePlayer.sleepingPlayerList)
			{
				basePlayer2.inventory.UpdatedVisibleHolsteredItems();
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060042D4 RID: 17108 RVA: 0x0018B3D4 File Offset: 0x001895D4
		// (set) Token: 0x060042D5 RID: 17109 RVA: 0x0018B3DB File Offset: 0x001895DB
		[ServerVar]
		public static int maxclientinfosize
		{
			get
			{
				return Connection.MaxClientInfoSize;
			}
			set
			{
				Connection.MaxClientInfoSize = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060042D6 RID: 17110 RVA: 0x0018B3E9 File Offset: 0x001895E9
		// (set) Token: 0x060042D7 RID: 17111 RVA: 0x0018B3F0 File Offset: 0x001895F0
		[ServerVar]
		public static int maxconnectionsperip
		{
			get
			{
				return Server.MaxConnectionsPerIP;
			}
			set
			{
				Server.MaxConnectionsPerIP = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060042D8 RID: 17112 RVA: 0x0018B403 File Offset: 0x00189603
		// (set) Token: 0x060042D9 RID: 17113 RVA: 0x0018B40A File Offset: 0x0018960A
		[ServerVar]
		public static int maxreceivetime
		{
			get
			{
				return Server.MaxReceiveTime;
			}
			set
			{
				Server.MaxReceiveTime = Mathf.Clamp(value, 10, 1000);
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060042DA RID: 17114 RVA: 0x0018B41E File Offset: 0x0018961E
		// (set) Token: 0x060042DB RID: 17115 RVA: 0x0018B425 File Offset: 0x00189625
		[ServerVar]
		public static int maxmainthreadwait
		{
			get
			{
				return Server.MaxMainThreadWait;
			}
			set
			{
				Server.MaxMainThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060042DC RID: 17116 RVA: 0x0018B438 File Offset: 0x00189638
		// (set) Token: 0x060042DD RID: 17117 RVA: 0x0018B43F File Offset: 0x0018963F
		[ServerVar]
		public static int maxreadthreadwait
		{
			get
			{
				return Server.MaxReadThreadWait;
			}
			set
			{
				Server.MaxReadThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060042DE RID: 17118 RVA: 0x0018B452 File Offset: 0x00189652
		// (set) Token: 0x060042DF RID: 17119 RVA: 0x0018B459 File Offset: 0x00189659
		[ServerVar]
		public static int maxwritethreadwait
		{
			get
			{
				return Server.MaxWriteThreadWait;
			}
			set
			{
				Server.MaxWriteThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060042E0 RID: 17120 RVA: 0x0018B46C File Offset: 0x0018966C
		// (set) Token: 0x060042E1 RID: 17121 RVA: 0x0018B473 File Offset: 0x00189673
		[ServerVar]
		public static int maxdecryptthreadwait
		{
			get
			{
				return Server.MaxDecryptThreadWait;
			}
			set
			{
				Server.MaxDecryptThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060042E2 RID: 17122 RVA: 0x0018B486 File Offset: 0x00189686
		// (set) Token: 0x060042E3 RID: 17123 RVA: 0x0018B48D File Offset: 0x0018968D
		[ServerVar]
		public static int maxreadqueuelength
		{
			get
			{
				return Server.MaxReadQueueLength;
			}
			set
			{
				Server.MaxReadQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060042E4 RID: 17124 RVA: 0x0018B49B File Offset: 0x0018969B
		// (set) Token: 0x060042E5 RID: 17125 RVA: 0x0018B4A2 File Offset: 0x001896A2
		[ServerVar]
		public static int maxwritequeuelength
		{
			get
			{
				return Server.MaxWriteQueueLength;
			}
			set
			{
				Server.MaxWriteQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060042E6 RID: 17126 RVA: 0x0018B4B0 File Offset: 0x001896B0
		// (set) Token: 0x060042E7 RID: 17127 RVA: 0x0018B4B7 File Offset: 0x001896B7
		[ServerVar]
		public static int maxdecryptqueuelength
		{
			get
			{
				return Server.MaxDecryptQueueLength;
			}
			set
			{
				Server.MaxDecryptQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060042E8 RID: 17128 RVA: 0x0018B4C5 File Offset: 0x001896C5
		// (set) Token: 0x060042E9 RID: 17129 RVA: 0x0018B4CC File Offset: 0x001896CC
		[ServerVar]
		public static int maxreadqueuebytes
		{
			get
			{
				return Server.MaxReadQueueBytes;
			}
			set
			{
				Server.MaxReadQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060042EA RID: 17130 RVA: 0x0018B4DA File Offset: 0x001896DA
		// (set) Token: 0x060042EB RID: 17131 RVA: 0x0018B4E1 File Offset: 0x001896E1
		[ServerVar]
		public static int maxwritequeuebytes
		{
			get
			{
				return Server.MaxWriteQueueBytes;
			}
			set
			{
				Server.MaxWriteQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060042EC RID: 17132 RVA: 0x0018B4EF File Offset: 0x001896EF
		// (set) Token: 0x060042ED RID: 17133 RVA: 0x0018B4F6 File Offset: 0x001896F6
		[ServerVar]
		public static int maxdecryptqueuebytes
		{
			get
			{
				return Server.MaxDecryptQueueBytes;
			}
			set
			{
				Server.MaxDecryptQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x060042EE RID: 17134 RVA: 0x0018B504 File Offset: 0x00189704
		[ServerVar]
		public static string printreadqueue(ConsoleSystem.Arg arg)
		{
			return "Server read queue: " + Net.sv.ReadQueueLength.ToString() + " items / " + Net.sv.ReadQueueBytes.FormatBytes(false);
		}

		// Token: 0x060042EF RID: 17135 RVA: 0x0018B544 File Offset: 0x00189744
		[ServerVar]
		public static string printwritequeue(ConsoleSystem.Arg arg)
		{
			return "Server write queue: " + Net.sv.WriteQueueLength.ToString() + " items / " + Net.sv.WriteQueueBytes.FormatBytes(false);
		}

		// Token: 0x060042F0 RID: 17136 RVA: 0x0018B584 File Offset: 0x00189784
		[ServerVar]
		public static string printdecryptqueue(ConsoleSystem.Arg arg)
		{
			return "Server decrypt queue: " + Net.sv.DecryptQueueLength.ToString() + " items / " + Net.sv.DecryptQueueBytes.FormatBytes(false);
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060042F1 RID: 17137 RVA: 0x0018B5C2 File Offset: 0x001897C2
		// (set) Token: 0x060042F2 RID: 17138 RVA: 0x0018B5CA File Offset: 0x001897CA
		[ServerVar]
		public static int maxpacketspersecond
		{
			get
			{
				return (int)Server.MaxPacketsPerSecond;
			}
			set
			{
				Server.MaxPacketsPerSecond = (ulong)((long)Mathf.Clamp(value, 1, 1000000));
			}
		}

		// Token: 0x060042F3 RID: 17139 RVA: 0x0018B5E0 File Offset: 0x001897E0
		[ServerVar]
		public static string packetlog(ConsoleSystem.Arg arg)
		{
			if (!Server.packetlog_enabled)
			{
				return "Packet log is not enabled.";
			}
			List<Tuple<Message.Type, ulong>> list = new List<Tuple<Message.Type, ulong>>();
			foreach (KeyValuePair<Message.Type, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.packetHistory.dict)
			{
				list.Add(new Tuple<Message.Type, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("calls");
			foreach (Tuple<Message.Type, ulong> tuple in list.OrderByDescending((Tuple<Message.Type, ulong> entry) => entry.Item2))
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = tuple.Item2.ToString();
				textTable.AddRow(new string[] { text, text2 });
			}
			if (!arg.HasArg("--json"))
			{
				return textTable.ToString();
			}
			return textTable.ToJson();
		}

		// Token: 0x060042F4 RID: 17140 RVA: 0x0018B744 File Offset: 0x00189944
		[ServerVar]
		public static string rpclog(ConsoleSystem.Arg arg)
		{
			if (!Server.rpclog_enabled)
			{
				return "RPC log is not enabled.";
			}
			List<Tuple<uint, ulong>> list = new List<Tuple<uint, ulong>>();
			foreach (KeyValuePair<uint, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.rpcHistory.dict)
			{
				list.Add(new Tuple<uint, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("calls");
			foreach (Tuple<uint, ulong> tuple in list.OrderByDescending((Tuple<uint, ulong> entry) => entry.Item2))
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = StringPool.Get(tuple.Item1);
				string text3 = tuple.Item2.ToString();
				textTable.AddRow(new string[] { text, text2, text3 });
			}
			return textTable.ToString();
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x0018B8AC File Offset: 0x00189AAC
		[ServerVar(Help = "Starts a server")]
		public static void start(ConsoleSystem.Arg arg)
		{
			if (Net.sv.IsConnected())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			string @string = arg.GetString(0, Server.level);
			if (!LevelManager.IsValid(@string))
			{
				arg.ReplyWith("Level '" + @string + "' isn't valid!");
				return;
			}
			if (UnityEngine.Object.FindObjectOfType<ServerMgr>())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true));
			LevelManager.LoadLevel(@string, true);
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x0018B931 File Offset: 0x00189B31
		[ServerVar(Help = "Stops a server")]
		public static void stop(ConsoleSystem.Arg arg)
		{
			if (!Net.sv.IsConnected())
			{
				arg.ReplyWith("There isn't a server running!");
				return;
			}
			Net.sv.Stop(arg.GetString(0, "Stopping Server"));
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060042F7 RID: 17143 RVA: 0x0018B961 File Offset: 0x00189B61
		public static string rootFolder
		{
			get
			{
				return "server/" + Server.identity;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060042F8 RID: 17144 RVA: 0x0018B972 File Offset: 0x00189B72
		public static string backupFolder
		{
			get
			{
				return "backup/0/" + Server.identity;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x060042F9 RID: 17145 RVA: 0x0018B983 File Offset: 0x00189B83
		public static string backupFolder1
		{
			get
			{
				return "backup/1/" + Server.identity;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060042FA RID: 17146 RVA: 0x0018B994 File Offset: 0x00189B94
		public static string backupFolder2
		{
			get
			{
				return "backup/2/" + Server.identity;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060042FB RID: 17147 RVA: 0x0018B9A5 File Offset: 0x00189BA5
		public static string backupFolder3
		{
			get
			{
				return "backup/3/" + Server.identity;
			}
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x0018B9B6 File Offset: 0x00189BB6
		[ServerVar(Help = "Backup server folder")]
		public static void backup()
		{
			DirectoryEx.Backup(new string[]
			{
				Server.backupFolder,
				Server.backupFolder1,
				Server.backupFolder2,
				Server.backupFolder3
			});
			DirectoryEx.CopyAll(Server.rootFolder, Server.backupFolder);
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x0018B9F4 File Offset: 0x00189BF4
		public static string GetServerFolder(string folder)
		{
			string text = Server.rootFolder + "/" + folder;
			if (Directory.Exists(text))
			{
				return text;
			}
			Directory.CreateDirectory(text);
			return text;
		}

		// Token: 0x060042FE RID: 17150 RVA: 0x0018BA24 File Offset: 0x00189C24
		[ServerVar(Help = "Writes config files")]
		public static void writecfg(ConsoleSystem.Arg arg)
		{
			string text = ConsoleSystem.SaveToConfigString(true);
			File.WriteAllText(Server.GetServerFolder("cfg") + "/serverauto.cfg", text);
			ServerUsers.Save();
			arg.ReplyWith("Config Saved");
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x0018BA62 File Offset: 0x00189C62
		[ServerVar]
		public static void fps(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(Performance.report.frameRate.ToString() + " FPS");
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x0018BA84 File Offset: 0x00189C84
		[ServerVar(Help = "Force save the current game")]
		public static void save(ConsoleSystem.Arg arg)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (BaseEntity baseEntity in BaseEntity.saveList)
			{
				baseEntity.InvalidateNetworkCache();
			}
			UnityEngine.Debug.Log("Invalidate Network Cache took " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds");
			SaveRestore.Save(true);
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x0018BB10 File Offset: 0x00189D10
		[ServerVar]
		public static string readcfg(ConsoleSystem.Arg arg)
		{
			string serverFolder = Server.GetServerFolder("cfg");
			if (File.Exists(serverFolder + "/serverauto.cfg"))
			{
				string text = File.ReadAllText(serverFolder + "/serverauto.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text);
			}
			if (File.Exists(serverFolder + "/server.cfg"))
			{
				string text2 = File.ReadAllText(serverFolder + "/server.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text2);
			}
			return "Server Config Loaded";
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06004302 RID: 17154 RVA: 0x0018BB99 File Offset: 0x00189D99
		// (set) Token: 0x06004303 RID: 17155 RVA: 0x0018BBAE File Offset: 0x00189DAE
		[ServerVar]
		public static bool compression
		{
			get
			{
				return Net.sv != null && Net.sv.compressionEnabled;
			}
			set
			{
				Net.sv.compressionEnabled = value;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06004304 RID: 17156 RVA: 0x0018BBBB File Offset: 0x00189DBB
		// (set) Token: 0x06004305 RID: 17157 RVA: 0x0018BBD0 File Offset: 0x00189DD0
		[ServerVar]
		public static bool netlog
		{
			get
			{
				return Net.sv != null && Net.sv.logging;
			}
			set
			{
				Net.sv.logging = value;
			}
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x0018BBDD File Offset: 0x00189DDD
		[ServerVar]
		public static string netprotocol(ConsoleSystem.Arg arg)
		{
			if (Net.sv == null)
			{
				return string.Empty;
			}
			return Net.sv.ProtocolId;
		}

		// Token: 0x06004307 RID: 17159 RVA: 0x0018BBF8 File Offset: 0x00189DF8
		[ServerUserVar]
		public static void cheatreport(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			string text = arg.GetUInt64(0, 0UL).ToString();
			string @string = arg.GetString(1, "");
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				basePlayer,
				" reported ",
				text,
				": ",
				@string.ToPrintable(140)
			}));
			EACServer.SendPlayerBehaviorReport(basePlayer, PlayerReportsCategory.Cheating, text, @string);
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x0018BC74 File Offset: 0x00189E74
		[ServerAllVar(Help = "Get the player combat log")]
		public static string combatlog(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int num = Server.combatlogsize;
			NetworkableId networkableId = default(NetworkableId);
			bool flag = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(num, networkableId, flag, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x06004309 RID: 17161 RVA: 0x0018BCFC File Offset: 0x00189EFC
		[ServerAllVar(Help = "Get the player combat log, only showing outgoing damage")]
		public static string combatlog_outgoing(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int num = Server.combatlogsize;
			NetworkableId id = basePlayer.net.ID;
			bool flag = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(num, id, flag, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x0018BD7C File Offset: 0x00189F7C
		[ServerVar(Help = "Print the current player position.")]
		public static string printpos(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.position.ToString();
			}
			return "invalid player";
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x0018BDCC File Offset: 0x00189FCC
		[ServerVar(Help = "Print the current player rotation.")]
		public static string printrot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x0600430C RID: 17164 RVA: 0x0018BE24 File Offset: 0x0018A024
		[ServerVar(Help = "Print the current player eyes.")]
		public static string printeyes(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.eyes.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x0600430D RID: 17165 RVA: 0x0018BE7C File Offset: 0x0018A07C
		[ServerVar(ServerAdmin = true, Help = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.")]
		public static void snapshot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			UnityEngine.Debug.Log("Sending full snapshot to " + basePlayer);
			basePlayer.SendNetworkUpdateImmediate(false);
			basePlayer.SendGlobalSnapshot();
			basePlayer.SendFullSnapshot();
			basePlayer.SendEntityUpdate();
			TreeManager.SendSnapshot(basePlayer);
			ServerMgr.SendReplicatedVars(basePlayer.net.connection);
		}

		// Token: 0x0600430E RID: 17166 RVA: 0x0018BEDC File Offset: 0x0018A0DC
		[ServerVar(Help = "Send network update for all players")]
		public static void sendnetworkupdate(ConsoleSystem.Arg arg)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x0600430F RID: 17167 RVA: 0x0018BF2C File Offset: 0x0018A12C
		[ServerVar(Help = "Prints the position of all players on the server")]
		public static void playerlistpos(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "SteamID", "DisplayName", "POS", "ROT" });
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.userID.ToString(),
					basePlayer.displayName,
					basePlayer.transform.position.ToString(),
					basePlayer.eyes.BodyForward().ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x0018C028 File Offset: 0x0018A228
		[ServerVar(Help = "Prints all the vending machines on the server")]
		public static void listvendingmachines(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "EntityId", "Position", "Name" });
			foreach (VendingMachine vendingMachine in BaseNetworkable.serverEntities.OfType<VendingMachine>())
			{
				textTable.AddRow(new string[]
				{
					vendingMachine.net.ID.ToString(),
					vendingMachine.transform.position.ToString(),
					vendingMachine.shopName.QuoteSafe()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x0018C10C File Offset: 0x0018A30C
		[ServerVar(Help = "Prints all the Tool Cupboards on the server")]
		public static void listtoolcupboards(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "EntityId", "Position", "Authed" });
			foreach (BuildingPrivlidge buildingPrivlidge in BaseNetworkable.serverEntities.OfType<BuildingPrivlidge>())
			{
				textTable.AddRow(new string[]
				{
					buildingPrivlidge.net.ID.ToString(),
					buildingPrivlidge.transform.position.ToString(),
					buildingPrivlidge.authorizedPlayers.Count.ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x0018C1FC File Offset: 0x0018A3FC
		[ServerVar]
		public static void BroadcastPlayVideo(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				arg.ReplyWith("Missing video URL");
				return;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.Command("client.playvideo", new object[] { @string });
			}
			arg.ReplyWith(string.Format("Sent video to {0} players", BasePlayer.activePlayerList.Count));
		}

		// Token: 0x04003C05 RID: 15365
		[ServerVar]
		public static string ip = "";

		// Token: 0x04003C06 RID: 15366
		[ServerVar]
		public static int port = 28015;

		// Token: 0x04003C07 RID: 15367
		[ServerVar]
		public static int queryport = 0;

		// Token: 0x04003C08 RID: 15368
		[ServerVar(ShowInAdminUI = true)]
		public static int maxplayers = 500;

		// Token: 0x04003C09 RID: 15369
		[ServerVar(ShowInAdminUI = true)]
		public static string hostname = "My Untitled Rust Server";

		// Token: 0x04003C0A RID: 15370
		[ServerVar]
		public static string identity = "my_server_identity";

		// Token: 0x04003C0B RID: 15371
		[ServerVar]
		public static string level = "Procedural Map";

		// Token: 0x04003C0C RID: 15372
		[ServerVar]
		public static string levelurl = "";

		// Token: 0x04003C0D RID: 15373
		[ServerVar]
		public static bool leveltransfer = true;

		// Token: 0x04003C0E RID: 15374
		[ServerVar]
		public static int seed = 1337;

		// Token: 0x04003C0F RID: 15375
		[ServerVar]
		public static int salt = 1;

		// Token: 0x04003C10 RID: 15376
		[ServerVar]
		public static int worldsize = 4500;

		// Token: 0x04003C11 RID: 15377
		[ServerVar]
		public static int saveinterval = 600;

		// Token: 0x04003C12 RID: 15378
		[ServerVar]
		public static bool secure = true;

		// Token: 0x04003C13 RID: 15379
		[ServerVar]
		public static int encryption = 2;

		// Token: 0x04003C14 RID: 15380
		[ServerVar]
		public static string anticheatid = "xyza7891h6UjNfd0eb2HQGtaul0WhfvS";

		// Token: 0x04003C15 RID: 15381
		[ServerVar]
		public static string anticheatkey = "OWUDFZmi9VNL/7VhGVSSmCWALKTltKw8ISepa0VXs60";

		// Token: 0x04003C16 RID: 15382
		[ServerVar]
		public static int tickrate = 10;

		// Token: 0x04003C17 RID: 15383
		[ServerVar]
		public static int entityrate = 16;

		// Token: 0x04003C18 RID: 15384
		[ServerVar]
		public static float schematime = 1800f;

		// Token: 0x04003C19 RID: 15385
		[ServerVar]
		public static float cycletime = 500f;

		// Token: 0x04003C1A RID: 15386
		[ServerVar]
		public static bool official = false;

		// Token: 0x04003C1B RID: 15387
		[ServerVar]
		public static bool stats = false;

		// Token: 0x04003C1C RID: 15388
		[ServerVar]
		public static bool stability = true;

		// Token: 0x04003C1D RID: 15389
		[ServerVar(ShowInAdminUI = true)]
		public static bool radiation = true;

		// Token: 0x04003C1E RID: 15390
		[ServerVar]
		public static float itemdespawn = 300f;

		// Token: 0x04003C1F RID: 15391
		[ServerVar]
		public static float itemdespawn_container_scale = 2f;

		// Token: 0x04003C20 RID: 15392
		[ServerVar]
		public static float itemdespawn_quick = 30f;

		// Token: 0x04003C21 RID: 15393
		[ServerVar]
		public static float corpsedespawn = 300f;

		// Token: 0x04003C22 RID: 15394
		[ServerVar]
		public static float debrisdespawn = 30f;

		// Token: 0x04003C23 RID: 15395
		[ServerVar]
		public static bool pve = false;

		// Token: 0x04003C24 RID: 15396
		[ServerVar]
		public static bool cinematic = false;

		// Token: 0x04003C25 RID: 15397
		[ServerVar(ShowInAdminUI = true)]
		public static string description = "No server description has been provided.";

		// Token: 0x04003C26 RID: 15398
		[ServerVar(ShowInAdminUI = true)]
		public static string url = "";

		// Token: 0x04003C27 RID: 15399
		[ServerVar]
		public static string branch = "";

		// Token: 0x04003C28 RID: 15400
		[ServerVar]
		public static int queriesPerSecond = 2000;

		// Token: 0x04003C29 RID: 15401
		[ServerVar]
		public static int ipQueriesPerMin = 30;

		// Token: 0x04003C2A RID: 15402
		[ServerVar]
		public static bool statBackup = false;

		// Token: 0x04003C2B RID: 15403
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string headerimage = "";

		// Token: 0x04003C2C RID: 15404
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string logoimage = "";

		// Token: 0x04003C2D RID: 15405
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static int saveBackupCount = 2;

		// Token: 0x04003C2E RID: 15406
		[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
		public static string motd = "";

		// Token: 0x04003C2F RID: 15407
		[ServerVar(Saved = true)]
		public static float meleedamage = 1f;

		// Token: 0x04003C30 RID: 15408
		[ServerVar(Saved = true)]
		public static float arrowdamage = 1f;

		// Token: 0x04003C31 RID: 15409
		[ServerVar(Saved = true)]
		public static float bulletdamage = 1f;

		// Token: 0x04003C32 RID: 15410
		[ServerVar(Saved = true)]
		public static float bleedingdamage = 1f;

		// Token: 0x04003C33 RID: 15411
		[ReplicatedVar(Saved = true)]
		public static float funWaterDamageThreshold = 0.8f;

		// Token: 0x04003C34 RID: 15412
		[ReplicatedVar(Saved = true)]
		public static float funWaterWetnessGain = 0.05f;

		// Token: 0x04003C35 RID: 15413
		[ServerVar(Saved = true)]
		public static float meleearmor = 1f;

		// Token: 0x04003C36 RID: 15414
		[ServerVar(Saved = true)]
		public static float arrowarmor = 1f;

		// Token: 0x04003C37 RID: 15415
		[ServerVar(Saved = true)]
		public static float bulletarmor = 1f;

		// Token: 0x04003C38 RID: 15416
		[ServerVar(Saved = true)]
		public static float bleedingarmor = 1f;

		// Token: 0x04003C39 RID: 15417
		[ServerVar]
		public static int updatebatch = 512;

		// Token: 0x04003C3A RID: 15418
		[ServerVar]
		public static int updatebatchspawn = 1024;

		// Token: 0x04003C3B RID: 15419
		[ServerVar]
		public static int entitybatchsize = 100;

		// Token: 0x04003C3C RID: 15420
		[ServerVar]
		public static float entitybatchtime = 1f;

		// Token: 0x04003C3D RID: 15421
		[ServerVar]
		public static float composterUpdateInterval = 300f;

		// Token: 0x04003C3E RID: 15422
		[ReplicatedVar]
		public static float planttick = 60f;

		// Token: 0x04003C3F RID: 15423
		[ServerVar]
		public static float planttickscale = 1f;

		// Token: 0x04003C40 RID: 15424
		[ServerVar]
		public static bool useMinimumPlantCondition = true;

		// Token: 0x04003C41 RID: 15425
		[ServerVar(Saved = true)]
		public static float nonPlanterDeathChancePerTick = 0.005f;

		// Token: 0x04003C42 RID: 15426
		[ServerVar(Saved = true)]
		public static float ceilingLightGrowableRange = 3f;

		// Token: 0x04003C43 RID: 15427
		[ServerVar(Saved = true)]
		public static float artificialTemperatureGrowableRange = 4f;

		// Token: 0x04003C44 RID: 15428
		[ServerVar(Saved = true)]
		public static float ceilingLightHeightOffset = 3f;

		// Token: 0x04003C45 RID: 15429
		[ServerVar(Saved = true)]
		public static float sprinklerRadius = 3f;

		// Token: 0x04003C46 RID: 15430
		[ServerVar(Saved = true)]
		public static float sprinklerEyeHeightOffset = 3f;

		// Token: 0x04003C47 RID: 15431
		[ServerVar(Saved = true)]
		public static float optimalPlanterQualitySaturation = 0.6f;

		// Token: 0x04003C48 RID: 15432
		[ServerVar]
		public static float metabolismtick = 1f;

		// Token: 0x04003C49 RID: 15433
		[ServerVar]
		public static float modifierTickRate = 1f;

		// Token: 0x04003C4A RID: 15434
		[ServerVar(Saved = true)]
		public static float rewounddelay = 60f;

		// Token: 0x04003C4B RID: 15435
		[ServerVar(Saved = true, Help = "Can players be wounded after recieving fatal damage")]
		public static bool woundingenabled = true;

		// Token: 0x04003C4C RID: 15436
		[ServerVar(Saved = true, Help = "Do players go into the crawling wounded state")]
		public static bool crawlingenabled = true;

		// Token: 0x04003C4D RID: 15437
		[ServerVar(Help = "Base chance of recovery after crawling wounded state", Saved = true)]
		public static float woundedrecoverchance = 0.2f;

		// Token: 0x04003C4E RID: 15438
		[ServerVar(Help = "Base chance of recovery after incapacitated wounded state", Saved = true)]
		public static float incapacitatedrecoverchance = 0.1f;

		// Token: 0x04003C4F RID: 15439
		[ServerVar(Help = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level", Saved = true)]
		public static float woundedmaxfoodandwaterbonus = 0.25f;

		// Token: 0x04003C50 RID: 15440
		[ServerVar(Help = "Minimum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingminimumhealth = 7;

		// Token: 0x04003C51 RID: 15441
		[ServerVar(Help = "Maximum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingmaximumhealth = 12;

		// Token: 0x04003C52 RID: 15442
		[ServerVar(Saved = true)]
		public static bool playerserverfall = true;

		// Token: 0x04003C53 RID: 15443
		[ServerVar]
		public static bool plantlightdetection = true;

		// Token: 0x04003C54 RID: 15444
		[ServerVar]
		public static float respawnresetrange = 50f;

		// Token: 0x04003C55 RID: 15445
		[ReplicatedVar]
		public static int max_sleeping_bags = 15;

		// Token: 0x04003C56 RID: 15446
		[ReplicatedVar]
		public static bool bag_quota_item_amount = true;

		// Token: 0x04003C57 RID: 15447
		[ServerVar]
		public static int maxunack = 4;

		// Token: 0x04003C58 RID: 15448
		[ServerVar]
		public static bool netcache = true;

		// Token: 0x04003C59 RID: 15449
		[ServerVar]
		public static bool corpses = true;

		// Token: 0x04003C5A RID: 15450
		[ServerVar]
		public static bool events = true;

		// Token: 0x04003C5B RID: 15451
		[ServerVar]
		public static bool dropitems = true;

		// Token: 0x04003C5C RID: 15452
		[ServerVar]
		public static int netcachesize = 0;

		// Token: 0x04003C5D RID: 15453
		[ServerVar]
		public static int savecachesize = 0;

		// Token: 0x04003C5E RID: 15454
		[ServerVar]
		public static int combatlogsize = 30;

		// Token: 0x04003C5F RID: 15455
		[ServerVar]
		public static int combatlogdelay = 10;

		// Token: 0x04003C60 RID: 15456
		[ServerVar]
		public static int authtimeout = 60;

		// Token: 0x04003C61 RID: 15457
		[ServerVar]
		public static int playertimeout = 60;

		// Token: 0x04003C62 RID: 15458
		[ServerVar(ShowInAdminUI = true)]
		public static int idlekick = 30;

		// Token: 0x04003C63 RID: 15459
		[ServerVar]
		public static int idlekickmode = 1;

		// Token: 0x04003C64 RID: 15460
		[ServerVar]
		public static int idlekickadmins = 0;

		// Token: 0x04003C65 RID: 15461
		[ServerVar]
		public static string gamemode = "";

		// Token: 0x04003C66 RID: 15462
		[ServerVar(Help = "Comma-separated server browser tag values (see wiki)", Saved = true, ShowInAdminUI = true)]
		public static string tags = "";

		// Token: 0x04003C67 RID: 15463
		[ServerVar(Help = "Censors the Steam player list to make player tracking more difficult")]
		public static bool censorplayerlist = true;

		// Token: 0x04003C68 RID: 15464
		[ServerVar(Help = "HTTP API endpoint for centralized banning (see wiki)")]
		public static string bansServerEndpoint = "";

		// Token: 0x04003C69 RID: 15465
		[ServerVar(Help = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)")]
		public static int bansServerFailureMode = 0;

		// Token: 0x04003C6A RID: 15466
		[ServerVar(Help = "Timeout (in seconds) for centralized banning web server requests")]
		public static int bansServerTimeout = 5;

		// Token: 0x04003C6B RID: 15467
		[ServerVar(Help = "HTTP API endpoint for receiving F7 reports", Saved = true)]
		public static string reportsServerEndpoint = "";

		// Token: 0x04003C6C RID: 15468
		[ServerVar(Help = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)", Saved = true)]
		public static string reportsServerEndpointKey = "";

		// Token: 0x04003C6D RID: 15469
		[ServerVar(Help = "Should F7 reports from players be printed to console", Saved = true)]
		public static bool printReportsToConsole = false;

		// Token: 0x04003C6E RID: 15470
		[ServerVar(Help = "If a player presses the respawn button, respawn at their death location (for trailer filming)")]
		public static bool respawnAtDeathPosition = false;

		// Token: 0x04003C6F RID: 15471
		[ServerVar(Help = "When a player respawns give them the loadout assigned to client.RespawnLoadout (created with inventory.saveloadout)")]
		public static bool respawnWithLoadout = false;

		// Token: 0x04003C70 RID: 15472
		[ServerVar(Help = "When transferring water, should containers keep 1 water behind. Enabling this should help performance if water IO is causing performance loss", Saved = true)]
		public static bool waterContainersLeaveWaterBehind = false;

		// Token: 0x04003C71 RID: 15473
		[ServerVar(Help = "How often industrial conveyors attempt to move items (value is an interval measured in seconds). Setting to 0 will disable all movement", Saved = true, ShowInAdminUI = true)]
		public static float conveyorMoveFrequency = 5f;

		// Token: 0x04003C72 RID: 15474
		[ServerVar(Help = "How often industrial crafters attempt to craft items (value is an interval measured in seconds). Setting to 0 will disable all crafting", Saved = true, ShowInAdminUI = true)]
		public static float industrialCrafterFrequency = 5f;

		// Token: 0x04003C73 RID: 15475
		[ReplicatedVar(Help = "How much scrap is required to research default blueprints", Saved = true, ShowInAdminUI = true)]
		public static int defaultBlueprintResearchCost = 10;

		// Token: 0x04003C74 RID: 15476
		[ServerVar(Help = "Whether to check for illegal industrial pipes when changing building block states (roof bunkers)", Saved = true, ShowInAdminUI = true)]
		public static bool enforcePipeChecksOnBuildingBlockChanges = true;

		// Token: 0x04003C75 RID: 15477
		[ServerVar(Help = "How many stacks a single conveyor can move in a single tick", Saved = true, ShowInAdminUI = true)]
		public static int maxItemStacksMovedPerTickIndustrial = 12;

		// Token: 0x04003C76 RID: 15478
		[ServerVar(Help = "How long per frame to spend on industrial jobs", Saved = true, ShowInAdminUI = true)]
		public static float industrialFrameBudgetMs = 0.5f;

		// Token: 0x04003C77 RID: 15479
		[ReplicatedVar(Help = "How many markers each player can place", Saved = true, ShowInAdminUI = true)]
		public static int maximumMapMarkers = 5;

		// Token: 0x04003C78 RID: 15480
		[ServerVar(Help = "How many pings can be placed by each player", Saved = true, ShowInAdminUI = true)]
		public static int maximumPings = 5;

		// Token: 0x04003C79 RID: 15481
		[ServerVar(Help = "How long a ping should last", Saved = true, ShowInAdminUI = true)]
		public static float pingDuration = 10f;

		// Token: 0x04003C7A RID: 15482
		[ServerVar(Saved = true)]
		public static bool showHolsteredItems = true;

		// Token: 0x04003C7B RID: 15483
		[ServerVar]
		public static int maxpacketspersecond_world = 1;

		// Token: 0x04003C7C RID: 15484
		[ServerVar]
		public static int maxpacketspersecond_rpc = 200;

		// Token: 0x04003C7D RID: 15485
		[ServerVar]
		public static int maxpacketspersecond_rpc_signal = 50;

		// Token: 0x04003C7E RID: 15486
		[ServerVar]
		public static int maxpacketspersecond_command = 100;

		// Token: 0x04003C7F RID: 15487
		[ServerVar]
		public static int maxpacketsize_command = 100000;

		// Token: 0x04003C80 RID: 15488
		[ServerVar]
		public static int maxpacketspersecond_tick = 300;

		// Token: 0x04003C81 RID: 15489
		[ServerVar]
		public static int maxpacketspersecond_voice = 100;

		// Token: 0x04003C82 RID: 15490
		[ServerVar]
		public static bool packetlog_enabled = false;

		// Token: 0x04003C83 RID: 15491
		[ServerVar]
		public static bool rpclog_enabled = false;
	}
}
