using System;
using CompanionServer.Cameras;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A00 RID: 2560
	public class CameraUnsubscribe : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D29 RID: 15657 RVA: 0x0016673F File Offset: 0x0016493F
		public override void Execute()
		{
			if (!CameraRenderer.enabled)
			{
				base.SendError("not_enabled");
				return;
			}
			base.Client.EndViewing();
			base.SendSuccess();
		}
	}
}
