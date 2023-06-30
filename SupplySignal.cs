using System;
using UnityEngine;

// Token: 0x0200042B RID: 1067
public class SupplySignal : TimedExplosive
{
	// Token: 0x06002433 RID: 9267 RVA: 0x000E6F54 File Offset: 0x000E5154
	public override void Explode()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.EntityToCreate.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
			baseEntity.SendMessage("InitDropPosition", base.transform.position + vector, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
		base.Invoke(new Action(this.FinishUp), 210f);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x00003384 File Offset: 0x00001584
	public void FinishUp()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x04001C2E RID: 7214
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001C2F RID: 7215
	public GameObjectRef EntityToCreate;

	// Token: 0x04001C30 RID: 7216
	[NonSerialized]
	public GameObject smokeEffect;
}
