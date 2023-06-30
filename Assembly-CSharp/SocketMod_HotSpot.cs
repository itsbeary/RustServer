using System;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class SocketMod_HotSpot : SocketMod
{
	// Token: 0x06001D08 RID: 7432 RVA: 0x000C8F77 File Offset: 0x000C7177
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, this.spotSize);
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x000C8FB8 File Offset: 0x000C71B8
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		place.position = vector;
	}

	// Token: 0x04001597 RID: 5527
	public float spotSize = 0.1f;
}
