using System;
using UnityEngine.Events;

// Token: 0x020005AF RID: 1455
public class WearableNotifyLifestate : WearableNotify
{
	// Token: 0x040023FC RID: 9212
	public BaseCombatEntity.LifeState TargetState;

	// Token: 0x040023FD RID: 9213
	public UnityEvent OnTargetState = new UnityEvent();

	// Token: 0x040023FE RID: 9214
	public UnityEvent OnTargetStateFailed = new UnityEvent();
}
