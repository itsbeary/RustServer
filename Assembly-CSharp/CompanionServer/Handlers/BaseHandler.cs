using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FD RID: 2557
	public abstract class BaseHandler<T> : IHandler, Pool.IPooled where T : class
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06003D0D RID: 15629 RVA: 0x00166176 File Offset: 0x00164376
		protected virtual double TokenCost
		{
			get
			{
				return 1.0;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06003D0E RID: 15630 RVA: 0x00166181 File Offset: 0x00164381
		// (set) Token: 0x06003D0F RID: 15631 RVA: 0x00166189 File Offset: 0x00164389
		public IConnection Client { get; private set; }

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06003D10 RID: 15632 RVA: 0x00166192 File Offset: 0x00164392
		// (set) Token: 0x06003D11 RID: 15633 RVA: 0x0016619A File Offset: 0x0016439A
		public AppRequest Request { get; private set; }

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06003D12 RID: 15634 RVA: 0x001661A3 File Offset: 0x001643A3
		// (set) Token: 0x06003D13 RID: 15635 RVA: 0x001661AB File Offset: 0x001643AB
		public T Proto { get; private set; }

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06003D14 RID: 15636 RVA: 0x001661B4 File Offset: 0x001643B4
		// (set) Token: 0x06003D15 RID: 15637 RVA: 0x001661BC File Offset: 0x001643BC
		private protected ulong UserId { protected get; private set; }

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06003D16 RID: 15638 RVA: 0x001661C5 File Offset: 0x001643C5
		// (set) Token: 0x06003D17 RID: 15639 RVA: 0x001661CD File Offset: 0x001643CD
		private protected global::BasePlayer Player { protected get; private set; }

		// Token: 0x06003D18 RID: 15640 RVA: 0x001661D6 File Offset: 0x001643D6
		public void Initialize(TokenBucketList<ulong> playerBuckets, IConnection client, AppRequest request, T proto)
		{
			this._playerBuckets = playerBuckets;
			this.Client = client;
			this.Request = request;
			this.Proto = proto;
		}

		// Token: 0x06003D19 RID: 15641 RVA: 0x001661F8 File Offset: 0x001643F8
		public virtual void EnterPool()
		{
			this._playerBuckets = null;
			this.Client = null;
			if (this.Request != null)
			{
				this.Request.Dispose();
				this.Request = null;
			}
			this.Proto = default(T);
			this.UserId = 0UL;
			this.Player = null;
		}

		// Token: 0x06003D1A RID: 15642 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06003D1B RID: 15643 RVA: 0x0016624C File Offset: 0x0016444C
		public virtual ValidationResult Validate()
		{
			bool flag;
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(this.Request.playerId, out flag);
			if (this.Request.playerId == 0UL || this.Request.playerToken != orGenerateAppToken)
			{
				return ValidationResult.NotFound;
			}
			if (flag)
			{
				return ValidationResult.Banned;
			}
			ServerUsers.User user = ServerUsers.Get(this.Request.playerId);
			if (((user != null) ? user.group : ServerUsers.UserGroup.None) == ServerUsers.UserGroup.Banned)
			{
				return ValidationResult.Banned;
			}
			TokenBucketList<ulong> playerBuckets = this._playerBuckets;
			TokenBucket tokenBucket = ((playerBuckets != null) ? playerBuckets.Get(this.Request.playerId) : null);
			if (tokenBucket != null && tokenBucket.TryTake(this.TokenCost))
			{
				this.UserId = this.Request.playerId;
				this.Player = global::BasePlayer.FindByID(this.UserId) ?? global::BasePlayer.FindSleeping(this.UserId);
				this.Client.Subscribe(new PlayerTarget(this.UserId));
				return ValidationResult.Success;
			}
			if (tokenBucket == null || !tokenBucket.IsNaughty)
			{
				return ValidationResult.RateLimit;
			}
			return ValidationResult.Rejected;
		}

		// Token: 0x06003D1C RID: 15644
		public abstract void Execute();

		// Token: 0x06003D1D RID: 15645 RVA: 0x00166340 File Offset: 0x00164540
		protected void SendSuccess()
		{
			AppSuccess appSuccess = Pool.Get<AppSuccess>();
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.success = appSuccess;
			this.Send(appResponse);
		}

		// Token: 0x06003D1E RID: 15646 RVA: 0x00166368 File Offset: 0x00164568
		public void SendError(string code)
		{
			AppError appError = Pool.Get<AppError>();
			appError.error = code;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.error = appError;
			this.Send(appResponse);
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x00166398 File Offset: 0x00164598
		public void SendFlag(bool value)
		{
			AppFlag appFlag = Pool.Get<AppFlag>();
			appFlag.value = value;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.flag = appFlag;
			this.Send(appResponse);
		}

		// Token: 0x06003D20 RID: 15648 RVA: 0x001663C6 File Offset: 0x001645C6
		protected void Send(AppResponse response)
		{
			response.seq = this.Request.seq;
			this.Client.Send(response);
		}

		// Token: 0x0400374C RID: 14156
		private TokenBucketList<ulong> _playerBuckets;
	}
}
