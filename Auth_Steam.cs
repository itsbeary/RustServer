using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;

// Token: 0x02000744 RID: 1860
public static class Auth_Steam
{
	// Token: 0x0600339D RID: 13213 RVA: 0x0013CABF File Offset: 0x0013ACBF
	public static IEnumerator Run(Connection connection)
	{
		connection.authStatus = "";
		if (!PlatformService.Instance.BeginPlayerSession(connection.userid, connection.token))
		{
			ConnectionAuth.Reject(connection, "Steam Auth Failed", null);
			yield break;
		}
		Auth_Steam.waitingList.Add(connection);
		Stopwatch timeout = Stopwatch.StartNew();
		while (timeout.Elapsed.TotalSeconds < 30.0 && connection.active && !(connection.authStatus != ""))
		{
			yield return null;
		}
		Auth_Steam.waitingList.Remove(connection);
		if (!connection.active)
		{
			yield break;
		}
		if (connection.authStatus.Length == 0)
		{
			ConnectionAuth.Reject(connection, "Steam Auth Timeout", null);
			PlatformService.Instance.EndPlayerSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "banned")
		{
			ConnectionAuth.Reject(connection, "Auth: " + connection.authStatus, null);
			PlatformService.Instance.EndPlayerSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "gamebanned")
		{
			ConnectionAuth.Reject(connection, "Steam Auth: " + connection.authStatus, null);
			PlatformService.Instance.EndPlayerSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "vacbanned")
		{
			ConnectionAuth.Reject(connection, "Steam Auth: " + connection.authStatus, null);
			PlatformService.Instance.EndPlayerSession(connection.userid);
			yield break;
		}
		if (connection.authStatus != "ok")
		{
			ConnectionAuth.Reject(connection, "Steam Auth Failed", "Steam Auth Error: " + connection.authStatus);
			PlatformService.Instance.EndPlayerSession(connection.userid);
			yield break;
		}
		string text = (ConVar.Server.censorplayerlist ? RandomUsernames.Get(connection.userid + (ulong)((long)UnityEngine.Random.Range(0, 100000))) : connection.username);
		PlatformService.Instance.UpdatePlayerSession(connection.userid, text);
		yield break;
	}

	// Token: 0x0600339E RID: 13214 RVA: 0x0013CAD0 File Offset: 0x0013ACD0
	public static bool ValidateConnecting(ulong steamid, ulong ownerSteamID, AuthResponse response)
	{
		Connection connection = Auth_Steam.waitingList.Find((Connection x) => x.userid == steamid);
		if (connection == null)
		{
			return false;
		}
		connection.ownerid = ownerSteamID;
		if (ServerUsers.Is(ownerSteamID, ServerUsers.UserGroup.Banned) || ServerUsers.Is(steamid, ServerUsers.UserGroup.Banned))
		{
			connection.authStatus = "banned";
			return true;
		}
		if (response == AuthResponse.OK)
		{
			connection.authStatus = "ok";
			return true;
		}
		if (response == AuthResponse.VACBanned)
		{
			connection.authStatus = "vacbanned";
			return true;
		}
		if (response == AuthResponse.PublisherBanned)
		{
			connection.authStatus = "gamebanned";
			return true;
		}
		if (response == AuthResponse.TimedOut)
		{
			connection.authStatus = "ok";
			return true;
		}
		connection.authStatus = response.ToString();
		return true;
	}

	// Token: 0x04002A6F RID: 10863
	internal static List<Connection> waitingList = new List<Connection>();
}
