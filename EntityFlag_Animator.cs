using System;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class EntityFlag_Animator : EntityFlag_Toggle
{
	// Token: 0x04001A32 RID: 6706
	public Animator TargetAnimator;

	// Token: 0x04001A33 RID: 6707
	public string ParamName = string.Empty;

	// Token: 0x04001A34 RID: 6708
	public EntityFlag_Animator.AnimatorMode AnimationMode;

	// Token: 0x04001A35 RID: 6709
	public float FloatOnState;

	// Token: 0x04001A36 RID: 6710
	public float FloatOffState;

	// Token: 0x04001A37 RID: 6711
	public int IntegerOnState;

	// Token: 0x04001A38 RID: 6712
	public int IntegerOffState;

	// Token: 0x02000CD7 RID: 3287
	public enum AnimatorMode
	{
		// Token: 0x0400458A RID: 17802
		Bool,
		// Token: 0x0400458B RID: 17803
		Float,
		// Token: 0x0400458C RID: 17804
		Trigger,
		// Token: 0x0400458D RID: 17805
		Integer
	}
}
