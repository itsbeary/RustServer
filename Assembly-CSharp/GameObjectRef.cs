using System;
using Facepunch;
using UnityEngine;

// Token: 0x0200096A RID: 2410
[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
	// Token: 0x060039BD RID: 14781 RVA: 0x00155F8A File Offset: 0x0015418A
	public GameObject Instantiate(Transform parent = null)
	{
		return Facepunch.Instantiate.GameObject(base.Get(), parent);
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x00155F98 File Offset: 0x00154198
	public BaseEntity GetEntity()
	{
		return base.Get().GetComponent<BaseEntity>();
	}
}
