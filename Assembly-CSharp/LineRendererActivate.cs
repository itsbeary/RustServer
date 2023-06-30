using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	// Token: 0x06001938 RID: 6456 RVA: 0x000B98E6 File Offset: 0x000B7AE6
	private void OnEnable()
	{
		base.GetComponent<LineRenderer>().enabled = true;
	}
}
