using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x02000746 RID: 1862
public class ConnectionQueue
{
	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x060033A8 RID: 13224 RVA: 0x0013CE1D File Offset: 0x0013B01D
	public int Queued
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x060033A9 RID: 13225 RVA: 0x0013CE2A File Offset: 0x0013B02A
	public int Joining
	{
		get
		{
			return this.joining.Count;
		}
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x0013CE38 File Offset: 0x0013B038
	public void SkipQueue(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			Connection connection = this.queue[i];
			if (connection.userid == userid)
			{
				this.JoinGame(connection);
				return;
			}
		}
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x0013CE79 File Offset: 0x0013B079
	internal void Join(Connection connection)
	{
		connection.state = Connection.State.InQueue;
		this.queue.Add(connection);
		this.nextMessageTime = 0f;
		if (this.CanJumpQueue(connection))
		{
			this.JoinGame(connection);
		}
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x0013CEA9 File Offset: 0x0013B0A9
	public void Cycle(int availableSlots)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		if (availableSlots - this.Joining > 0)
		{
			this.JoinGame(this.queue[0]);
		}
		this.SendMessages();
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x0013CEDC File Offset: 0x0013B0DC
	private void SendMessages()
	{
		if (this.nextMessageTime > Time.realtimeSinceStartup)
		{
			return;
		}
		this.nextMessageTime = Time.realtimeSinceStartup + 10f;
		for (int i = 0; i < this.queue.Count; i++)
		{
			this.SendMessage(this.queue[i], i);
		}
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x0013CF34 File Offset: 0x0013B134
	private void SendMessage(Connection c, int position)
	{
		string text = string.Empty;
		if (position > 0)
		{
			text = string.Format("{0:N0} PLAYERS AHEAD OF YOU, {1:N0} PLAYERS BEHIND", position, this.queue.Count - position - 1);
		}
		else
		{
			text = string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", position, this.queue.Count - position - 1);
		}
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Message);
		netWrite.String("QUEUE");
		netWrite.String(text);
		netWrite.Send(new SendInfo(c));
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x0013CFC6 File Offset: 0x0013B1C6
	public void RemoveConnection(Connection connection)
	{
		if (this.queue.Remove(connection))
		{
			this.nextMessageTime = 0f;
		}
		this.joining.Remove(connection);
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x0013CFEE File Offset: 0x0013B1EE
	private void JoinGame(Connection connection)
	{
		this.queue.Remove(connection);
		connection.state = Connection.State.Welcoming;
		this.nextMessageTime = 0f;
		this.joining.Add(connection);
		SingletonComponent<ServerMgr>.Instance.JoinGame(connection);
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x0013D026 File Offset: 0x0013B226
	public void JoinedGame(Connection connection)
	{
		this.RemoveConnection(connection);
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x0013D030 File Offset: 0x0013B230
	private bool CanJumpQueue(Connection connection)
	{
		if (DeveloperList.Contains(connection.userid))
		{
			return true;
		}
		ServerUsers.User user = ServerUsers.Get(connection.userid);
		return (user != null && user.group == ServerUsers.UserGroup.Moderator) || (user != null && user.group == ServerUsers.UserGroup.Owner) || (user != null && user.group == ServerUsers.UserGroup.SkipQueue);
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x0013D084 File Offset: 0x0013B284
	public bool IsQueued(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			if (this.queue[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x0013D0C0 File Offset: 0x0013B2C0
	public bool IsJoining(ulong userid)
	{
		for (int i = 0; i < this.joining.Count; i++)
		{
			if (this.joining[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002A71 RID: 10865
	private List<Connection> queue = new List<Connection>();

	// Token: 0x04002A72 RID: 10866
	private List<Connection> joining = new List<Connection>();

	// Token: 0x04002A73 RID: 10867
	private float nextMessageTime;
}
