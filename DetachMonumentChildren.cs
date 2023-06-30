using System;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class DetachMonumentChildren : MonoBehaviour
{
	// Token: 0x06002972 RID: 10610 RVA: 0x000FEC75 File Offset: 0x000FCE75
	private void Awake()
	{
		base.transform.DetachChildren();
	}
}
