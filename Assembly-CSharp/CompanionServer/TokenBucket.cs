using System;
using Network;

namespace CompanionServer
{
	// Token: 0x020009F8 RID: 2552
	public class TokenBucket
	{
		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06003CEE RID: 15598 RVA: 0x001657CD File Offset: 0x001639CD
		public bool IsFull
		{
			get
			{
				this.Update();
				return this._tokens >= this.Settings.MaxTokens;
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06003CEF RID: 15599 RVA: 0x001657EB File Offset: 0x001639EB
		public bool IsNaughty
		{
			get
			{
				this.Update();
				return this._tokens <= -10.0;
			}
		}

		// Token: 0x06003CF0 RID: 15600 RVA: 0x00165807 File Offset: 0x00163A07
		public void Reset()
		{
			this._lastUpdate = TimeEx.realtimeSinceStartup;
			ITokenBucketSettings settings = this.Settings;
			this._tokens = ((settings != null) ? settings.MaxTokens : 0.0);
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x00165834 File Offset: 0x00163A34
		public bool TryTake(double requestedTokens)
		{
			this.Update();
			if (requestedTokens > this._tokens)
			{
				this._tokens -= 1.0;
				return false;
			}
			this._tokens -= requestedTokens;
			return true;
		}

		// Token: 0x06003CF2 RID: 15602 RVA: 0x0016586C File Offset: 0x00163A6C
		private void Update()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			double num = realtimeSinceStartup - this._lastUpdate;
			this._lastUpdate = realtimeSinceStartup;
			double num2 = num * this.Settings.TokensPerSec;
			this._tokens = Math.Min(this._tokens + num2, this.Settings.MaxTokens);
		}

		// Token: 0x0400373D RID: 14141
		private double _lastUpdate;

		// Token: 0x0400373E RID: 14142
		private double _tokens;

		// Token: 0x0400373F RID: 14143
		public ITokenBucketSettings Settings;
	}
}
