using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class Socket_Free : Socket_Base
{
	// Token: 0x06001D29 RID: 7465 RVA: 0x000C9940 File Offset: 0x000C7B40
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
		GizmosUtil.DrawWireCircleZ(Vector3.forward * 0f, 0.2f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x000C99AF File Offset: 0x000C7BAF
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x000C99B8 File Offset: 0x000C7BB8
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.identity;
		if (this.useTargetNormal)
		{
			if (this.blendAimAngle)
			{
				Vector3 vector = (target.position - target.ray.origin).normalized;
				float num = Mathf.Abs(Vector3.Dot(vector, target.normal));
				vector = Vector3.Lerp(vector, this.idealPlacementNormal, num);
				quaternion = Quaternion.LookRotation(target.normal, vector) * Quaternion.Inverse(this.rotation) * Quaternion.Euler(target.rotation);
			}
			else
			{
				quaternion = Quaternion.LookRotation(target.normal);
			}
		}
		else
		{
			Vector3 normalized = (target.position - target.ray.origin).normalized;
			normalized.y = 0f;
			quaternion = Quaternion.LookRotation(normalized, this.idealPlacementNormal) * Quaternion.Euler(target.rotation);
		}
		Vector3 vector2 = target.position;
		vector2 -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = vector2
		};
	}

	// Token: 0x040015BA RID: 5562
	public Vector3 idealPlacementNormal = Vector3.up;

	// Token: 0x040015BB RID: 5563
	public bool useTargetNormal = true;

	// Token: 0x040015BC RID: 5564
	public bool blendAimAngle = true;
}
