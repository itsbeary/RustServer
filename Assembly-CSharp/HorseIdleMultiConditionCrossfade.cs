using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class HorseIdleMultiConditionCrossfade : StateMachineBehaviour
{
	// Token: 0x040013B7 RID: 5047
	public string TargetState = "breathe";

	// Token: 0x040013B8 RID: 5048
	public float NormalizedTransitionDuration = 0.1f;

	// Token: 0x02000C83 RID: 3203
	[Serializable]
	public struct Condition
	{
		// Token: 0x040043F9 RID: 17401
		public int FloatParameter;

		// Token: 0x040043FA RID: 17402
		public HorseIdleMultiConditionCrossfade.Condition.CondtionOperator Operator;

		// Token: 0x040043FB RID: 17403
		public float Value;

		// Token: 0x02000FDD RID: 4061
		public enum CondtionOperator
		{
			// Token: 0x0400517C RID: 20860
			GreaterThan,
			// Token: 0x0400517D RID: 20861
			LessThan
		}
	}
}
