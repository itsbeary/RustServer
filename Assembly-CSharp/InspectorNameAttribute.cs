using System;
using UnityEngine;

// Token: 0x020008F4 RID: 2292
public class InspectorNameAttribute : PropertyAttribute
{
	// Token: 0x060037B7 RID: 14263 RVA: 0x0014D1A7 File Offset: 0x0014B3A7
	public InspectorNameAttribute(string name)
	{
		this.name = name;
	}

	// Token: 0x04003311 RID: 13073
	public string name;
}
