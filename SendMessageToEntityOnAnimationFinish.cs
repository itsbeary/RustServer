using System;
using UnityEngine;

// Token: 0x0200073F RID: 1855
public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
	// Token: 0x06003378 RID: 13176 RVA: 0x0013B4B4 File Offset: 0x001396B4
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (0f + this.repeatRate > Time.time)
		{
			return;
		}
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (stateInfo.normalizedTime < 1f)
		{
			return;
		}
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (i != layerIndex)
			{
				if (animator.IsInTransition(i))
				{
					return;
				}
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (currentAnimatorStateInfo.speed > 0f && currentAnimatorStateInfo.normalizedTime < 1f)
				{
					return;
				}
			}
		}
		BaseEntity baseEntity = animator.gameObject.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.SendMessage(this.messageToSendToEntity, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x04002A53 RID: 10835
	public string messageToSendToEntity;

	// Token: 0x04002A54 RID: 10836
	public float repeatRate = 0.1f;

	// Token: 0x04002A55 RID: 10837
	private const float lastMessageSent = 0f;
}
