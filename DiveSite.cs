using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class DiveSite : JunkPile
{
	// Token: 0x0600174A RID: 5962 RVA: 0x000B1239 File Offset: 0x000AF439
	public override float TimeoutPlayerCheckRadius()
	{
		return 40f;
	}

	// Token: 0x04000FEA RID: 4074
	public Transform bobber;
}
