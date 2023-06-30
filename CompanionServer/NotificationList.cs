using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009F1 RID: 2545
	public class NotificationList
	{
		// Token: 0x06003CB7 RID: 15543 RVA: 0x001646D1 File Offset: 0x001628D1
		public bool AddSubscription(ulong steamId)
		{
			return steamId != 0UL && this._subscriptions.Count < 50 && this._subscriptions.Add(steamId);
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x001646F5 File Offset: 0x001628F5
		public bool RemoveSubscription(ulong steamId)
		{
			return this._subscriptions.Remove(steamId);
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x00164703 File Offset: 0x00162903
		public bool HasSubscription(ulong steamId)
		{
			return this._subscriptions.Contains(steamId);
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x00164714 File Offset: 0x00162914
		public List<ulong> ToList()
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (ulong num in this._subscriptions)
			{
				list.Add(num);
			}
			return list;
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x00164770 File Offset: 0x00162970
		public void LoadFrom(List<ulong> steamIds)
		{
			this._subscriptions.Clear();
			if (steamIds == null)
			{
				return;
			}
			foreach (ulong num in steamIds)
			{
				this._subscriptions.Add(num);
			}
		}

		// Token: 0x06003CBC RID: 15548 RVA: 0x001647D4 File Offset: 0x001629D4
		public void IntersectWith(List<PlayerNameID> players)
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (PlayerNameID playerNameID in players)
			{
				list.Add(playerNameID.userid);
			}
			this._subscriptions.IntersectWith(list);
			Facepunch.Pool.FreeList<ulong>(ref list);
		}

		// Token: 0x06003CBD RID: 15549 RVA: 0x00164840 File Offset: 0x00162A40
		public Task<NotificationSendResult> SendNotification(NotificationChannel channel, string title, string body, string type)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (realtimeSinceStartup - this._lastSend < 15.0)
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.RateLimited);
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			if (!string.IsNullOrWhiteSpace(type))
			{
				serverPairingData["type"] = type;
			}
			this._lastSend = realtimeSinceStartup;
			return NotificationList.SendNotificationImpl(this._subscriptions, channel, title, body, serverPairingData);
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x001648A0 File Offset: 0x00162AA0
		public static async Task<NotificationSendResult> SendNotificationTo(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult notificationSendResult = await NotificationList.SendNotificationImpl(steamIds, channel, title, body, data);
			if (notificationSendResult == NotificationSendResult.NoTargetsFound)
			{
				notificationSendResult = NotificationSendResult.Sent;
			}
			return notificationSendResult;
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x00164908 File Offset: 0x00162B08
		public static async Task<NotificationSendResult> SendNotificationTo(ulong steamId, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			HashSet<ulong> set = Facepunch.Pool.Get<HashSet<ulong>>();
			set.Clear();
			set.Add(steamId);
			NotificationSendResult notificationSendResult = await NotificationList.SendNotificationImpl(set, channel, title, body, data);
			set.Clear();
			Facepunch.Pool.Free<HashSet<ulong>>(ref set);
			return notificationSendResult;
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x00164970 File Offset: 0x00162B70
		private static async Task<NotificationSendResult> SendNotificationImpl(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult notificationSendResult;
			if (!CompanionServer.Server.IsEnabled || !App.notifications)
			{
				notificationSendResult = NotificationSendResult.Disabled;
			}
			else if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
			{
				notificationSendResult = NotificationSendResult.Empty;
			}
			else if (steamIds.Count == 0)
			{
				notificationSendResult = NotificationSendResult.Sent;
			}
			else
			{
				PushRequest pushRequest = Facepunch.Pool.Get<PushRequest>();
				pushRequest.ServerToken = CompanionServer.Server.Token;
				pushRequest.Channel = channel;
				pushRequest.Title = title;
				pushRequest.Body = body;
				pushRequest.Data = data;
				pushRequest.SteamIds = Facepunch.Pool.GetList<ulong>();
				foreach (ulong num in steamIds)
				{
					pushRequest.SteamIds.Add(num);
				}
				string text = JsonConvert.SerializeObject(pushRequest);
				Facepunch.Pool.Free<PushRequest>(ref pushRequest);
				try
				{
					StringContent stringContent = new StringContent(text, Encoding.UTF8, "application/json");
					HttpResponseMessage httpResponseMessage = await NotificationList.Http.PostAsync("https://companion-rust.facepunch.com/api/push/send", stringContent);
					if (!httpResponseMessage.IsSuccessStatusCode)
					{
						DebugEx.LogWarning(string.Format("Failed to send notification: {0}", httpResponseMessage.StatusCode), StackTraceLogType.None);
						notificationSendResult = NotificationSendResult.ServerError;
					}
					else if (httpResponseMessage.StatusCode == HttpStatusCode.Accepted)
					{
						notificationSendResult = NotificationSendResult.NoTargetsFound;
					}
					else
					{
						notificationSendResult = NotificationSendResult.Sent;
					}
				}
				catch (Exception ex)
				{
					DebugEx.LogWarning(string.Format("Exception thrown when sending notification: {0}", ex), StackTraceLogType.None);
					notificationSendResult = NotificationSendResult.Failed;
				}
			}
			return notificationSendResult;
		}

		// Token: 0x04003728 RID: 14120
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/push/send";

		// Token: 0x04003729 RID: 14121
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x0400372A RID: 14122
		private readonly HashSet<ulong> _subscriptions = new HashSet<ulong>();

		// Token: 0x0400372B RID: 14123
		private double _lastSend;
	}
}
