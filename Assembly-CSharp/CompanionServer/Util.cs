using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009FB RID: 2555
	public static class Util
	{
		// Token: 0x06003CFB RID: 15611 RVA: 0x00165A00 File Offset: 0x00163C00
		public static Vector2 WorldToMap(Vector3 worldPos)
		{
			return new Vector2(worldPos.x - TerrainMeta.Position.x, worldPos.z - TerrainMeta.Position.z);
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x00165A2C File Offset: 0x00163C2C
		public static void SendSignedInNotification(global::BasePlayer player)
		{
			if (player == null || player.currentTeam == 0UL)
			{
				return;
			}
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(player.currentTeam);
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("type", "login");
			serverPairingData.Add("targetId", player.UserIDString);
			serverPairingData.Add("targetName", player.displayName.Truncate(128, null));
			if (playerTeam == null)
			{
				return;
			}
			playerTeam.SendNotification(NotificationChannel.PlayerLoggedIn, player.displayName + " is now online", ConVar.Server.hostname, serverPairingData, player.userID);
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x00165ACC File Offset: 0x00163CCC
		public static void SendDeathNotification(global::BasePlayer player, global::BaseEntity killer)
		{
			global::BasePlayer basePlayer;
			string text;
			string text2;
			if ((basePlayer = killer as global::BasePlayer) != null && basePlayer.GetType() == typeof(global::BasePlayer))
			{
				text = basePlayer.UserIDString;
				text2 = basePlayer.displayName;
			}
			else
			{
				text = "";
				text2 = killer.ShortPrefabName;
			}
			if (player == null || string.IsNullOrEmpty(text2))
			{
				return;
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("type", "death");
			serverPairingData.Add("targetId", text);
			serverPairingData.Add("targetName", text2.Truncate(128, null));
			NotificationList.SendNotificationTo(player.userID, NotificationChannel.PlayerDied, "You were killed by " + text2, ConVar.Server.hostname, serverPairingData);
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x00165B84 File Offset: 0x00163D84
		public static Task<NotificationSendResult> SendPairNotification(string type, global::BasePlayer player, string title, string message, Dictionary<string, string> data)
		{
			if (!CompanionServer.Server.IsEnabled)
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.Disabled);
			}
			if (!CompanionServer.Server.CanSendPairingNotification(player.userID))
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.RateLimited);
			}
			if (data == null)
			{
				data = Util.GetPlayerPairingData(player);
			}
			data.Add("type", type);
			return NotificationList.SendNotificationTo(player.userID, NotificationChannel.Pairing, title, message, data);
		}

		// Token: 0x06003CFF RID: 15615 RVA: 0x00165BE0 File Offset: 0x00163DE0
		public static Dictionary<string, string> GetServerPairingData()
		{
			Dictionary<string, string> dictionary = Facepunch.Pool.Get<Dictionary<string, string>>();
			dictionary.Clear();
			dictionary.Add("id", App.serverid);
			dictionary.Add("name", ConVar.Server.hostname.Truncate(128, null));
			dictionary.Add("desc", ConVar.Server.description.Truncate(512, null));
			dictionary.Add("img", ConVar.Server.headerimage.Truncate(128, null));
			dictionary.Add("logo", ConVar.Server.logoimage.Truncate(128, null));
			dictionary.Add("url", ConVar.Server.url.Truncate(128, null));
			dictionary.Add("ip", App.GetPublicIP());
			dictionary.Add("port", App.port.ToString("G", CultureInfo.InvariantCulture));
			return dictionary;
		}

		// Token: 0x06003D00 RID: 15616 RVA: 0x00165CC0 File Offset: 0x00163EC0
		public static Dictionary<string, string> GetPlayerPairingData(global::BasePlayer player)
		{
			bool flag;
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(player.userID, out flag);
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("playerId", player.UserIDString);
			serverPairingData.Add("playerToken", orGenerateAppToken.ToString("G", CultureInfo.InvariantCulture));
			return serverPairingData;
		}

		// Token: 0x06003D01 RID: 15617 RVA: 0x00165D18 File Offset: 0x00163F18
		public static void BroadcastAppTeamRemoval(this global::BasePlayer player)
		{
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamChanged = Facepunch.Pool.Get<AppTeamChanged>();
			appBroadcast.teamChanged.playerId = player.userID;
			appBroadcast.teamChanged.teamInfo = player.GetAppTeamInfo(player.userID);
			CompanionServer.Server.Broadcast(new PlayerTarget(player.userID), appBroadcast);
		}

		// Token: 0x06003D02 RID: 15618 RVA: 0x00165D70 File Offset: 0x00163F70
		public static void BroadcastAppTeamUpdate(this global::RelationshipManager.PlayerTeam team)
		{
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamChanged = Facepunch.Pool.Get<AppTeamChanged>();
			appBroadcast.ShouldPool = false;
			foreach (ulong num in team.members)
			{
				appBroadcast.teamChanged.playerId = num;
				appBroadcast.teamChanged.teamInfo = team.GetAppTeamInfo(num);
				CompanionServer.Server.Broadcast(new PlayerTarget(num), appBroadcast);
			}
			appBroadcast.ShouldPool = true;
			appBroadcast.Dispose();
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x00165E0C File Offset: 0x0016400C
		public static void BroadcastTeamChat(this global::RelationshipManager.PlayerTeam team, ulong steamId, string name, string message, string color)
		{
			uint num = (uint)Epoch.Current;
			CompanionServer.Server.TeamChat.Record(team.teamID, steamId, name, message, color, num);
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamMessage = Facepunch.Pool.Get<AppTeamMessage>();
			appBroadcast.teamMessage.message = Facepunch.Pool.Get<AppChatMessage>();
			appBroadcast.ShouldPool = false;
			AppChatMessage message2 = appBroadcast.teamMessage.message;
			message2.steamId = steamId;
			message2.name = name;
			message2.message = message;
			message2.color = color;
			message2.time = num;
			foreach (ulong num2 in team.members)
			{
				CompanionServer.Server.Broadcast(new PlayerTarget(num2), appBroadcast);
			}
			appBroadcast.ShouldPool = true;
			appBroadcast.Dispose();
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x00165EE4 File Offset: 0x001640E4
		public static void SendNotification(this global::RelationshipManager.PlayerTeam team, NotificationChannel channel, string title, string body, Dictionary<string, string> data, ulong ignorePlayer = 0UL)
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (ulong num in team.members)
			{
				if (num != ignorePlayer)
				{
					global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
					if (!(basePlayer == null))
					{
						Networkable net = basePlayer.net;
						if (((net != null) ? net.connection : null) != null)
						{
							continue;
						}
					}
					list.Add(num);
				}
			}
			NotificationList.SendNotificationTo(list, channel, title, body, data);
			Facepunch.Pool.FreeList<ulong>(ref list);
		}

		// Token: 0x06003D05 RID: 15621 RVA: 0x00165F7C File Offset: 0x0016417C
		public static string ToErrorCode(this ValidationResult result)
		{
			switch (result)
			{
			case ValidationResult.NotFound:
				return "not_found";
			case ValidationResult.RateLimit:
				return "rate_limit";
			case ValidationResult.Banned:
				return "banned";
			default:
				return "unknown";
			}
		}

		// Token: 0x06003D06 RID: 15622 RVA: 0x00165FAC File Offset: 0x001641AC
		public static string ToErrorMessage(this NotificationSendResult result)
		{
			switch (result)
			{
			case NotificationSendResult.Sent:
				return null;
			case NotificationSendResult.Empty:
				return Util.NotificationEmpty.translated;
			case NotificationSendResult.Disabled:
				return Util.NotificationDisabled.translated;
			case NotificationSendResult.RateLimited:
				return Util.NotificationRateLimit.translated;
			case NotificationSendResult.ServerError:
				return Util.NotificationServerError.translated;
			case NotificationSendResult.NoTargetsFound:
				return Util.NotificationNoTargets.translated;
			case NotificationSendResult.TooManySubscribers:
				return Util.NotificationTooManySubscribers.translated;
			default:
				return Util.NotificationUnknown.translated;
			}
		}

		// Token: 0x04003743 RID: 14147
		public const int OceanMargin = 500;

		// Token: 0x04003744 RID: 14148
		public static readonly Translate.Phrase NotificationEmpty = new Translate.Phrase("app.error.empty", "Notification was not sent because it was missing some content.");

		// Token: 0x04003745 RID: 14149
		public static readonly Translate.Phrase NotificationDisabled = new Translate.Phrase("app.error.disabled", "Rust+ features are disabled on this server.");

		// Token: 0x04003746 RID: 14150
		public static readonly Translate.Phrase NotificationRateLimit = new Translate.Phrase("app.error.ratelimit", "You are sending too many notifications at a time. Please wait and then try again.");

		// Token: 0x04003747 RID: 14151
		public static readonly Translate.Phrase NotificationServerError = new Translate.Phrase("app.error.servererror", "The companion server failed to send the notification.");

		// Token: 0x04003748 RID: 14152
		public static readonly Translate.Phrase NotificationNoTargets = new Translate.Phrase("app.error.notargets", "Open the Rust+ menu in-game to pair your phone with this server.");

		// Token: 0x04003749 RID: 14153
		public static readonly Translate.Phrase NotificationTooManySubscribers = new Translate.Phrase("app.error.toomanysubs", "There are too many players subscribed to these notifications.");

		// Token: 0x0400374A RID: 14154
		public static readonly Translate.Phrase NotificationUnknown = new Translate.Phrase("app.error.unknown", "An unknown error occurred sending the notification.");
	}
}
