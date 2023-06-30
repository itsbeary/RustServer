using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AF6 RID: 2806
	public class RCon
	{
		// Token: 0x0600439A RID: 17306 RVA: 0x0018E47C File Offset: 0x0018C67C
		public static void Initialize()
		{
			if (RCon.Port == 0)
			{
				RCon.Port = Server.port;
			}
			RCon.Password = CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", ""));
			if (RCon.Password == "password")
			{
				return;
			}
			if (RCon.Password == "")
			{
				return;
			}
			Output.OnMessage += RCon.OnMessage;
			if (RCon.Web)
			{
				RCon.listenerNew = new Listener();
				if (!string.IsNullOrEmpty(RCon.Ip))
				{
					RCon.listenerNew.Address = RCon.Ip;
				}
				RCon.listenerNew.Password = RCon.Password;
				RCon.listenerNew.Port = RCon.Port;
				RCon.listenerNew.SslCertificate = CommandLine.GetSwitch("-rcon.ssl", null);
				RCon.listenerNew.SslCertificatePassword = CommandLine.GetSwitch("-rcon.sslpwd", null);
				RCon.listenerNew.OnMessage = delegate(IPAddress ip, int id, string msg)
				{
					Queue<RCon.Command> commands = RCon.Commands;
					lock (commands)
					{
						RCon.Command command = JsonConvert.DeserializeObject<RCon.Command>(msg);
						command.Ip = ip;
						command.ConnectionId = id;
						RCon.Commands.Enqueue(command);
					}
				};
				RCon.listenerNew.Start();
				Debug.Log("WebSocket RCon Started on " + RCon.Port);
				return;
			}
			RCon.listener = new RCon.RConListener();
			Debug.Log("RCon Started on " + RCon.Port);
			Debug.Log("Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
		}

		// Token: 0x0600439B RID: 17307 RVA: 0x0018E5DE File Offset: 0x0018C7DE
		public static void Shutdown()
		{
			if (RCon.listenerNew != null)
			{
				RCon.listenerNew.Shutdown();
				RCon.listenerNew = null;
			}
			if (RCon.listener != null)
			{
				RCon.listener.Shutdown();
				RCon.listener = null;
			}
		}

		// Token: 0x0600439C RID: 17308 RVA: 0x0018E610 File Offset: 0x0018C810
		public static void Broadcast(RCon.LogType type, object obj)
		{
			if (RCon.listenerNew == null)
			{
				return;
			}
			string text = JsonConvert.SerializeObject(obj, Formatting.Indented);
			RCon.Broadcast(type, text);
		}

		// Token: 0x0600439D RID: 17309 RVA: 0x0018E634 File Offset: 0x0018C834
		public static void Broadcast(RCon.LogType type, string message)
		{
			if (RCon.listenerNew == null || string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			RCon.Response response = default(RCon.Response);
			response.Identifier = -1;
			response.Message = message;
			response.Type = type;
			if (RCon.responseConnection < 0)
			{
				RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
				return;
			}
			RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x0018E6AC File Offset: 0x0018C8AC
		public static void Update()
		{
			Queue<RCon.Command> commands = RCon.Commands;
			lock (commands)
			{
				while (RCon.Commands.Count > 0)
				{
					RCon.OnCommand(RCon.Commands.Dequeue());
				}
			}
			if (RCon.listener == null)
			{
				return;
			}
			if (RCon.lastRunTime + 0.02f >= UnityEngine.Time.realtimeSinceStartup)
			{
				return;
			}
			RCon.lastRunTime = UnityEngine.Time.realtimeSinceStartup;
			try
			{
				RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.banTime < UnityEngine.Time.realtimeSinceStartup);
				RCon.listener.Cycle();
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Rcon Exception");
				Debug.LogException(ex);
			}
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x0018E77C File Offset: 0x0018C97C
		public static void BanIP(IPAddress addr, float seconds)
		{
			RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.addr == addr);
			RCon.BannedAddresses bannedAddresses = default(RCon.BannedAddresses);
			bannedAddresses.addr = addr;
			bannedAddresses.banTime = UnityEngine.Time.realtimeSinceStartup + seconds;
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x0018E7D0 File Offset: 0x0018C9D0
		public static bool IsBanned(IPAddress addr)
		{
			return RCon.bannedAddresses.Count((RCon.BannedAddresses x) => x.addr == addr && x.banTime > UnityEngine.Time.realtimeSinceStartup) > 0;
		}

		// Token: 0x060043A1 RID: 17313 RVA: 0x0018E804 File Offset: 0x0018CA04
		private static void OnCommand(RCon.Command cmd)
		{
			try
			{
				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = true;
				if (RCon.Print)
				{
					Debug.Log(string.Concat(new object[] { "[rcon] ", cmd.Ip, ": ", cmd.Message }));
				}
				RCon.isInput = false;
				string text = ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), cmd.Message, Array.Empty<object>());
				if (text != null)
				{
					RCon.OnMessage(text, string.Empty, UnityEngine.LogType.Log);
				}
			}
			finally
			{
				RCon.responseIdentifier = 0;
				RCon.responseConnection = -1;
			}
		}

		// Token: 0x060043A2 RID: 17314 RVA: 0x0018E8B8 File Offset: 0x0018CAB8
		private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
		{
			if (RCon.isInput)
			{
				return;
			}
			if (RCon.listenerNew != null)
			{
				RCon.Response response = default(RCon.Response);
				response.Identifier = RCon.responseIdentifier;
				response.Message = message;
				response.Stacktrace = stacktrace;
				response.Type = RCon.LogType.Generic;
				if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
				{
					response.Type = RCon.LogType.Error;
				}
				if (type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Warning)
				{
					response.Type = RCon.LogType.Warning;
				}
				if (RCon.responseConnection < 0)
				{
					RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
					return;
				}
				RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
			}
		}

		// Token: 0x04003CC3 RID: 15555
		public static string Password = "";

		// Token: 0x04003CC4 RID: 15556
		[ServerVar]
		public static int Port = 0;

		// Token: 0x04003CC5 RID: 15557
		[ServerVar]
		public static string Ip = "";

		// Token: 0x04003CC6 RID: 15558
		[ServerVar(Help = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
		public static bool Web = true;

		// Token: 0x04003CC7 RID: 15559
		[ServerVar(Help = "If true, rcon commands etc will be printed in the console")]
		public static bool Print = false;

		// Token: 0x04003CC8 RID: 15560
		internal static RCon.RConListener listener = null;

		// Token: 0x04003CC9 RID: 15561
		internal static Listener listenerNew = null;

		// Token: 0x04003CCA RID: 15562
		private static Queue<RCon.Command> Commands = new Queue<RCon.Command>();

		// Token: 0x04003CCB RID: 15563
		private static float lastRunTime = 0f;

		// Token: 0x04003CCC RID: 15564
		internal static List<RCon.BannedAddresses> bannedAddresses = new List<RCon.BannedAddresses>();

		// Token: 0x04003CCD RID: 15565
		private static int responseIdentifier;

		// Token: 0x04003CCE RID: 15566
		private static int responseConnection;

		// Token: 0x04003CCF RID: 15567
		private static bool isInput;

		// Token: 0x04003CD0 RID: 15568
		internal static int SERVERDATA_AUTH = 3;

		// Token: 0x04003CD1 RID: 15569
		internal static int SERVERDATA_EXECCOMMAND = 2;

		// Token: 0x04003CD2 RID: 15570
		internal static int SERVERDATA_AUTH_RESPONSE = 2;

		// Token: 0x04003CD3 RID: 15571
		internal static int SERVERDATA_RESPONSE_VALUE = 0;

		// Token: 0x04003CD4 RID: 15572
		internal static int SERVERDATA_CONSOLE_LOG = 4;

		// Token: 0x04003CD5 RID: 15573
		internal static int SERVERDATA_SWITCH_UTF8 = 5;

		// Token: 0x02000F7C RID: 3964
		public struct Command
		{
			// Token: 0x04005039 RID: 20537
			public IPAddress Ip;

			// Token: 0x0400503A RID: 20538
			public int ConnectionId;

			// Token: 0x0400503B RID: 20539
			public string Name;

			// Token: 0x0400503C RID: 20540
			public string Message;

			// Token: 0x0400503D RID: 20541
			public int Identifier;
		}

		// Token: 0x02000F7D RID: 3965
		public enum LogType
		{
			// Token: 0x0400503F RID: 20543
			Generic,
			// Token: 0x04005040 RID: 20544
			Error,
			// Token: 0x04005041 RID: 20545
			Warning,
			// Token: 0x04005042 RID: 20546
			Chat,
			// Token: 0x04005043 RID: 20547
			Report,
			// Token: 0x04005044 RID: 20548
			ClientPerf
		}

		// Token: 0x02000F7E RID: 3966
		public struct Response
		{
			// Token: 0x04005045 RID: 20549
			public string Message;

			// Token: 0x04005046 RID: 20550
			public int Identifier;

			// Token: 0x04005047 RID: 20551
			[JsonConverter(typeof(StringEnumConverter))]
			public RCon.LogType Type;

			// Token: 0x04005048 RID: 20552
			public string Stacktrace;
		}

		// Token: 0x02000F7F RID: 3967
		internal struct BannedAddresses
		{
			// Token: 0x04005049 RID: 20553
			public IPAddress addr;

			// Token: 0x0400504A RID: 20554
			public float banTime;
		}

		// Token: 0x02000F80 RID: 3968
		internal class RConClient
		{
			// Token: 0x060054ED RID: 21741 RVA: 0x001B65C2 File Offset: 0x001B47C2
			internal RConClient(Socket cl)
			{
				this.socket = cl;
				this.socket.NoDelay = true;
				this.connectionName = this.socket.RemoteEndPoint.ToString();
			}

			// Token: 0x060054EE RID: 21742 RVA: 0x001B65FA File Offset: 0x001B47FA
			internal bool IsValid()
			{
				return this.socket != null;
			}

			// Token: 0x060054EF RID: 21743 RVA: 0x001B6608 File Offset: 0x001B4808
			internal void Update()
			{
				if (this.socket == null)
				{
					return;
				}
				if (!this.socket.Connected)
				{
					this.Close("Disconnected");
					return;
				}
				int available = this.socket.Available;
				if (available < 14)
				{
					return;
				}
				if (available > 4096)
				{
					this.Close("overflow");
					return;
				}
				byte[] array = new byte[available];
				this.socket.Receive(array);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(array, false), this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII))
				{
					int num = binaryReader.ReadInt32();
					if (available < num)
					{
						this.Close("invalid packet");
					}
					else
					{
						this.lastMessageID = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						string text = this.ReadNullTerminatedString(binaryReader);
						this.ReadNullTerminatedString(binaryReader);
						if (!this.HandleMessage(num2, text))
						{
							this.Close("invalid packet");
						}
						else
						{
							this.lastMessageID = -1;
						}
					}
				}
			}

			// Token: 0x060054F0 RID: 21744 RVA: 0x001B670C File Offset: 0x001B490C
			internal bool HandleMessage(int type, string msg)
			{
				if (!this.isAuthorised)
				{
					return this.HandleMessage_UnAuthed(type, msg);
				}
				if (type == RCon.SERVERDATA_SWITCH_UTF8)
				{
					this.utf8Mode = true;
					return true;
				}
				if (type == RCon.SERVERDATA_EXECCOMMAND)
				{
					Debug.Log("[RCON][" + this.connectionName + "] " + msg);
					this.runningConsoleCommand = true;
					ConsoleSystem.Run(ConsoleSystem.Option.Server, msg, Array.Empty<object>());
					this.runningConsoleCommand = false;
					this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				if (type == RCon.SERVERDATA_RESPONSE_VALUE)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				Debug.Log(string.Concat(new object[] { "[RCON][", this.connectionName, "] Unhandled: ", this.lastMessageID, " -> ", type, " -> ", msg }));
				return false;
			}

			// Token: 0x060054F1 RID: 21745 RVA: 0x001B6808 File Offset: 0x001B4A08
			internal bool HandleMessage_UnAuthed(int type, string msg)
			{
				if (type != RCon.SERVERDATA_AUTH)
				{
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Command - Not Authed");
					return false;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
				this.isAuthorised = RCon.Password == msg;
				if (!this.isAuthorised)
				{
					this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Password");
					return true;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
				Debug.Log("[RCON] Auth: " + this.connectionName);
				Output.OnMessage += this.Output_OnMessage;
				return true;
			}

			// Token: 0x060054F2 RID: 21746 RVA: 0x001B68F4 File Offset: 0x001B4AF4
			private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
			{
				if (!this.isAuthorised)
				{
					return;
				}
				if (!this.IsValid())
				{
					return;
				}
				if (this.lastMessageID != -1 && this.runningConsoleCommand)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
				}
				this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
			}

			// Token: 0x060054F3 RID: 21747 RVA: 0x001B6944 File Offset: 0x001B4B44
			internal void Reply(int id, int type, string msg)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (this.utf8Mode)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(msg);
						int num = 10 + bytes.Length;
						binaryWriter.Write(num);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						binaryWriter.Write(bytes);
					}
					else
					{
						int num2 = 10 + msg.Length;
						binaryWriter.Write(num2);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						foreach (char c in msg)
						{
							binaryWriter.Write((sbyte)c);
						}
					}
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Flush();
					try
					{
						this.socket.Send(memoryStream.GetBuffer(), (int)memoryStream.Position, SocketFlags.None);
					}
					catch (Exception ex)
					{
						Debug.LogWarning("Error sending rcon reply: " + ex);
						this.Close("Exception");
					}
				}
			}

			// Token: 0x060054F4 RID: 21748 RVA: 0x001B6A60 File Offset: 0x001B4C60
			internal void Close(string strReasn)
			{
				Output.OnMessage -= this.Output_OnMessage;
				if (this.socket == null)
				{
					return;
				}
				Debug.Log("[RCON][" + this.connectionName + "] Disconnected: " + strReasn);
				this.socket.Close();
				this.socket = null;
			}

			// Token: 0x060054F5 RID: 21749 RVA: 0x001B6AB4 File Offset: 0x001B4CB4
			internal string ReadNullTerminatedString(BinaryReader read)
			{
				string text = "";
				while (read.BaseStream.Position != read.BaseStream.Length)
				{
					char c = read.ReadChar();
					if (c == '\0')
					{
						return text;
					}
					text += c.ToString();
					if (text.Length > 8192)
					{
						return string.Empty;
					}
				}
				return "";
			}

			// Token: 0x0400504B RID: 20555
			private Socket socket;

			// Token: 0x0400504C RID: 20556
			private bool isAuthorised;

			// Token: 0x0400504D RID: 20557
			private string connectionName;

			// Token: 0x0400504E RID: 20558
			private int lastMessageID = -1;

			// Token: 0x0400504F RID: 20559
			private bool runningConsoleCommand;

			// Token: 0x04005050 RID: 20560
			private bool utf8Mode;
		}

		// Token: 0x02000F81 RID: 3969
		internal class RConListener
		{
			// Token: 0x060054F6 RID: 21750 RVA: 0x001B6B14 File Offset: 0x001B4D14
			internal RConListener()
			{
				IPAddress ipaddress = IPAddress.Any;
				if (!IPAddress.TryParse(RCon.Ip, out ipaddress))
				{
					ipaddress = IPAddress.Any;
				}
				this.server = new TcpListener(ipaddress, RCon.Port);
				try
				{
					this.server.Start();
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Couldn't start RCON Listener: " + ex.Message);
					this.server = null;
				}
			}

			// Token: 0x060054F7 RID: 21751 RVA: 0x001B6B9C File Offset: 0x001B4D9C
			internal void Shutdown()
			{
				if (this.server != null)
				{
					this.server.Stop();
					this.server = null;
				}
			}

			// Token: 0x060054F8 RID: 21752 RVA: 0x001B6BB8 File Offset: 0x001B4DB8
			internal void Cycle()
			{
				if (this.server == null)
				{
					return;
				}
				this.ProcessConnections();
				this.RemoveDeadClients();
				this.UpdateClients();
			}

			// Token: 0x060054F9 RID: 21753 RVA: 0x001B6BD8 File Offset: 0x001B4DD8
			private void ProcessConnections()
			{
				if (!this.server.Pending())
				{
					return;
				}
				Socket socket = this.server.AcceptSocket();
				if (socket == null)
				{
					return;
				}
				IPEndPoint ipendPoint = socket.RemoteEndPoint as IPEndPoint;
				if (RCon.IsBanned(ipendPoint.Address))
				{
					Debug.Log("[RCON] Ignoring connection - banned. " + ipendPoint.Address.ToString());
					socket.Close();
					return;
				}
				this.clients.Add(new RCon.RConClient(socket));
			}

			// Token: 0x060054FA RID: 21754 RVA: 0x001B6C50 File Offset: 0x001B4E50
			private void UpdateClients()
			{
				foreach (RCon.RConClient rconClient in this.clients)
				{
					rconClient.Update();
				}
			}

			// Token: 0x060054FB RID: 21755 RVA: 0x001B6CA0 File Offset: 0x001B4EA0
			private void RemoveDeadClients()
			{
				this.clients.RemoveAll((RCon.RConClient x) => !x.IsValid());
			}

			// Token: 0x04005051 RID: 20561
			private TcpListener server;

			// Token: 0x04005052 RID: 20562
			private List<RCon.RConClient> clients = new List<RCon.RConClient>();
		}
	}
}
