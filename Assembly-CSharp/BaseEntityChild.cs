using System;
using Rust;
using Rust.Registry;
using UnityEngine;

// Token: 0x020003BD RID: 957
public class BaseEntityChild : MonoBehaviour
{
	// Token: 0x06002168 RID: 8552 RVA: 0x000DAE64 File Offset: 0x000D9064
	public static void Setup(GameObject obj, BaseEntity parent)
	{
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Entity.Register(obj, parent);
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000DAEA0 File Offset: 0x000D90A0
	public void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Entity.Unregister(base.gameObject);
		}
	}
}
