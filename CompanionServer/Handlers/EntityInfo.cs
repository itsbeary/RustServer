using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A02 RID: 2562
	public class EntityInfo : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003D2D RID: 15661 RVA: 0x001667B4 File Offset: 0x001649B4
		public override void Execute()
		{
			AppEntityInfo appEntityInfo = Pool.Get<AppEntityInfo>();
			appEntityInfo.type = base.Entity.Type;
			appEntityInfo.payload = Pool.Get<AppEntityPayload>();
			base.Entity.FillEntityPayload(appEntityInfo.payload);
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.entityInfo = appEntityInfo;
			base.Send(appResponse);
		}
	}
}
