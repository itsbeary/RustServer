using System;

// Token: 0x02000472 RID: 1138
public class BaseVehicleSeat : BaseVehicleMountPoint
{
	// Token: 0x060025A0 RID: 9632 RVA: 0x000ED3F4 File Offset: 0x000EB5F4
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000ED41C File Offset: 0x000EB61C
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.MounteeTookDamage(mountee, info);
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000ED444 File Offset: 0x000EB644
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.PlayerServerInput(inputState, player);
		}
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000ED474 File Offset: 0x000EB674
	public override void LightToggle(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.LightToggle(player);
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SwitchParent(BaseEntity ent)
	{
	}

	// Token: 0x04001DB8 RID: 7608
	public float mountedAnimationSpeed;

	// Token: 0x04001DB9 RID: 7609
	public bool sendClientInputToVehicleParent;

	// Token: 0x04001DBA RID: 7610
	public bool forcePlayerModelUpdate;
}
