using System;
using Network;

// Token: 0x0200005E RID: 94
public class CommunityEntity : PointEntity
{
	// Token: 0x060009D3 RID: 2515 RVA: 0x0005C734 File Offset: 0x0005A934
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CommunityEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0005C774 File Offset: 0x0005A974
	public override void InitShared()
	{
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = this;
		}
		else
		{
			CommunityEntity.ClientInstance = this;
		}
		base.InitShared();
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0005C792 File Offset: 0x0005A992
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = null;
			return;
		}
		CommunityEntity.ClientInstance = null;
	}

	// Token: 0x04000687 RID: 1671
	public static CommunityEntity ServerInstance;

	// Token: 0x04000688 RID: 1672
	public static CommunityEntity ClientInstance;
}
