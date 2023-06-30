using System;
using System.Collections;
using Network;

// Token: 0x02000743 RID: 1859
public static class Auth_EAC
{
	// Token: 0x0600339C RID: 13212 RVA: 0x0013CAB0 File Offset: 0x0013ACB0
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
		connection.authStatus = string.Empty;
		EACServer.OnJoinGame(connection);
		while (connection.active && !connection.rejected && connection.authStatus == string.Empty)
		{
			yield return null;
		}
		yield break;
	}
}
