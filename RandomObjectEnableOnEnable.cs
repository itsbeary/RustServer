using System;
using UnityEngine;

// Token: 0x020008B4 RID: 2228
public class RandomObjectEnableOnEnable : MonoBehaviour
{
	// Token: 0x06003717 RID: 14103 RVA: 0x0014BAC6 File Offset: 0x00149CC6
	public void OnEnable()
	{
		this.objects[UnityEngine.Random.Range(0, this.objects.Length)].SetActive(true);
	}

	// Token: 0x04003243 RID: 12867
	public GameObject[] objects;
}
