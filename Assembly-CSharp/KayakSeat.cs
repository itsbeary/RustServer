using System;

// Token: 0x02000477 RID: 1143
public class KayakSeat : BaseVehicleSeat
{
	// Token: 0x060025D7 RID: 9687 RVA: 0x000EECAE File Offset: 0x000ECEAE
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerMounted();
		}
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000EECCF File Offset: 0x000ECECF
	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerDismounted(player);
		}
	}

	// Token: 0x04001E01 RID: 7681
	public ItemDefinition PaddleItem;
}
