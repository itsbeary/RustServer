using System;
using UnityEngine;

// Token: 0x02000732 RID: 1842
public struct MaterialPropertyDesc
{
	// Token: 0x0600334E RID: 13134 RVA: 0x0013A169 File Offset: 0x00138369
	public MaterialPropertyDesc(string name, Type type)
	{
		this.nameID = Shader.PropertyToID(name);
		this.type = type;
	}

	// Token: 0x04002A17 RID: 10775
	public int nameID;

	// Token: 0x04002A18 RID: 10776
	public Type type;
}
