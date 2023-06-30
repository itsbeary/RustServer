using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class SocketMod_AngleCheck : SocketMod
{
	// Token: 0x06001CF2 RID: 7410 RVA: 0x000C8624 File Offset: 0x000C6824
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, this.withinDegrees, 1f, 0f, 1f);
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x000C865F File Offset: 0x000C685F
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.worldNormal.DotDegrees(place.rotation * Vector3.up) < this.withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = SocketMod_AngleCheck.ErrorPhrase.translated;
		return false;
	}

	// Token: 0x0400157D RID: 5501
	public bool wantsAngle = true;

	// Token: 0x0400157E RID: 5502
	public Vector3 worldNormal = Vector3.up;

	// Token: 0x0400157F RID: 5503
	public float withinDegrees = 45f;

	// Token: 0x04001580 RID: 5504
	public static Translate.Phrase ErrorPhrase = new Translate.Phrase("error_anglecheck", "Invalid angle");
}
