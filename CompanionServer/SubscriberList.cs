using System;
using System.Collections.Generic;
using System.Diagnostics;
using Facepunch;
using Network;

namespace CompanionServer
{
	// Token: 0x020009F6 RID: 2550
	public class SubscriberList<TKey, TTarget, TMessage> where TKey : IEquatable<TKey> where TTarget : class
	{
		// Token: 0x06003CE2 RID: 15586 RVA: 0x00164EAC File Offset: 0x001630AC
		public SubscriberList(IBroadcastSender<TTarget, TMessage> sender, double? timeoutSeconds = null)
		{
			this._syncRoot = new object();
			this._subscriptions = new Dictionary<TKey, Dictionary<TTarget, double>>();
			this._sender = sender;
			this._timeoutSeconds = timeoutSeconds;
			this._lastCleanup = Stopwatch.StartNew();
		}

		// Token: 0x06003CE3 RID: 15587 RVA: 0x00164EE4 File Offset: 0x001630E4
		public void Add(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary = new Dictionary<TTarget, double>();
					this._subscriptions.Add(key, dictionary);
				}
				dictionary[value] = TimeEx.realtimeSinceStartup;
			}
			this.CleanupExpired();
		}

		// Token: 0x06003CE4 RID: 15588 RVA: 0x00164F54 File Offset: 0x00163154
		public void Remove(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary.Remove(value);
					if (dictionary.Count == 0)
					{
						this._subscriptions.Remove(key);
					}
				}
			}
			this.CleanupExpired();
		}

		// Token: 0x06003CE5 RID: 15589 RVA: 0x00164FC4 File Offset: 0x001631C4
		public void Clear(TKey key)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary.Clear();
				}
			}
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x00165014 File Offset: 0x00163214
		public void Send(TKey key, TMessage message)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			List<TTarget> list;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					return;
				}
				list = Pool.GetList<TTarget>();
				foreach (KeyValuePair<TTarget, double> keyValuePair in dictionary)
				{
					if (this._timeoutSeconds == null || realtimeSinceStartup - keyValuePair.Value < this._timeoutSeconds.Value)
					{
						list.Add(keyValuePair.Key);
					}
				}
			}
			this._sender.BroadcastTo(list, message);
			Pool.FreeList<TTarget>(ref list);
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x001650EC File Offset: 0x001632EC
		public bool HasAnySubscribers(TKey key)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					return false;
				}
				foreach (KeyValuePair<TTarget, double> keyValuePair in dictionary)
				{
					if (this._timeoutSeconds == null || realtimeSinceStartup - keyValuePair.Value < this._timeoutSeconds.Value)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x001651A4 File Offset: 0x001633A4
		public bool HasSubscriber(TKey key, TTarget target)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				double num;
				if (!this._subscriptions.TryGetValue(key, out dictionary) || !dictionary.TryGetValue(target, out num))
				{
					return false;
				}
				if (this._timeoutSeconds == null || realtimeSinceStartup - num < this._timeoutSeconds.Value)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x0016522C File Offset: 0x0016342C
		private void CleanupExpired()
		{
			if (this._timeoutSeconds == null || this._lastCleanup.Elapsed.TotalMinutes < 2.0)
			{
				return;
			}
			this._lastCleanup.Restart();
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			List<ValueTuple<TKey, TTarget>> list = Pool.GetList<ValueTuple<TKey, TTarget>>();
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				foreach (KeyValuePair<TKey, Dictionary<TTarget, double>> keyValuePair in this._subscriptions)
				{
					foreach (KeyValuePair<TTarget, double> keyValuePair2 in keyValuePair.Value)
					{
						if (realtimeSinceStartup - keyValuePair2.Value >= this._timeoutSeconds.Value)
						{
							list.Add(new ValueTuple<TKey, TTarget>(keyValuePair.Key, keyValuePair2.Key));
						}
					}
				}
				foreach (ValueTuple<TKey, TTarget> valueTuple in list)
				{
					TKey item = valueTuple.Item1;
					TTarget item2 = valueTuple.Item2;
					this.Remove(item, item2);
				}
			}
			Pool.FreeList<ValueTuple<TKey, TTarget>>(ref list);
		}

		// Token: 0x04003738 RID: 14136
		private readonly object _syncRoot;

		// Token: 0x04003739 RID: 14137
		private readonly Dictionary<TKey, Dictionary<TTarget, double>> _subscriptions;

		// Token: 0x0400373A RID: 14138
		private readonly IBroadcastSender<TTarget, TMessage> _sender;

		// Token: 0x0400373B RID: 14139
		private readonly double? _timeoutSeconds;

		// Token: 0x0400373C RID: 14140
		private readonly Stopwatch _lastCleanup;
	}
}
