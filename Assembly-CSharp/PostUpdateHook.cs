using System;
using UnityEngine;

// Token: 0x02000915 RID: 2325
public class PostUpdateHook : MonoBehaviour
{
	// Token: 0x0600380E RID: 14350 RVA: 0x0014E168 File Offset: 0x0014C368
	private void Update()
	{
		Action onUpdate = PostUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x0014E179 File Offset: 0x0014C379
	private void LateUpdate()
	{
		Action onLateUpdate = PostUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x0014E18A File Offset: 0x0014C38A
	private void FixedUpdate()
	{
		Action onFixedUpdate = PostUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}

	// Token: 0x04003372 RID: 13170
	public static Action OnUpdate;

	// Token: 0x04003373 RID: 13171
	public static Action OnLateUpdate;

	// Token: 0x04003374 RID: 13172
	public static Action OnFixedUpdate;
}
