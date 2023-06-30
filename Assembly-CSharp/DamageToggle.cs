using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007AA RID: 1962
[RequireComponent(typeof(Toggle))]
public class DamageToggle : MonoBehaviour
{
	// Token: 0x06003512 RID: 13586 RVA: 0x0014585C File Offset: 0x00143A5C
	private void Reset()
	{
		this.toggle = base.GetComponent<Toggle>();
	}

	// Token: 0x04002BBD RID: 11197
	public Toggle toggle;
}
