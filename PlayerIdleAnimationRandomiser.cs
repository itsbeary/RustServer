using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class PlayerIdleAnimationRandomiser : StateMachineBehaviour
{
	// Token: 0x04000042 RID: 66
	public int MaxValue = 3;

	// Token: 0x04000043 RID: 67
	public static int Param_Random = Animator.StringToHash("Random Idle");

	// Token: 0x04000044 RID: 68
	private TimeSince lastRandomisation;
}
