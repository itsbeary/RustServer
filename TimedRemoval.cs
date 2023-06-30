using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class TimedRemoval : MonoBehaviour
{
	// Token: 0x06001E49 RID: 7753 RVA: 0x000CE46D File Offset: 0x000CC66D
	private void OnEnable()
	{
		UnityEngine.Object.Destroy(this.objectToDestroy, this.removeDelay);
	}

	// Token: 0x04001782 RID: 6018
	public UnityEngine.Object objectToDestroy;

	// Token: 0x04001783 RID: 6019
	public float removeDelay = 1f;
}
