using System;
using UnityEngine;

// Token: 0x020008F2 RID: 2290
public class BaseScriptableObject : ScriptableObject
{
	// Token: 0x060037AF RID: 14255 RVA: 0x0014D120 File Offset: 0x0014B320
	public string LookupFileName()
	{
		return StringPool.Get(this.FilenameStringId);
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x0014D12D File Offset: 0x0014B32D
	public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
	{
		return a == b || (a != null && b != null && a.FilenameStringId == b.FilenameStringId);
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x0014D14B File Offset: 0x0014B34B
	public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
	{
		return !(a == b);
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x0014D157 File Offset: 0x0014B357
	public override int GetHashCode()
	{
		return (int)this.FilenameStringId;
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0014D15F File Offset: 0x0014B35F
	public override bool Equals(object o)
	{
		return o != null && o is BaseScriptableObject && o as BaseScriptableObject == this;
	}

	// Token: 0x04003310 RID: 13072
	[HideInInspector]
	public uint FilenameStringId;
}
