using System;
using UnityEngine;

// Token: 0x020004C1 RID: 1217
public abstract class VehicleChassisVisuals<T> : MonoBehaviour where T : BaseVehicle, VehicleChassisVisuals<T>.IClientWheelUser
{
	// Token: 0x02000D32 RID: 3378
	public interface IClientWheelUser
	{
		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x0600507E RID: 20606
		Vector3 Velocity { get; }

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x0600507F RID: 20607
		float DriveWheelVelocity { get; }

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06005080 RID: 20608
		float SteerAngle { get; }

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06005081 RID: 20609
		float MaxSteerAngle { get; }

		// Token: 0x06005082 RID: 20610
		float GetThrottleInput();
	}
}
