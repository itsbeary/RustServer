using System;
using System.Collections.Generic;

// Token: 0x0200068C RID: 1676
public class PathSequence : PrefabAttribute
{
	// Token: 0x06003012 RID: 12306 RVA: 0x0012110C File Offset: 0x0011F30C
	protected override Type GetIndexedType()
	{
		return typeof(PathSequence);
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
	}
}
