using System;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class Socket_Terrain : Socket_Base
{
	// Token: 0x06001D35 RID: 7477 RVA: 0x000C9E18 File Offset: 0x000C8018
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
		Gizmos.DrawCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x000C99AF File Offset: 0x000C7BAF
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x000C9F30 File Offset: 0x000C8130
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Vector3 eulerAngles = this.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		Vector3 direction = target.ray.direction;
		direction.y = 0f;
		direction.Normalize();
		Vector3 vector = Vector3.up;
		if (this.alignToNormal)
		{
			vector = target.normal;
		}
		Quaternion quaternion = Quaternion.LookRotation(direction, vector) * Quaternion.Euler(0f, eulerAngles.y, 0f) * Quaternion.Euler(target.rotation);
		Vector3 vector2 = target.position;
		vector2 -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = vector2
		};
	}

	// Token: 0x040015C3 RID: 5571
	public float placementHeight;

	// Token: 0x040015C4 RID: 5572
	public bool alignToNormal;
}
