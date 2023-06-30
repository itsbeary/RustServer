using System;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class SocketMod_TerrainCheck : SocketMod
{
	// Token: 0x06001D18 RID: 7448 RVA: 0x000C94BC File Offset: 0x000C76BC
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = SocketMod_TerrainCheck.IsInTerrain(base.transform.position);
		if (!this.wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x000C951C File Offset: 0x000C771C
	public static bool IsInTerrain(Vector3 vPoint)
	{
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				if (terrain.SampleHeight(vPoint) + terrain.transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x000C95C0 File Offset: 0x000C77C0
	public override bool DoCheck(Construction.Placement place)
	{
		if (SocketMod_TerrainCheck.IsInTerrain(place.position + place.rotation * this.worldPosition) == this.wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = this.fullName + ": not in terrain";
		return false;
	}

	// Token: 0x040015A6 RID: 5542
	public bool wantsInTerrain = true;
}
