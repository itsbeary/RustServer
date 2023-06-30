using System;
using UnityEngine;

// Token: 0x02000917 RID: 2327
public class PreUpdateHook : MonoBehaviour
{
	// Token: 0x06003812 RID: 14354 RVA: 0x0014E19B File Offset: 0x0014C39B
	private void Update()
	{
		Action onUpdate = PreUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x0014E1AC File Offset: 0x0014C3AC
	private void LateUpdate()
	{
		Action onLateUpdate = PreUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x0014E1BD File Offset: 0x0014C3BD
	private void FixedUpdate()
	{
		Action onFixedUpdate = PreUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}

	// Token: 0x0400337E RID: 13182
	public static Action OnUpdate;

	// Token: 0x0400337F RID: 13183
	public static Action OnLateUpdate;

	// Token: 0x04003380 RID: 13184
	public static Action OnFixedUpdate;
}
