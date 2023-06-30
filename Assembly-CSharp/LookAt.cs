using System;
using UnityEngine;

// Token: 0x020002CD RID: 717
[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
	// Token: 0x06001DC3 RID: 7619 RVA: 0x000CC6A2 File Offset: 0x000CA8A2
	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		base.transform.LookAt(this.target);
	}

	// Token: 0x040016AA RID: 5802
	public Transform target;
}
