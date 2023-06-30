using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A0B RID: 2571
	public class SetSubscription : BaseEntityHandler<AppFlag>
	{
		// Token: 0x06003D44 RID: 15684 RVA: 0x00166DF0 File Offset: 0x00164FF0
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = base.Entity as ISubscribable) != null)
			{
				if (base.Proto.value)
				{
					if (subscribable.AddSubscription(base.UserId))
					{
						base.SendSuccess();
					}
					else
					{
						base.SendError("too_many_subscribers");
					}
				}
				else
				{
					subscribable.RemoveSubscription(base.UserId);
				}
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
