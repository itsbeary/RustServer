using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009E9 RID: 2537
	public class ChatLog
	{
		// Token: 0x06003C7E RID: 15486 RVA: 0x001634F4 File Offset: 0x001616F4
		public void Record(ulong teamId, ulong steamId, string name, string message, string color, uint time)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				chatState = Pool.Get<ChatLog.ChatState>();
				chatState.History = Pool.GetList<ChatLog.Entry>();
				this.States.Add(teamId, chatState);
			}
			while (chatState.History.Count >= 20)
			{
				chatState.History.RemoveAt(0);
			}
			chatState.History.Add(new ChatLog.Entry
			{
				SteamId = steamId,
				Name = name,
				Message = message,
				Color = color,
				Time = time
			});
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x0016358C File Offset: 0x0016178C
		public void Remove(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return;
			}
			this.States.Remove(teamId);
			Pool.Free<ChatLog.ChatState>(ref chatState);
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x001635C0 File Offset: 0x001617C0
		public IReadOnlyList<ChatLog.Entry> GetHistory(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return null;
			}
			return chatState.History;
		}

		// Token: 0x040036FA RID: 14074
		private const int MaxBacklog = 20;

		// Token: 0x040036FB RID: 14075
		private readonly Dictionary<ulong, ChatLog.ChatState> States = new Dictionary<ulong, ChatLog.ChatState>();

		// Token: 0x02000EF9 RID: 3833
		public struct Entry
		{
			// Token: 0x04004E31 RID: 20017
			public ulong SteamId;

			// Token: 0x04004E32 RID: 20018
			public string Name;

			// Token: 0x04004E33 RID: 20019
			public string Message;

			// Token: 0x04004E34 RID: 20020
			public string Color;

			// Token: 0x04004E35 RID: 20021
			public uint Time;
		}

		// Token: 0x02000EFA RID: 3834
		private class ChatState : Pool.IPooled
		{
			// Token: 0x060053ED RID: 21485 RVA: 0x001B3E91 File Offset: 0x001B2091
			public void EnterPool()
			{
				if (this.History != null)
				{
					Pool.FreeList<ChatLog.Entry>(ref this.History);
				}
			}

			// Token: 0x060053EE RID: 21486 RVA: 0x000063A5 File Offset: 0x000045A5
			public void LeavePool()
			{
			}

			// Token: 0x04004E36 RID: 20022
			public List<ChatLog.Entry> History;
		}
	}
}
