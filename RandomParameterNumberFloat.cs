using System;
using UnityEngine;

// Token: 0x02000976 RID: 2422
public class RandomParameterNumberFloat : StateMachineBehaviour
{
	// Token: 0x060039DA RID: 14810 RVA: 0x00156613 File Offset: 0x00154813
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (string.IsNullOrEmpty(this.parameterName))
		{
			return;
		}
		animator.SetFloat(this.parameterName, Mathf.Floor(UnityEngine.Random.Range((float)this.min, (float)this.max + 0.5f)));
	}

	// Token: 0x0400345D RID: 13405
	public string parameterName;

	// Token: 0x0400345E RID: 13406
	public int min;

	// Token: 0x0400345F RID: 13407
	public int max;
}
