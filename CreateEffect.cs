using System;
using UnityEngine;

// Token: 0x020004F6 RID: 1270
public class CreateEffect : MonoBehaviour
{
	// Token: 0x06002932 RID: 10546 RVA: 0x000FDADF File Offset: 0x000FBCDF
	public void OnEnable()
	{
		Effect.client.Run(this.EffectToCreate.resourcePath, base.transform.position, base.transform.up, base.transform.forward, Effect.Type.Generic);
	}

	// Token: 0x0400214A RID: 8522
	public GameObjectRef EffectToCreate;
}
