using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009FA RID: 2554
	public class TokenBucketList<TKey> : ITokenBucketSettings
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06003CF6 RID: 15606 RVA: 0x001658B9 File Offset: 0x00163AB9
		public double MaxTokens { get; }

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x001658C1 File Offset: 0x00163AC1
		public double TokensPerSec { get; }

		// Token: 0x06003CF8 RID: 15608 RVA: 0x001658C9 File Offset: 0x00163AC9
		public TokenBucketList(double maxTokens, double tokensPerSec)
		{
			this._buckets = new Dictionary<TKey, TokenBucket>();
			this.MaxTokens = maxTokens;
			this.TokensPerSec = tokensPerSec;
		}

		// Token: 0x06003CF9 RID: 15609 RVA: 0x001658EC File Offset: 0x00163AEC
		public TokenBucket Get(TKey key)
		{
			TokenBucket tokenBucket;
			if (this._buckets.TryGetValue(key, out tokenBucket))
			{
				return tokenBucket;
			}
			TokenBucket tokenBucket2 = Pool.Get<TokenBucket>();
			tokenBucket2.Settings = this;
			tokenBucket2.Reset();
			this._buckets.Add(key, tokenBucket2);
			return tokenBucket2;
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x0016592C File Offset: 0x00163B2C
		public void Cleanup()
		{
			List<TKey> list = Pool.GetList<TKey>();
			foreach (KeyValuePair<TKey, TokenBucket> keyValuePair in this._buckets)
			{
				if (keyValuePair.Value.IsFull)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (TKey tkey in list)
			{
				TokenBucket tokenBucket;
				if (this._buckets.TryGetValue(tkey, out tokenBucket))
				{
					Pool.Free<TokenBucket>(ref tokenBucket);
					this._buckets.Remove(tkey);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}

		// Token: 0x04003740 RID: 14144
		private readonly Dictionary<TKey, TokenBucket> _buckets;
	}
}
