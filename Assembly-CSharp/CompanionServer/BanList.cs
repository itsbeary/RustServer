using System;
using System.Collections.Generic;
using Facepunch;
using Network;

namespace CompanionServer
{
	// Token: 0x020009E6 RID: 2534
	public class BanList<TKey>
	{
		// Token: 0x06003C69 RID: 15465 RVA: 0x00162E39 File Offset: 0x00161039
		public BanList()
		{
			this._bans = new Dictionary<TKey, double>();
		}

		// Token: 0x06003C6A RID: 15466 RVA: 0x00162E4C File Offset: 0x0016104C
		public void Ban(TKey key, double timeInSeconds)
		{
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				double num = TimeEx.realtimeSinceStartup + timeInSeconds;
				double num2;
				if (this._bans.TryGetValue(key, out num2))
				{
					num = Math.Max(num, num2);
				}
				this._bans[key] = num;
			}
		}

		// Token: 0x06003C6B RID: 15467 RVA: 0x00162EB4 File Offset: 0x001610B4
		public bool IsBanned(TKey key)
		{
			Dictionary<TKey, double> bans = this._bans;
			bool flag2;
			lock (bans)
			{
				double num;
				if (!this._bans.TryGetValue(key, out num))
				{
					flag2 = false;
				}
				else if (TimeEx.realtimeSinceStartup < num)
				{
					flag2 = true;
				}
				else
				{
					this._bans.Remove(key);
					flag2 = false;
				}
			}
			return flag2;
		}

		// Token: 0x06003C6C RID: 15468 RVA: 0x00162F20 File Offset: 0x00161120
		public void Cleanup()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			List<TKey> list = Pool.GetList<TKey>();
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				foreach (KeyValuePair<TKey, double> keyValuePair in this._bans)
				{
					if (realtimeSinceStartup >= keyValuePair.Value)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (TKey tkey in list)
				{
					this._bans.Remove(tkey);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}

		// Token: 0x040036F0 RID: 14064
		private readonly Dictionary<TKey, double> _bans;
	}
}
