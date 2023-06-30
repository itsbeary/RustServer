using System;
using UnityEngine;

// Token: 0x02000580 RID: 1408
public class VehicleSpawnPoint : SpaceCheckingSpawnPoint
{
	// Token: 0x06002B34 RID: 11060 RVA: 0x001068FE File Offset: 0x00104AFE
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		base.ObjectSpawned(instance);
		VehicleSpawnPoint.AddStartingFuel(instance.gameObject.ToBaseEntity() as BaseVehicle);
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x0010691C File Offset: 0x00104B1C
	public static void AddStartingFuel(BaseVehicle vehicle)
	{
		if (vehicle == null)
		{
			return;
		}
		EntityFuelSystem fuelSystem = vehicle.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AddStartingFuel(vehicle.StartingFuelUnits());
		}
	}
}
