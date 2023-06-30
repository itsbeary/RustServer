using System;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06001DF1 RID: 7665 RVA: 0x000CCEA8 File Offset: 0x000CB0A8
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		base.GetComponent<ParticleSystem>().enableEmission = false;
		if (this.destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(base.gameObject, this.destroyAfterSeconds);
		}
	}

	// Token: 0x040016EE RID: 5870
	public float destroyAfterSeconds;
}
