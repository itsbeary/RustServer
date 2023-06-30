using System;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public class FruitScale : MonoBehaviour, IClientComponent
{
	// Token: 0x060024B3 RID: 9395 RVA: 0x000E93E6 File Offset: 0x000E75E6
	public void SetProgress(float progress)
	{
		base.transform.localScale = Vector3.one * progress;
	}
}
