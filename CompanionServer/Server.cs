using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009F4 RID: 2548
	public static class Server
	{
		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06003CCD RID: 15565 RVA: 0x00164AB2 File Offset: 0x00162CB2
		// (set) Token: 0x06003CCE RID: 15566 RVA: 0x00164AB9 File Offset: 0x00162CB9
		public static Listener Listener { get; private set; }

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06003CCF RID: 15567 RVA: 0x00164AC1 File Offset: 0x00162CC1
		public static bool IsEnabled
		{
			get
			{
				return App.port >= 0 && !string.IsNullOrWhiteSpace(App.serverid) && Server.Listener != null;
			}
		}

		// Token: 0x06003CD0 RID: 15568 RVA: 0x00164AE4 File Offset: 0x00162CE4
		public static void Initialize()
		{
			if (App.port < 0)
			{
				return;
			}
			if (Server.IsEnabled)
			{
				UnityEngine.Debug.LogWarning("Rust+ is already started up! Skipping second startup");
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (activeGameMode != null && !activeGameMode.rustPlus)
			{
				return;
			}
			Map.PopulateCache();
			if (App.port == 0)
			{
				App.port = Math.Max(Server.port, RCon.Port) + 67;
			}
			try
			{
				Server.Listener = new Listener(App.GetListenIP(), App.port);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("Companion server failed to start: {0}", ex));
			}
			Server.PostInitializeServer();
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x00164B88 File Offset: 0x00162D88
		public static void Shutdown()
		{
			Server.SetServerId(null);
			Listener listener = Server.Listener;
			if (listener != null)
			{
				listener.Dispose();
			}
			Server.Listener = null;
		}

		// Token: 0x06003CD2 RID: 15570 RVA: 0x00164BA6 File Offset: 0x00162DA6
		public static void Update()
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			listener.Update();
		}

		// Token: 0x06003CD3 RID: 15571 RVA: 0x00164BB7 File Offset: 0x00162DB7
		public static void Broadcast(PlayerTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<PlayerTarget, Connection, AppBroadcast> playerSubscribers = listener.PlayerSubscribers;
			if (playerSubscribers == null)
			{
				return;
			}
			playerSubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x00164BD4 File Offset: 0x00162DD4
		public static void Broadcast(EntityTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<EntityTarget, Connection, AppBroadcast> entitySubscribers = listener.EntitySubscribers;
			if (entitySubscribers == null)
			{
				return;
			}
			entitySubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x00164BF1 File Offset: 0x00162DF1
		public static void Broadcast(CameraTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<CameraTarget, Connection, AppBroadcast> cameraSubscribers = listener.CameraSubscribers;
			if (cameraSubscribers == null)
			{
				return;
			}
			cameraSubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x00164C10 File Offset: 0x00162E10
		public static bool HasAnySubscribers(CameraTarget target)
		{
			Listener listener = Server.Listener;
			bool? flag;
			if (listener == null)
			{
				flag = null;
			}
			else
			{
				SubscriberList<CameraTarget, Connection, AppBroadcast> cameraSubscribers = listener.CameraSubscribers;
				flag = ((cameraSubscribers != null) ? new bool?(cameraSubscribers.HasAnySubscribers(target)) : null);
			}
			return flag ?? false;
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x00164C63 File Offset: 0x00162E63
		public static bool CanSendPairingNotification(ulong playerId)
		{
			Listener listener = Server.Listener;
			return listener != null && listener.CanSendPairingNotification(playerId);
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x00164C78 File Offset: 0x00162E78
		private static async void PostInitializeServer()
		{
			await Server.SetupServerRegistration();
			await Server.CheckConnectivity();
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x00164CAC File Offset: 0x00162EAC
		private static async Task SetupServerRegistration()
		{
			try
			{
				string text;
				string text2;
				if (Server.TryLoadServerRegistration(out text, out text2))
				{
					StringContent stringContent = new StringContent(text2, Encoding.UTF8, "text/plain");
					HttpResponseMessage httpResponseMessage = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server/refresh", stringContent);
					if (httpResponseMessage.IsSuccessStatusCode)
					{
						Server.SetServerRegistration(await httpResponseMessage.Content.ReadAsStringAsync());
						return;
					}
					UnityEngine.Debug.LogWarning("Failed to refresh server ID - registering a new one");
				}
				Server.SetServerRegistration(await Server.Http.GetStringAsync("https://companion-rust.facepunch.com/api/server/register"));
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to setup companion server registration: {0}", ex));
			}
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x00164CEC File Offset: 0x00162EEC
		private static bool TryLoadServerRegistration(out string serverId, out string serverToken)
		{
			serverId = null;
			serverToken = null;
			string serverIdPath = Server.GetServerIdPath();
			if (!File.Exists(serverIdPath))
			{
				return false;
			}
			bool flag;
			try
			{
				Server.RegisterResponse registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(File.ReadAllText(serverIdPath));
				serverId = registerResponse.ServerId;
				serverToken = registerResponse.ServerToken;
				flag = true;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to load companion server registration: {0}", ex));
				flag = false;
			}
			return flag;
		}

		// Token: 0x06003CDB RID: 15579 RVA: 0x00164D58 File Offset: 0x00162F58
		private static void SetServerRegistration(string responseJson)
		{
			Server.RegisterResponse registerResponse = null;
			try
			{
				registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(responseJson);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to parse registration response JSON: {0}\n\n{1}", responseJson, ex));
			}
			Server.SetServerId((registerResponse != null) ? registerResponse.ServerId : null);
			Server.Token = ((registerResponse != null) ? registerResponse.ServerToken : null);
			if (registerResponse == null)
			{
				return;
			}
			try
			{
				File.WriteAllText(Server.GetServerIdPath(), responseJson);
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.LogError(string.Format("Unable to save companion app server registration - server ID may be different after restart: {0}", ex2));
			}
		}

		// Token: 0x06003CDC RID: 15580 RVA: 0x00164DE8 File Offset: 0x00162FE8
		private static async Task CheckConnectivity()
		{
			if (!Server.IsEnabled)
			{
				Server.SetServerId(null);
			}
			else
			{
				try
				{
					string text = await Server.GetPublicIPAsync();
					StringContent stringContent = new StringContent("", Encoding.UTF8, "text/plain");
					HttpResponseMessage testResponse = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server" + string.Format("/test_connection?address={0}&port={1}", text, App.port), stringContent);
					string text2 = await testResponse.Content.ReadAsStringAsync();
					Server.TestConnectionResponse testConnectionResponse = null;
					try
					{
						testConnectionResponse = JsonConvert.DeserializeObject<Server.TestConnectionResponse>(text2);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogError(string.Format("Failed to parse connectivity test response JSON: {0}\n\n{1}", text2, ex));
					}
					if (testConnectionResponse != null)
					{
						string text3 = string.Join("\n", testConnectionResponse.Messages ?? Enumerable.Empty<string>());
						if (testResponse.StatusCode == (HttpStatusCode)555)
						{
							UnityEngine.Debug.LogError("Rust+ companion server connectivity test failed! Disabling Rust+ features.\n\n" + text3);
							Server.SetServerId(null);
						}
						else
						{
							testResponse.EnsureSuccessStatusCode();
							if (!string.IsNullOrWhiteSpace(text3))
							{
								UnityEngine.Debug.LogWarning("Rust+ companion server connectivity test has warnings:\n" + text3);
							}
						}
					}
					testResponse = null;
				}
				catch (Exception ex2)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to check connectivity to the companion server: {0}", ex2));
				}
			}
		}

		// Token: 0x06003CDD RID: 15581 RVA: 0x00164E28 File Offset: 0x00163028
		private static async Task<string> GetPublicIPAsync()
		{
			Stopwatch timer = Stopwatch.StartNew();
			string publicIP;
			for (;;)
			{
				bool flag = timer.Elapsed.TotalMinutes > 2.0;
				publicIP = App.GetPublicIP();
				if (flag || (!string.IsNullOrWhiteSpace(publicIP) && publicIP != "0.0.0.0"))
				{
					break;
				}
				await Task.Delay(10000);
			}
			return publicIP;
		}

		// Token: 0x06003CDE RID: 15582 RVA: 0x00164E65 File Offset: 0x00163065
		private static void SetServerId(string serverId)
		{
			ConsoleSystem.Command command = ConsoleSystem.Index.Server.Find("app.serverid");
			if (command == null)
			{
				return;
			}
			command.Set(serverId ?? "");
		}

		// Token: 0x06003CDF RID: 15583 RVA: 0x00164E85 File Offset: 0x00163085
		private static string GetServerIdPath()
		{
			return Path.Combine(Server.rootFolder, "companion.id");
		}

		// Token: 0x04003733 RID: 14131
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/server";

		// Token: 0x04003734 RID: 14132
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x04003735 RID: 14133
		internal static readonly ChatLog TeamChat = new ChatLog();

		// Token: 0x04003736 RID: 14134
		internal static string Token;

		// Token: 0x02000F04 RID: 3844
		private class RegisterResponse
		{
			// Token: 0x04004E6D RID: 20077
			public string ServerId;

			// Token: 0x04004E6E RID: 20078
			public string ServerToken;
		}

		// Token: 0x02000F05 RID: 3845
		private class TestConnectionResponse
		{
			// Token: 0x04004E6F RID: 20079
			public List<string> Messages;
		}
	}
}
