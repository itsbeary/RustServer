using System;

// Token: 0x0200016F RID: 367
public class MapMarkerDeliveryDrone : MapMarker
{
	// Token: 0x0600179D RID: 6045 RVA: 0x000B3435 File Offset: 0x000B1635
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000B3444 File Offset: 0x000B1644
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
