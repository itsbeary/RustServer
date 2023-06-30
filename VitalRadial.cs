using System;
using UnityEngine;

// Token: 0x020008EF RID: 2287
public class VitalRadial : MonoBehaviour
{
	// Token: 0x060037A2 RID: 14242 RVA: 0x0014CDF6 File Offset: 0x0014AFF6
	private void Awake()
	{
		Debug.LogWarning("VitalRadial is obsolete " + base.transform.GetRecursiveName(""), base.gameObject);
	}
}
