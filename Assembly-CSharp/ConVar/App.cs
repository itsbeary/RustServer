using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CompanionServer;
using Facepunch.Extend;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AAE RID: 2734
	[ConsoleSystem.Factory("app")]
	public class App : ConsoleSystem
	{
		// Token: 0x0600416E RID: 16750 RVA: 0x00183954 File Offset: 0x00181B54
		[ServerUserVar]
		public static async void pair(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!(basePlayer == null))
			{
				Dictionary<string, string> playerPairingData = Util.GetPlayerPairingData(basePlayer);
				NotificationSendResult notificationSendResult = await Util.SendPairNotification("server", basePlayer, Server.hostname.Truncate(128, null), "Tap to pair with this server.", playerPairingData);
				arg.ReplyWith((notificationSendResult == NotificationSendResult.Sent) ? "Sent pairing notification." : notificationSendResult.ToErrorMessage());
			}
		}

		// Token: 0x0600416F RID: 16751 RVA: 0x00183990 File Offset: 0x00181B90
		[ServerUserVar]
		public static void regeneratetoken(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			SingletonComponent<ServerMgr>.Instance.persistance.RegenerateAppToken(basePlayer.userID);
			arg.ReplyWith("Regenerated Rust+ token");
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x001839D0 File Offset: 0x00181BD0
		[ServerVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			if (!Server.IsEnabled)
			{
				arg.ReplyWith("Companion server is not enabled");
				return;
			}
			Listener listener = Server.Listener;
			arg.ReplyWith(string.Format("Server ID: {0}\nListening on: {1}:{2}\nApp connects to: {3}:{4}", new object[]
			{
				App.serverid,
				listener.Address,
				listener.Port,
				App.GetPublicIP(),
				App.port
			}));
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x00183A40 File Offset: 0x00181C40
		[ServerVar]
		public static void resetlimiter(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			ConnectionLimiter limiter = listener.Limiter;
			if (limiter == null)
			{
				return;
			}
			limiter.Clear();
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x00183A5C File Offset: 0x00181C5C
		[ServerVar]
		public static void connections(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			string text;
			if (listener == null)
			{
				text = null;
			}
			else
			{
				ConnectionLimiter limiter = listener.Limiter;
				text = ((limiter != null) ? limiter.ToString() : null);
			}
			string text2 = text ?? "Not available";
			arg.ReplyWith(text2);
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x00183A98 File Offset: 0x00181C98
		[ServerVar]
		public static void appban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appban <steamID64>");
				return;
			}
			string text = (SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, true) ? string.Format("Banned {0} from using the companion app", @ulong) : string.Format("{0} is already banned from using the companion app", @ulong));
			arg.ReplyWith(text);
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x00183AFC File Offset: 0x00181CFC
		[ServerVar]
		public static void appunban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appunban <steamID64>");
				return;
			}
			string text = (SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, false) ? string.Format("Unbanned {0}, they can use the companion app again", @ulong) : string.Format("{0} is not banned from using the companion app", @ulong));
			arg.ReplyWith(text);
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x00183B60 File Offset: 0x00181D60
		public static IPAddress GetListenIP()
		{
			if (string.IsNullOrWhiteSpace(App.listenip))
			{
				return IPAddress.Any;
			}
			IPAddress ipaddress;
			if (!IPAddress.TryParse(App.listenip, out ipaddress) || ipaddress.AddressFamily != AddressFamily.InterNetwork)
			{
				Debug.LogError("Invalid app.listenip: " + App.listenip);
				return IPAddress.Any;
			}
			return ipaddress;
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x00183BB4 File Offset: 0x00181DB4
		public static string GetPublicIP()
		{
			IPAddress ipaddress;
			if (!string.IsNullOrWhiteSpace(App.publicip) && IPAddress.TryParse(App.publicip, out ipaddress) && ipaddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return App.publicip;
			}
			return SteamServer.PublicIp.ToString();
		}

		// Token: 0x04003B53 RID: 15187
		[ServerVar]
		public static string listenip = "";

		// Token: 0x04003B54 RID: 15188
		[ServerVar]
		public static int port;

		// Token: 0x04003B55 RID: 15189
		[ServerVar]
		public static string publicip = "";

		// Token: 0x04003B56 RID: 15190
		[ServerVar(Help = "Disables updating entirely - emergency use only")]
		public static bool update = true;

		// Token: 0x04003B57 RID: 15191
		[ServerVar(Help = "Enables sending push notifications")]
		public static bool notifications = true;

		// Token: 0x04003B58 RID: 15192
		[ServerVar(Help = "Max number of queued messages - set to 0 to disable message processing")]
		public static int queuelimit = 100;

		// Token: 0x04003B59 RID: 15193
		[ReplicatedVar(Default = "")]
		public static string serverid = "";

		// Token: 0x04003B5A RID: 15194
		[ServerVar(Help = "Cooldown time before alarms can send another notification (in seconds)")]
		public static float alarmcooldown = 30f;

		// Token: 0x04003B5B RID: 15195
		[ServerVar]
		public static int maxconnections = 500;

		// Token: 0x04003B5C RID: 15196
		[ServerVar]
		public static int maxconnectionsperip = 5;

		// Token: 0x04003B5D RID: 15197
		[ServerVar]
		public static int maxmessagesize = 1048576;
	}
}
