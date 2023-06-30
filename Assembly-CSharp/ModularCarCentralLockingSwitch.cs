using System;
using UnityEngine;

// Token: 0x0200049D RID: 1181
[Serializable]
public class ModularCarCentralLockingSwitch : VehicleModuleButtonComponent
{
	// Token: 0x060026E7 RID: 9959 RVA: 0x000F4248 File Offset: 0x000F2448
	public override void ServerUse(BasePlayer player, BaseVehicleModule parentModule)
	{
		ModularCar modularCar;
		if ((modularCar = parentModule.Vehicle as ModularCar) != null)
		{
			modularCar.CarLock.ToggleCentralLocking();
		}
	}

	// Token: 0x04001F4C RID: 8012
	public Transform centralLockingSwitch;

	// Token: 0x04001F4D RID: 8013
	public Vector3 switchOffPos;

	// Token: 0x04001F4E RID: 8014
	public Vector3 switchOnPos;
}
