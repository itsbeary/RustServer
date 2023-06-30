using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CircularBuffer;
using CompanionServer;
using Facepunch;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB2 RID: 2738
	[ConsoleSystem.Factory("chat")]
	public class Chat : ConsoleSystem
	{
		// Token: 0x06004184 RID: 16772 RVA: 0x00183DF0 File Offset: 0x00181FF0
		public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0UL)
		{
			string text = username.EscapeRichText();
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				string.Concat(new string[] { "<color=", color, ">", text, "</color> ", message })
			});
			Chat.Record(new Chat.ChatEntry
			{
				Channel = Chat.ChatChannel.Server,
				Message = message,
				UserId = userid.ToString(),
				Username = username,
				Color = color,
				Time = Epoch.Current
			});
		}

		// Token: 0x06004185 RID: 16773 RVA: 0x00183EA0 File Offset: 0x001820A0
		[ServerUserVar]
		public static void say(ConsoleSystem.Arg arg)
		{
			if (Chat.globalchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Global, arg);
			}
		}

		// Token: 0x06004186 RID: 16774 RVA: 0x00183EB0 File Offset: 0x001820B0
		[ServerUserVar]
		public static void localsay(ConsoleSystem.Arg arg)
		{
			if (Chat.localchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Local, arg);
			}
		}

		// Token: 0x06004187 RID: 16775 RVA: 0x00183EC0 File Offset: 0x001820C0
		[ServerUserVar]
		public static void teamsay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Team, arg);
		}

		// Token: 0x06004188 RID: 16776 RVA: 0x00183EC9 File Offset: 0x001820C9
		[ServerUserVar]
		public static void cardgamesay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Cards, arg);
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x00183ED4 File Offset: 0x001820D4
		private static void sayImpl(Chat.ChatChannel targetChannel, ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				arg.ReplyWith("Chat is disabled.");
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
			{
				if (basePlayer.NextChatTime == 0f)
				{
					basePlayer.NextChatTime = Time.realtimeSinceStartup - 30f;
				}
				if (basePlayer.NextChatTime > Time.realtimeSinceStartup)
				{
					basePlayer.NextChatTime += 2f;
					float num = basePlayer.NextChatTime - Time.realtimeSinceStartup;
					ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add", new object[]
					{
						2,
						0,
						"You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds"
					});
					if (num > 120f)
					{
						basePlayer.Kick("Chatting too fast");
					}
					return;
				}
			}
			string @string = arg.GetString(0, "text");
			bool flag = Chat.sayAs(targetChannel, basePlayer.userID, basePlayer.displayName, @string, basePlayer);
			Analytics.Azure.OnChatMessage(basePlayer, @string, (int)targetChannel);
			if (flag)
			{
				basePlayer.NextChatTime = Time.realtimeSinceStartup + 1.5f;
			}
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x00184018 File Offset: 0x00182218
		internal static bool sayAs(Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			if (!player)
			{
				player = null;
			}
			if (!Chat.enabled)
			{
				return false;
			}
			if (player != null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return false;
			}
			ServerUsers.User user = ServerUsers.Get(userId);
			ServerUsers.UserGroup userGroup = ((user != null) ? user.group : ServerUsers.UserGroup.None);
			if (userGroup == ServerUsers.UserGroup.Banned)
			{
				return false;
			}
			string text = message.Replace("\n", "").Replace("\r", "").Trim();
			if (text.Length > 128)
			{
				text = text.Substring(0, 128);
			}
			if (text.Length <= 0)
			{
				return false;
			}
			if (text.StartsWith("/") || text.StartsWith("\\"))
			{
				return false;
			}
			text = text.EscapeRichText();
			if (Chat.serverlog)
			{
				ServerConsole.PrintColoured(new object[]
				{
					ConsoleColor.DarkYellow,
					string.Concat(new object[] { "[", targetChannel, "] ", username, ": " }),
					ConsoleColor.DarkGreen,
					text
				});
				string text2 = ((player != null) ? player.ToString() : null) ?? string.Format("{0}[{1}]", username, userId);
				if (targetChannel == Chat.ChatChannel.Team)
				{
					DebugEx.Log("[TEAM CHAT] " + text2 + " : " + text, StackTraceLogType.None);
				}
				else if (targetChannel == Chat.ChatChannel.Cards)
				{
					DebugEx.Log("[CARDS CHAT] " + text2 + " : " + text, StackTraceLogType.None);
				}
				else
				{
					DebugEx.Log("[CHAT] " + text2 + " : " + text, StackTraceLogType.None);
				}
			}
			bool flag = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
			bool flag2 = ((player != null) ? player.IsDeveloper : DeveloperList.Contains(userId));
			string text3 = "#5af";
			if (flag)
			{
				text3 = "#af5";
			}
			if (flag2)
			{
				text3 = "#fa5";
			}
			string text4 = username.EscapeRichText();
			Chat.Record(new Chat.ChatEntry
			{
				Channel = targetChannel,
				Message = text,
				UserId = ((player != null) ? player.UserIDString : userId.ToString()),
				Username = username,
				Color = text3,
				Time = Epoch.Current
			});
			switch (targetChannel)
			{
			case Chat.ChatChannel.Global:
				ConsoleNetwork.BroadcastToAllClients("chat.add2", new object[] { 0, userId, text, text4, text3, 1f });
				return true;
			case Chat.ChatChannel.Team:
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
				if (playerTeam == null)
				{
					return false;
				}
				List<Network.Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
				if (onlineMemberConnections != null)
				{
					ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", new object[] { 1, userId, text, text4, text3, 1f });
				}
				playerTeam.BroadcastTeamChat(userId, text4, text, text3);
				return true;
			}
			case Chat.ChatChannel.Cards:
			{
				if (player == null)
				{
					return false;
				}
				if (!player.isMounted)
				{
					return false;
				}
				BaseCardGameEntity baseCardGameEntity = player.GetMountedVehicle() as BaseCardGameEntity;
				if (baseCardGameEntity == null || !baseCardGameEntity.GameController.IsAtTable(player))
				{
					return false;
				}
				List<Network.Connection> list = Pool.GetList<Network.Connection>();
				baseCardGameEntity.GameController.GetConnectionsInGame(list);
				if (list.Count > 0)
				{
					ConsoleNetwork.SendClientCommand(list, "chat.add2", new object[] { 3, userId, text, text4, text3, 1f });
				}
				Pool.FreeList<Network.Connection>(ref list);
				return true;
			}
			case Chat.ChatChannel.Local:
				if (player != null)
				{
					float num = Chat.localChatRange * Chat.localChatRange;
					foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
					{
						float sqrMagnitude = (basePlayer.transform.position - player.transform.position).sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add2", new object[]
							{
								4,
								userId,
								text,
								text4,
								text3,
								Mathf.Clamp01(sqrMagnitude / num + 0.2f)
							});
						}
					}
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x001844A8 File Offset: 0x001826A8
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Chat.ChatEntry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Chat.History.Size - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Chat.History.Skip(num);
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x001844E0 File Offset: 0x001826E0
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Chat.ChatEntry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Chat.ChatEntry>();
			}
			return Chat.History.Where((Chat.ChatEntry x) => x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase));
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x00184528 File Offset: 0x00182728
		private static void Record(Chat.ChatEntry ce)
		{
			int num = Mathf.Max(Chat.historysize, 10);
			if (Chat.History.Capacity != num)
			{
				CircularBuffer<Chat.ChatEntry> circularBuffer = new CircularBuffer<Chat.ChatEntry>(num);
				foreach (Chat.ChatEntry chatEntry in Chat.History)
				{
					circularBuffer.PushBack(chatEntry);
				}
				Chat.History = circularBuffer;
			}
			Chat.History.PushBack(ce);
			RCon.Broadcast(RCon.LogType.Chat, ce);
		}

		// Token: 0x04003B75 RID: 15221
		[ServerVar]
		public static float localChatRange = 100f;

		// Token: 0x04003B76 RID: 15222
		[ReplicatedVar]
		public static bool globalchat = true;

		// Token: 0x04003B77 RID: 15223
		[ReplicatedVar]
		public static bool localchat = false;

		// Token: 0x04003B78 RID: 15224
		private const float textVolumeBoost = 0.2f;

		// Token: 0x04003B79 RID: 15225
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003B7A RID: 15226
		[ServerVar(Help = "Number of messages to keep in memory for chat history")]
		public static int historysize = 1000;

		// Token: 0x04003B7B RID: 15227
		private static CircularBuffer<Chat.ChatEntry> History = new CircularBuffer<Chat.ChatEntry>(Chat.historysize);

		// Token: 0x04003B7C RID: 15228
		[ServerVar]
		public static bool serverlog = true;

		// Token: 0x02000F5D RID: 3933
		public enum ChatChannel
		{
			// Token: 0x04004FF9 RID: 20473
			Global,
			// Token: 0x04004FFA RID: 20474
			Team,
			// Token: 0x04004FFB RID: 20475
			Server,
			// Token: 0x04004FFC RID: 20476
			Cards,
			// Token: 0x04004FFD RID: 20477
			Local
		}

		// Token: 0x02000F5E RID: 3934
		public struct ChatEntry
		{
			// Token: 0x17000732 RID: 1842
			// (get) Token: 0x06005492 RID: 21650 RVA: 0x001B5CA6 File Offset: 0x001B3EA6
			// (set) Token: 0x06005493 RID: 21651 RVA: 0x001B5CAE File Offset: 0x001B3EAE
			public Chat.ChatChannel Channel { get; set; }

			// Token: 0x17000733 RID: 1843
			// (get) Token: 0x06005494 RID: 21652 RVA: 0x001B5CB7 File Offset: 0x001B3EB7
			// (set) Token: 0x06005495 RID: 21653 RVA: 0x001B5CBF File Offset: 0x001B3EBF
			public string Message { get; set; }

			// Token: 0x17000734 RID: 1844
			// (get) Token: 0x06005496 RID: 21654 RVA: 0x001B5CC8 File Offset: 0x001B3EC8
			// (set) Token: 0x06005497 RID: 21655 RVA: 0x001B5CD0 File Offset: 0x001B3ED0
			public string UserId { get; set; }

			// Token: 0x17000735 RID: 1845
			// (get) Token: 0x06005498 RID: 21656 RVA: 0x001B5CD9 File Offset: 0x001B3ED9
			// (set) Token: 0x06005499 RID: 21657 RVA: 0x001B5CE1 File Offset: 0x001B3EE1
			public string Username { get; set; }

			// Token: 0x17000736 RID: 1846
			// (get) Token: 0x0600549A RID: 21658 RVA: 0x001B5CEA File Offset: 0x001B3EEA
			// (set) Token: 0x0600549B RID: 21659 RVA: 0x001B5CF2 File Offset: 0x001B3EF2
			public string Color { get; set; }

			// Token: 0x17000737 RID: 1847
			// (get) Token: 0x0600549C RID: 21660 RVA: 0x001B5CFB File Offset: 0x001B3EFB
			// (set) Token: 0x0600549D RID: 21661 RVA: 0x001B5D03 File Offset: 0x001B3F03
			public int Time { get; set; }
		}
	}
}
