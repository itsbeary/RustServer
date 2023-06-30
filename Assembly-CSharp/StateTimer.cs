using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
[Serializable]
public struct StateTimer
{
	// Token: 0x06001BDC RID: 7132 RVA: 0x000C3E69 File Offset: 0x000C2069
	public void Activate(float seconds, Action onFinished = null)
	{
		this.ReleaseTime = Time.time + seconds;
		this.OnFinished = onFinished;
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06001BDD RID: 7133 RVA: 0x000C3E7F File Offset: 0x000C207F
	public bool IsActive
	{
		get
		{
			bool flag = this.ReleaseTime > Time.time;
			if (!flag && this.OnFinished != null)
			{
				this.OnFinished();
				this.OnFinished = null;
			}
			return flag;
		}
	}

	// Token: 0x0400138F RID: 5007
	public float ReleaseTime;

	// Token: 0x04001390 RID: 5008
	public Action OnFinished;
}
