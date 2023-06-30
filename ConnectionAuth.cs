using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;

// Token: 0x02000745 RID: 1861
public class ConnectionAuth : MonoBehaviour
{
	// Token: 0x060033A0 RID: 13216 RVA: 0x0013CB95 File Offset: 0x0013AD95
	public bool IsAuthed(ulong iSteamID)
	{
		return BasePlayer.FindByID(iSteamID) || SingletonComponent<ServerMgr>.Instance.connectionQueue.IsJoining(iSteamID) || SingletonComponent<ServerMgr>.Instance.connectionQueue.IsQueued(iSteamID);
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x0013CBCF File Offset: 0x0013ADCF
	public static void Reject(Connection connection, string strReason, string strReasonPrivate = null)
	{
		DebugEx.Log(connection.ToString() + " Rejecting connection - " + (string.IsNullOrEmpty(strReasonPrivate) ? strReason : strReasonPrivate), StackTraceLogType.None);
		Net.sv.Kick(connection, strReason, false);
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x0013CC0C File Offset: 0x0013AE0C
	public static void OnDisconnect(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x0013CC1A File Offset: 0x0013AE1A
	public void Approve(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
		SingletonComponent<ServerMgr>.Instance.connectionQueue.Join(connection);
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x0013CC38 File Offset: 0x0013AE38
	public void OnNewConnection(Connection connection)
	{
		connection.connected = false;
		if (connection.token == null || connection.token.Length < 32)
		{
			ConnectionAuth.Reject(connection, "Invalid Token", null);
			return;
		}
		if (connection.userid == 0UL)
		{
			ConnectionAuth.Reject(connection, "Invalid SteamID", null);
			return;
		}
		if (connection.protocol != 2397U)
		{
			if (!DeveloperList.Contains(connection.userid))
			{
				ConnectionAuth.Reject(connection, "Incompatible Version", null);
				return;
			}
			DebugEx.Log("Not kicking " + connection.userid + " for incompatible protocol (is a developer)", StackTraceLogType.None);
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Banned))
		{
			ServerUsers.User user = ServerUsers.Get(connection.userid);
			string text = ((user != null) ? user.notes : null) ?? "no reason given";
			string text2 = ((user != null && user.expiry > 0L) ? (" for " + (user.expiry - (long)Epoch.Current).FormatSecondsLong()) : "");
			ConnectionAuth.Reject(connection, string.Concat(new string[] { "You are banned from this server", text2, " (", text, ")" }), null);
			return;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Moderator))
		{
			DebugEx.Log(connection.ToString() + " has auth level 1", StackTraceLogType.None);
			connection.authLevel = 1U;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Owner))
		{
			DebugEx.Log(connection.ToString() + " has auth level 2", StackTraceLogType.None);
			connection.authLevel = 2U;
		}
		if (DeveloperList.Contains(connection.userid))
		{
			DebugEx.Log(connection.ToString() + " is a developer", StackTraceLogType.None);
			connection.authLevel = 3U;
		}
		ConnectionAuth.m_AuthConnection.Add(connection);
		base.StartCoroutine(this.AuthorisationRoutine(connection));
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x0013CDFB File Offset: 0x0013AFFB
	public IEnumerator AuthorisationRoutine(Connection connection)
	{
		yield return base.StartCoroutine(Auth_Steam.Run(connection));
		yield return base.StartCoroutine(Auth_EAC.Run(connection));
		yield return base.StartCoroutine(Auth_CentralizedBans.Run(connection));
		if (connection.rejected || !connection.active)
		{
			yield break;
		}
		if (this.IsAuthed(connection.userid))
		{
			ConnectionAuth.Reject(connection, "You are already connected as a player!", null);
			yield break;
		}
		this.Approve(connection);
		yield break;
	}

	// Token: 0x04002A70 RID: 10864
	[NonSerialized]
	public static List<Connection> m_AuthConnection = new List<Connection>();
}
