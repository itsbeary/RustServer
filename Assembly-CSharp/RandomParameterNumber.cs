using System;
using UnityEngine;

// Token: 0x02000975 RID: 2421
public class RandomParameterNumber : StateMachineBehaviour
{
	// Token: 0x060039D8 RID: 14808 RVA: 0x00156598 File Offset: 0x00154798
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = UnityEngine.Random.Range(this.min, this.max);
		int num2 = 0;
		while (this.last == num && this.preventRepetition && num2 < 100)
		{
			num = UnityEngine.Random.Range(this.min, this.max);
			num2++;
		}
		if (this.isFloat)
		{
			animator.SetFloat(this.parameterName, (float)num);
		}
		else
		{
			animator.SetInteger(this.parameterName, num);
		}
		this.last = num;
	}

	// Token: 0x04003457 RID: 13399
	public string parameterName;

	// Token: 0x04003458 RID: 13400
	public int min;

	// Token: 0x04003459 RID: 13401
	public int max;

	// Token: 0x0400345A RID: 13402
	public bool preventRepetition;

	// Token: 0x0400345B RID: 13403
	public bool isFloat;

	// Token: 0x0400345C RID: 13404
	private int last;
}
