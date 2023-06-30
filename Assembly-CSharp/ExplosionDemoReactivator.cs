using System;
using UnityEngine;

// Token: 0x02000994 RID: 2452
public class ExplosionDemoReactivator : MonoBehaviour
{
	// Token: 0x06003A3F RID: 14911 RVA: 0x0015812C File Offset: 0x0015632C
	private void Start()
	{
		base.InvokeRepeating("Reactivate", 0f, this.TimeDelayToReactivate);
	}

	// Token: 0x06003A40 RID: 14912 RVA: 0x00158144 File Offset: 0x00156344
	private void Reactivate()
	{
		foreach (Transform transform in base.GetComponentsInChildren<Transform>())
		{
			transform.gameObject.SetActive(false);
			transform.gameObject.SetActive(true);
		}
	}

	// Token: 0x040034E9 RID: 13545
	public float TimeDelayToReactivate = 3f;
}
