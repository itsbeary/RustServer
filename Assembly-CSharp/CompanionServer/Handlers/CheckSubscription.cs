using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A01 RID: 2561
	public class CheckSubscription : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003D2B RID: 15659 RVA: 0x00166770 File Offset: 0x00164970
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = base.Entity as ISubscribable) != null)
			{
				bool flag = subscribable.HasSubscription(base.UserId);
				base.SendFlag(flag);
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
