using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class DirectionProperties : PrefabAttribute
{
	// Token: 0x06001CA2 RID: 7330 RVA: 0x000C7262 File Offset: 0x000C5462
	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000C7270 File Offset: 0x000C5470
	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		if (this.bounds.size == Vector3.zero)
		{
			return false;
		}
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (initiatorPlayer == null)
		{
			return false;
		}
		BaseEntity hitEntity = info.HitEntity;
		if (hitEntity == null)
		{
			return false;
		}
		Matrix4x4 worldToLocalMatrix = tx.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(info.PointStart) - this.worldPosition;
		float num = this.worldForward.DotDegrees(vector);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(info.HitPositionWorld);
		OBB obb = new OBB(this.worldPosition, this.worldRotation, this.bounds);
		Vector3 position = initiatorPlayer.eyes.position;
		WeakpointProperties[] array = PrefabAttribute.server.FindAll<WeakpointProperties>(hitEntity.prefabID);
		if (array != null && array.Length != 0)
		{
			bool flag = false;
			foreach (WeakpointProperties weakpointProperties in array)
			{
				if ((!weakpointProperties.BlockWhenRoofAttached || this.CheckWeakpointRoof(hitEntity)) && this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(weakpointProperties.worldPosition)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		else if (!this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(obb.position)))
		{
			return false;
		}
		return num > 100f && obb.Contains(vector2);
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x000C73BC File Offset: 0x000C55BC
	private bool CheckWeakpointRoof(BaseEntity hitEntity)
	{
		foreach (EntityLink entityLink in hitEntity.GetEntityLinks(true))
		{
			if (entityLink.socket is NeighbourSocket)
			{
				using (List<EntityLink>.Enumerator enumerator2 = entityLink.connections.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						BuildingBlock buildingBlock;
						if ((buildingBlock = enumerator2.Current.owner as BuildingBlock) != null && (buildingBlock.ShortPrefabName == "roof" || buildingBlock.ShortPrefabName == "roof.triangle"))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x000C7490 File Offset: 0x000C5690
	private bool IsWeakspotVisible(BaseEntity hitEntity, Vector3 playerEyes, Vector3 weakspotPos)
	{
		return hitEntity.IsVisible(playerEyes, weakspotPos, float.PositiveInfinity);
	}

	// Token: 0x04001537 RID: 5431
	private const float radius = 200f;

	// Token: 0x04001538 RID: 5432
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04001539 RID: 5433
	public ProtectionProperties extraProtection;
}
