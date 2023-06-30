using System;
using UnityEngine;

// Token: 0x0200049E RID: 1182
public class ModularCarChassisVisuals : VehicleChassisVisuals<ModularCar>, IClientComponent
{
	// Token: 0x04001F4F RID: 8015
	public Transform frontAxle;

	// Token: 0x04001F50 RID: 8016
	public Transform rearAxle;

	// Token: 0x04001F51 RID: 8017
	public ModularCarChassisVisuals.Steering steering;

	// Token: 0x04001F52 RID: 8018
	public ModularCarChassisVisuals.LookAtTarget transmission;

	// Token: 0x02000D1E RID: 3358
	[Serializable]
	public class Steering
	{
		// Token: 0x040046C9 RID: 18121
		public Transform steerL;

		// Token: 0x040046CA RID: 18122
		public Transform steerR;

		// Token: 0x040046CB RID: 18123
		public ModularCarChassisVisuals.LookAtTarget steerRodL;

		// Token: 0x040046CC RID: 18124
		public ModularCarChassisVisuals.LookAtTarget steerRodR;

		// Token: 0x040046CD RID: 18125
		public ModularCarChassisVisuals.LookAtTarget steeringArm;
	}

	// Token: 0x02000D1F RID: 3359
	[Serializable]
	public class LookAtTarget
	{
		// Token: 0x040046CE RID: 18126
		public Transform aim;

		// Token: 0x040046CF RID: 18127
		public Transform target;

		// Token: 0x040046D0 RID: 18128
		public Vector3 angleAdjust;
	}
}
