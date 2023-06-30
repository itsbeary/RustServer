using System;
using UnityEngine;

// Token: 0x02000489 RID: 1161
public class CH47ReinforcementListener : BaseEntity
{
	// Token: 0x0600267B RID: 9851 RVA: 0x000F2D1C File Offset: 0x000F0F1C
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			this.Call();
		}
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000F2D34 File Offset: 0x000F0F34
	public void Call()
	{
		CH47HelicopterAIController component = GameManager.server.CreateEntity(this.heliPrefab.resourcePath, default(Vector3), default(Quaternion), true).GetComponent<CH47HelicopterAIController>();
		if (component)
		{
			Vector3 size = TerrainMeta.Size;
			CH47LandingZone closest = CH47LandingZone.GetClosest(base.transform.position);
			Vector3 zero = Vector3.zero;
			zero.y = closest.transform.position.y;
			Vector3 vector = Vector3Ex.Direction2D(closest.transform.position, zero);
			Vector3 vector2 = closest.transform.position + vector * this.startDist;
			vector2.y = closest.transform.position.y;
			component.transform.position = vector2;
			component.SetLandingTarget(closest.transform.position);
			component.Spawn();
		}
	}

	// Token: 0x04001EC9 RID: 7881
	public string listenString;

	// Token: 0x04001ECA RID: 7882
	public GameObjectRef heliPrefab;

	// Token: 0x04001ECB RID: 7883
	public float startDist = 300f;
}
