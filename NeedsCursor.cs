using System;
using UnityEngine;

// Token: 0x020007A8 RID: 1960
public class NeedsCursor : MonoBehaviour, IClientComponent
{
	// Token: 0x0600350E RID: 13582 RVA: 0x0014583D File Offset: 0x00143A3D
	private void Update()
	{
		CursorManager.HoldOpen(false);
	}
}
