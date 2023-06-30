using System;

// Token: 0x020002BB RID: 699
public abstract class DecalComponent : PrefabAttribute
{
	// Token: 0x06001DAE RID: 7598 RVA: 0x000CC3B5 File Offset: 0x000CA5B5
	protected override Type GetIndexedType()
	{
		return typeof(DecalComponent);
	}
}
