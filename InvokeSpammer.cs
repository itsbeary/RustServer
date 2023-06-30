using System;
using System.Threading;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class InvokeSpammer : MonoBehaviour
{
	// Token: 0x06001ED8 RID: 7896 RVA: 0x000D206D File Offset: 0x000D026D
	private void Start()
	{
		SingletonComponent<InvokeHandler>.Instance.InvokeRepeating(new Action(this.TestInvoke), this.RepeatTime, this.RepeatTime);
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x000D2091 File Offset: 0x000D0291
	private void TestInvoke()
	{
		Thread.Sleep(this.InvokeMilliseconds);
	}

	// Token: 0x040017C4 RID: 6084
	public int InvokeMilliseconds = 1;

	// Token: 0x040017C5 RID: 6085
	public float RepeatTime = 0.6f;
}
