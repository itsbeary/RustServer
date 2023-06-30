using System;

// Token: 0x02000493 RID: 1171
public class MapMarkerMLRSRocket : MapMarker
{
	// Token: 0x06002692 RID: 9874 RVA: 0x000B3435 File Offset: 0x000B1635
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000B3444 File Offset: 0x000B1644
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
