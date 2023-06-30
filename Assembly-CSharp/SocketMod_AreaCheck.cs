using System;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class SocketMod_AreaCheck : SocketMod
{
	// Token: 0x06001CF6 RID: 7414 RVA: 0x000C86D4 File Offset: 0x000C68D4
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = true;
		if (!this.wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green.WithAlpha(0.5f) : Color.red.WithAlpha(0.5f));
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x000C8743 File Offset: 0x000C6943
	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask, BaseEntity entity = null)
	{
		return GamePhysics.CheckOBBAndEntity(new OBB(position, rotation, bounds), layerMask.value, QueryTriggerInteraction.UseGlobal, entity);
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x000C875C File Offset: 0x000C695C
	public bool DoCheck(Vector3 position, Quaternion rotation, BaseEntity entity = null)
	{
		Vector3 vector = position + rotation * this.worldPosition;
		Quaternion quaternion = rotation * this.worldRotation;
		return SocketMod_AreaCheck.IsInArea(vector, quaternion, this.bounds, this.layerMask, entity) == this.wantsInside;
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000C87A3 File Offset: 0x000C69A3
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.DoCheck(place.position, place.rotation, null))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: IsInArea (" + this.hierachyName + ")";
		return false;
	}

	// Token: 0x04001581 RID: 5505
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	// Token: 0x04001582 RID: 5506
	public LayerMask layerMask;

	// Token: 0x04001583 RID: 5507
	public bool wantsInside = true;
}
