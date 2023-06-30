using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000742 RID: 1858
public static class Auth_CentralizedBans
{
	// Token: 0x06003398 RID: 13208 RVA: 0x0013C845 File Offset: 0x0013AA45
	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active)
		{
			yield break;
		}
		if (connection.rejected)
		{
			yield break;
		}
		if (string.IsNullOrWhiteSpace(ConVar.Server.bansServerEndpoint) || !ConVar.Server.bansServerEndpoint.StartsWith("http"))
		{
			yield break;
		}
		connection.authStatus = "";
		if (!ConVar.Server.bansServerEndpoint.EndsWith("/"))
		{
			ConVar.Server.bansServerEndpoint += "/";
		}
		if (connection.ownerid != 0UL && connection.ownerid != connection.userid)
		{
			string text = ConVar.Server.bansServerEndpoint + connection.ownerid;
			UnityWebRequest ownerRequest = UnityWebRequest.Get(text);
			ownerRequest.timeout = ConVar.Server.bansServerTimeout;
			yield return ownerRequest.SendWebRequest();
			if (Auth_CentralizedBans.CheckIfPlayerBanned(connection.ownerid, connection, ownerRequest))
			{
				yield break;
			}
			ownerRequest = null;
		}
		string text2 = ConVar.Server.bansServerEndpoint + connection.userid;
		UnityWebRequest userRequest = UnityWebRequest.Get(text2);
		userRequest.timeout = ConVar.Server.bansServerTimeout;
		yield return userRequest.SendWebRequest();
		if (Auth_CentralizedBans.CheckIfPlayerBanned(connection.userid, connection, userRequest))
		{
			yield break;
		}
		connection.authStatus = "ok";
		yield break;
	}

	// Token: 0x06003399 RID: 13209 RVA: 0x0013C854 File Offset: 0x0013AA54
	private static bool CheckIfPlayerBanned(ulong steamId, Connection connection, UnityWebRequest request)
	{
		Auth_CentralizedBans.<>c__DisplayClass2_0 CS$<>8__locals1;
		CS$<>8__locals1.connection = connection;
		if (request.isNetworkError)
		{
			Debug.LogError("Failed to check centralized bans due to a network error (" + request.error + ")");
			if (ConVar.Server.bansServerFailureMode == 1)
			{
				Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Network Error", ref CS$<>8__locals1);
				return true;
			}
			return false;
		}
		else
		{
			if (request.responseCode == 404L)
			{
				return false;
			}
			if (!request.isHttpError)
			{
				bool flag;
				try
				{
					Auth_CentralizedBans.payloadData.steamId = 0UL;
					Auth_CentralizedBans.payloadData.reason = null;
					Auth_CentralizedBans.payloadData.expiryDate = 0L;
					JsonUtility.FromJsonOverwrite(request.downloadHandler.text, Auth_CentralizedBans.payloadData);
					if (Auth_CentralizedBans.payloadData.expiryDate > 0L && (long)Epoch.Current >= Auth_CentralizedBans.payloadData.expiryDate)
					{
						flag = false;
					}
					else if (Auth_CentralizedBans.payloadData.steamId != steamId)
					{
						Debug.LogError(string.Format("Failed to check centralized bans due to SteamID mismatch (expected {0}, got {1})", steamId, Auth_CentralizedBans.payloadData.steamId));
						if (ConVar.Server.bansServerFailureMode == 1)
						{
							Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: SteamID Mismatch", ref CS$<>8__locals1);
							flag = true;
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						string text = Auth_CentralizedBans.payloadData.reason ?? "no reason given";
						string text2 = ((Auth_CentralizedBans.payloadData.expiryDate > 0L) ? (" for " + (Auth_CentralizedBans.payloadData.expiryDate - (long)Epoch.Current).FormatSecondsLong()) : "");
						Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0(string.Concat(new string[] { "You are banned from this server", text2, " (", text, ")" }), ref CS$<>8__locals1);
						flag = true;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Failed to check centralized bans due to a malformed response: " + request.downloadHandler.text);
					Debug.LogException(ex);
					if (ConVar.Server.bansServerFailureMode == 1)
					{
						Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Malformed Response", ref CS$<>8__locals1);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				return flag;
			}
			Debug.LogError(string.Format("Failed to check centralized bans due to a server error ({0}: {1})", request.responseCode, request.error));
			if (ConVar.Server.bansServerFailureMode == 1)
			{
				Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Server Error", ref CS$<>8__locals1);
				return true;
			}
			return false;
		}
	}

	// Token: 0x0600339B RID: 13211 RVA: 0x0013CA8C File Offset: 0x0013AC8C
	[CompilerGenerated]
	internal static void <CheckIfPlayerBanned>g__Reject|2_0(string reason, ref Auth_CentralizedBans.<>c__DisplayClass2_0 A_1)
	{
		ConnectionAuth.Reject(A_1.connection, reason, null);
		PlatformService.Instance.EndPlayerSession(A_1.connection.userid);
	}

	// Token: 0x04002A6E RID: 10862
	private static readonly Auth_CentralizedBans.BanPayload payloadData = new Auth_CentralizedBans.BanPayload();

	// Token: 0x02000E57 RID: 3671
	private class BanPayload
	{
		// Token: 0x04004B87 RID: 19335
		public ulong steamId;

		// Token: 0x04004B88 RID: 19336
		public string reason;

		// Token: 0x04004B89 RID: 19337
		public long expiryDate;
	}
}
