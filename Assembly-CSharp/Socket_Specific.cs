using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class Socket_Specific : Socket_Base
{
	// Token: 0x06001D2D RID: 7469 RVA: 0x000C9AF8 File Offset: 0x000C7CF8
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000C9B94 File Offset: 0x000C7D94
	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		Socket_Specific_Female socket_Specific_Female = target.socket as Socket_Specific_Female;
		return !(socket_Specific_Female == null) && (!this.blockPlacementOnChildEntities || !(target.entity != null) || !(target.entity.GetParentEntity() != null)) && socket_Specific_Female.CanAccept(this);
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x000C9BF8 File Offset: 0x000C7DF8
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = target.socket.rotation;
		if (target.socket.male && target.socket.female)
		{
			quaternion = target.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
		}
		Transform transform = target.entity.transform;
		Vector3 vector = transform.localToWorldMatrix.MultiplyPoint3x4(target.socket.position);
		Quaternion quaternion2;
		if (this.useFemaleRotation)
		{
			quaternion2 = transform.rotation * quaternion;
		}
		else
		{
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			Vector3 vector3 = new Vector3(target.player.eyes.position.x, 0f, target.player.eyes.position.z);
			quaternion2 = Quaternion.LookRotation((vector2 - vector3).normalized) * quaternion;
		}
		Construction.Placement placement = new Construction.Placement();
		Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(this.rotation);
		Vector3 vector4 = quaternion3 * this.position;
		placement.position = vector - vector4;
		placement.rotation = quaternion3;
		return placement;
	}

	// Token: 0x040015BD RID: 5565
	public bool useFemaleRotation = true;

	// Token: 0x040015BE RID: 5566
	public string targetSocketName;

	// Token: 0x040015BF RID: 5567
	public bool blockPlacementOnChildEntities;
}
