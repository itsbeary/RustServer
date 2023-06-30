using System;
using UnityEngine;

// Token: 0x02000218 RID: 536
[Serializable]
public struct VitalLevel
{
	// Token: 0x06001BDE RID: 7134 RVA: 0x000C3EAB File Offset: 0x000C20AB
	internal void Add(float f)
	{
		this.Level += f;
		if (this.Level > 1f)
		{
			this.Level = 1f;
		}
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001BDF RID: 7135 RVA: 0x000C3EEB File Offset: 0x000C20EB
	public float TimeSinceUsed
	{
		get
		{
			return Time.time - this.lastUsedTime;
		}
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x000C3EFC File Offset: 0x000C20FC
	internal void Use(float f)
	{
		if (Mathf.Approximately(f, 0f))
		{
			return;
		}
		this.Level -= Mathf.Abs(f);
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
		this.lastUsedTime = Time.time;
	}

	// Token: 0x04001391 RID: 5009
	public float Level;

	// Token: 0x04001392 RID: 5010
	private float lastUsedTime;
}
