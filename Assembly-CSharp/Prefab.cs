using System;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class Prefab<T> : Prefab, IComparable<Prefab<T>> where T : Component
{
	// Token: 0x06002A53 RID: 10835 RVA: 0x00102358 File Offset: 0x00100558
	public Prefab(string name, GameObject prefab, T component, GameManager manager, PrefabAttribute.Library attribute)
		: base(name, prefab, manager, attribute)
	{
		this.Component = component;
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x0010236D File Offset: 0x0010056D
	public int CompareTo(Prefab<T> that)
	{
		return base.CompareTo(that);
	}

	// Token: 0x040022AC RID: 8876
	public T Component;
}
