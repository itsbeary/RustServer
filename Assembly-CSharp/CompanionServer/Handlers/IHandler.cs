using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A04 RID: 2564
	public interface IHandler : Pool.IPooled
	{
		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06003D2F RID: 15663
		AppRequest Request { get; }

		// Token: 0x06003D30 RID: 15664
		ValidationResult Validate();

		// Token: 0x06003D31 RID: 15665
		void Execute();

		// Token: 0x06003D32 RID: 15666
		void SendError(string code);
	}
}
