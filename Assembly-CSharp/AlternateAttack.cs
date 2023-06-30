using System;
using UnityEngine;

// Token: 0x0200096E RID: 2414
public class AlternateAttack : StateMachineBehaviour
{
	// Token: 0x060039C7 RID: 14791 RVA: 0x00156020 File Offset: 0x00154220
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.random)
		{
			string text = this.targetTransitions[UnityEngine.Random.Range(0, this.targetTransitions.Length)];
			animator.Play(text, layerIndex, 0f);
			return;
		}
		int integer = animator.GetInteger("lastAttack");
		string text2 = this.targetTransitions[integer % this.targetTransitions.Length];
		animator.Play(text2, layerIndex, 0f);
		if (!this.dontIncrement)
		{
			animator.SetInteger("lastAttack", integer + 1);
		}
	}

	// Token: 0x0400342B RID: 13355
	public bool random;

	// Token: 0x0400342C RID: 13356
	public bool dontIncrement;

	// Token: 0x0400342D RID: 13357
	public string[] targetTransitions;
}
