using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class AnimationFlagHandler : MonoBehaviour
{
	// Token: 0x06001764 RID: 5988 RVA: 0x000B1ADC File Offset: 0x000AFCDC
	public void SetBoolTrue(string name)
	{
		this.animator.SetBool(name, true);
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x000B1AEB File Offset: 0x000AFCEB
	public void SetBoolFalse(string name)
	{
		this.animator.SetBool(name, false);
	}

	// Token: 0x04000FF7 RID: 4087
	public Animator animator;
}
