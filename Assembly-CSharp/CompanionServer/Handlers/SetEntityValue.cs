using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A0A RID: 2570
	public class SetEntityValue : BaseEntityHandler<AppSetEntityValue>
	{
		// Token: 0x06003D42 RID: 15682 RVA: 0x00166DA8 File Offset: 0x00164FA8
		public override void Execute()
		{
			SmartSwitch smartSwitch;
			if ((smartSwitch = base.Entity as SmartSwitch) != null)
			{
				smartSwitch.Value = base.Proto.value;
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
