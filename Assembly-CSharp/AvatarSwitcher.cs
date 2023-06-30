using System;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class AvatarSwitcher : StateMachineBehaviour
{
	// Token: 0x06001D4B RID: 7499 RVA: 0x000CA32D File Offset: 0x000C852D
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (this.ToApply != null)
		{
			animator.avatar = this.ToApply;
			animator.Play(stateInfo.shortNameHash, layerIndex);
		}
	}

	// Token: 0x040015DE RID: 5598
	public Avatar ToApply;
}
