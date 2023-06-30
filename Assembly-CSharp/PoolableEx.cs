using System;
using UnityEngine;

// Token: 0x02000964 RID: 2404
public static class PoolableEx
{
	// Token: 0x060039A2 RID: 14754 RVA: 0x00155AC4 File Offset: 0x00153CC4
	public static bool SupportsPoolingInParent(this GameObject gameObject)
	{
		Poolable componentInParent = gameObject.GetComponentInParent<Poolable>();
		return componentInParent != null && componentInParent.prefabID > 0U;
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x00155AEC File Offset: 0x00153CEC
	public static bool SupportsPooling(this GameObject gameObject)
	{
		Poolable component = gameObject.GetComponent<Poolable>();
		return component != null && component.prefabID > 0U;
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x00155B14 File Offset: 0x00153D14
	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		if (gameObject.activeSelf)
		{
			gameObject.GetComponent<Poolable>().SetBehaviourEnabled(true);
			return;
		}
		gameObject.SetActive(true);
	}
}
