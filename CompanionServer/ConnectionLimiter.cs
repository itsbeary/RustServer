using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ConVar;

namespace CompanionServer
{
	// Token: 0x020009EB RID: 2539
	public class ConnectionLimiter
	{
		// Token: 0x06003C96 RID: 15510 RVA: 0x00163B7C File Offset: 0x00161D7C
		public ConnectionLimiter()
		{
			this._sync = new object();
			this._addressCounts = new Dictionary<IPAddress, int>();
			this._overallCount = 0;
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x00163BA4 File Offset: 0x00161DA4
		public bool TryAdd(IPAddress address)
		{
			if (address == null)
			{
				return false;
			}
			object sync = this._sync;
			bool flag2;
			lock (sync)
			{
				if (this._overallCount >= App.maxconnections)
				{
					flag2 = false;
				}
				else
				{
					int num;
					if (this._addressCounts.TryGetValue(address, out num))
					{
						if (num >= App.maxconnectionsperip)
						{
							return false;
						}
						this._addressCounts[address] = num + 1;
					}
					else
					{
						this._addressCounts.Add(address, 1);
					}
					this._overallCount++;
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x06003C98 RID: 15512 RVA: 0x00163C40 File Offset: 0x00161E40
		public void Remove(IPAddress address)
		{
			if (address == null)
			{
				return;
			}
			object sync = this._sync;
			lock (sync)
			{
				int num;
				if (this._addressCounts.TryGetValue(address, out num))
				{
					if (num <= 1)
					{
						this._addressCounts.Remove(address);
					}
					else
					{
						this._addressCounts[address] = num - 1;
					}
					this._overallCount--;
				}
			}
		}

		// Token: 0x06003C99 RID: 15513 RVA: 0x00163CC0 File Offset: 0x00161EC0
		public void Clear()
		{
			object sync = this._sync;
			lock (sync)
			{
				this._addressCounts.Clear();
				this._overallCount = 0;
			}
		}

		// Token: 0x06003C9A RID: 15514 RVA: 0x00163D0C File Offset: 0x00161F0C
		public override string ToString()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "IP", "connections" });
			object sync = this._sync;
			string text;
			lock (sync)
			{
				foreach (KeyValuePair<IPAddress, int> keyValuePair in this._addressCounts.OrderByDescending((KeyValuePair<IPAddress, int> t) => t.Value))
				{
					textTable.AddRow(new string[]
					{
						keyValuePair.Key.ToString(),
						keyValuePair.Value.ToString()
					});
				}
				text = string.Format("{0}\n{1} total", textTable, this._overallCount);
			}
			return text;
		}

		// Token: 0x04003706 RID: 14086
		private readonly object _sync;

		// Token: 0x04003707 RID: 14087
		private readonly Dictionary<IPAddress, int> _addressCounts;

		// Token: 0x04003708 RID: 14088
		private int _overallCount;
	}
}
