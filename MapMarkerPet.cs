using System;

// Token: 0x020001FE RID: 510
public class MapMarkerPet : MapMarker
{
	// Token: 0x06001AC1 RID: 6849 RVA: 0x000B3435 File Offset: 0x000B1635
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000B3444 File Offset: 0x000B1644
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
