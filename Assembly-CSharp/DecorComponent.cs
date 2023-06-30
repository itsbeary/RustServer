using System;
using UnityEngine;

// Token: 0x02000663 RID: 1635
public abstract class DecorComponent : PrefabAttribute
{
	// Token: 0x06002FB8 RID: 12216
	public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

	// Token: 0x06002FB9 RID: 12217 RVA: 0x0011FA9E File Offset: 0x0011DC9E
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isRoot = rootObj == base.gameObject;
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x0011FABF File Offset: 0x0011DCBF
	protected override Type GetIndexedType()
	{
		return typeof(DecorComponent);
	}

	// Token: 0x0400272E RID: 10030
	internal bool isRoot;
}
