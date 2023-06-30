using System;
using System.Collections.Generic;

namespace CompanionServer
{
	// Token: 0x020009F5 RID: 2549
	public interface IBroadcastSender<TTarget, TMessage> where TTarget : class
	{
		// Token: 0x06003CE1 RID: 15585
		void BroadcastTo(List<TTarget> targets, TMessage message);
	}
}
