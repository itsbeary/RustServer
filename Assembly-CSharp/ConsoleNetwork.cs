using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x0200035E RID: 862
public static class ConsoleNetwork
{
	// Token: 0x06001F90 RID: 8080 RVA: 0x000063A5 File Offset: 0x000045A5
	internal static void Init()
	{
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000D5540 File Offset: 0x000D3740
	internal static void OnClientCommand(Message packet)
	{
		if (packet.read.Unread > ConVar.Server.maxpacketsize_command)
		{
			Debug.LogWarning("Dropping client command due to size");
			return;
		}
		string text = packet.read.StringRaw(8388608U);
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning("Client without connection tried to run command: " + text);
			return;
		}
		string text2 = ConsoleSystem.Run(ConsoleSystem.Option.Server.FromConnection(packet.connection).Quiet(), text, Array.Empty<object>());
		if (!string.IsNullOrEmpty(text2))
		{
			ConsoleNetwork.SendClientReply(packet.connection, text2);
		}
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000D55DC File Offset: 0x000D37DC
	internal static void SendClientReply(Connection cn, string strCommand)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleMessage);
		netWrite.String(strCommand);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000D5610 File Offset: 0x000D3810
	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		string text = ConsoleSystem.BuildCommand(strCommand, args);
		netWrite.String(text);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000D5656 File Offset: 0x000D3856
	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x000D5690 File Offset: 0x000D3890
	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(Network.Net.sv.connections));
	}
}
