using System;
using System.Collections.Generic;

// Token: 0x0200045A RID: 1114
public class ServerStatistics
{
	// Token: 0x06002516 RID: 9494 RVA: 0x000EAF6E File Offset: 0x000E916E
	public ServerStatistics(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000EAF7D File Offset: 0x000E917D
	public void Init()
	{
		this.storage = ServerStatistics.Get(this.player.userID);
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Save()
	{
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000EAF95 File Offset: 0x000E9195
	public void Add(string name, int val)
	{
		if (this.storage != null)
		{
			this.storage.Add(name, val);
		}
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000EAFAC File Offset: 0x000E91AC
	public static ServerStatistics.Storage Get(ulong id)
	{
		ServerStatistics.Storage storage;
		if (ServerStatistics.players.TryGetValue(id, out storage))
		{
			return storage;
		}
		storage = new ServerStatistics.Storage();
		ServerStatistics.players.Add(id, storage);
		return storage;
	}

	// Token: 0x04001D5D RID: 7517
	private BasePlayer player;

	// Token: 0x04001D5E RID: 7518
	private ServerStatistics.Storage storage;

	// Token: 0x04001D5F RID: 7519
	private static Dictionary<ulong, ServerStatistics.Storage> players = new Dictionary<ulong, ServerStatistics.Storage>();

	// Token: 0x02000D04 RID: 3332
	public class Storage
	{
		// Token: 0x0600502B RID: 20523 RVA: 0x001A8324 File Offset: 0x001A6524
		public int Get(string name)
		{
			int num;
			this.dict.TryGetValue(name, out num);
			return num;
		}

		// Token: 0x0600502C RID: 20524 RVA: 0x001A8344 File Offset: 0x001A6544
		public void Add(string name, int val)
		{
			if (this.dict.ContainsKey(name))
			{
				Dictionary<string, int> dictionary = this.dict;
				dictionary[name] += val;
				return;
			}
			this.dict.Add(name, val);
		}

		// Token: 0x04004683 RID: 18051
		private Dictionary<string, int> dict = new Dictionary<string, int>();
	}
}
