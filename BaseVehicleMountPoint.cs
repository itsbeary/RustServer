using System;

// Token: 0x02000471 RID: 1137
public class BaseVehicleMountPoint : BaseMountable
{
	// Token: 0x0600259A RID: 9626 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool DirectlyMountable()
	{
		return false;
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000ED32C File Offset: 0x000EB52C
	public override BaseVehicle VehicleParent()
	{
		BaseVehicle baseVehicle = base.GetParentEntity() as BaseVehicle;
		while (baseVehicle != null && !baseVehicle.IsVehicleRoot())
		{
			BaseVehicle baseVehicle2 = baseVehicle.GetParentEntity() as BaseVehicle;
			if (baseVehicle2 == null)
			{
				return baseVehicle;
			}
			baseVehicle = baseVehicle2;
		}
		return baseVehicle;
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000ED374 File Offset: 0x000EB574
	public override bool BlocksWaterFor(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle == null) && baseVehicle.BlocksWaterFor(player);
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000ED39C File Offset: 0x000EB59C
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.WaterFactorForPlayer(player);
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000ED3C8 File Offset: 0x000EB5C8
	public override float AirFactor()
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.AirFactor();
	}
}
