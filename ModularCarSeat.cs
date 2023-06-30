using System;
using UnityEngine;

// Token: 0x020004A4 RID: 1188
public class ModularCarSeat : MouseSteerableSeat
{
	// Token: 0x0600270E RID: 9998 RVA: 0x000F4880 File Offset: 0x000F2A80
	public override bool CanSwapToThis(BasePlayer player)
	{
		if (this.associatedSeatingModule.DoorsAreLockable)
		{
			ModularCar modularCar = this.associatedSeatingModule.Vehicle as ModularCar;
			if (modularCar != null)
			{
				return modularCar.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
			}
		}
		return true;
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000F48BE File Offset: 0x000F2ABE
	public override float GetComfort()
	{
		return this.providesComfort;
	}

	// Token: 0x04001F65 RID: 8037
	[SerializeField]
	private Vector3 leftFootIKPos;

	// Token: 0x04001F66 RID: 8038
	[SerializeField]
	private Vector3 rightFootIKPos;

	// Token: 0x04001F67 RID: 8039
	[SerializeField]
	private Vector3 leftHandIKPos;

	// Token: 0x04001F68 RID: 8040
	[SerializeField]
	private Vector3 rightHandIKPos;

	// Token: 0x04001F69 RID: 8041
	public float providesComfort;

	// Token: 0x04001F6A RID: 8042
	public VehicleModuleSeating associatedSeatingModule;
}
