using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class SocketMod_Attraction : SocketMod
{
	// Token: 0x06001CFB RID: 7419 RVA: 0x000C8808 File Offset: 0x000C6A08
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawSphere(Vector3.zero, this.outerRadius);
		Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
		Gizmos.DrawSphere(Vector3.zero, this.innerRadius);
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool DoCheck(Construction.Placement place)
	{
		return true;
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x000C8884 File Offset: 0x000C6A84
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.outerRadius * 2f, list, -1, QueryTriggerInteraction.Collide);
		Vector3 vector2 = Vector3.zero;
		float num = float.MaxValue;
		Vector3 position = place.position;
		Quaternion quaternion = Quaternion.identity;
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer == this.isServer)
			{
				AttractionPoint[] array = this.prefabAttribute.FindAll<AttractionPoint>(baseEntity.prefabID);
				if (array != null)
				{
					foreach (AttractionPoint attractionPoint in array)
					{
						if (!(attractionPoint.groupName != this.groupName))
						{
							Vector3 vector3 = baseEntity.transform.position + baseEntity.transform.rotation * attractionPoint.worldPosition;
							float magnitude = (vector3 - vector).magnitude;
							if (this.ignoreRotationForRadiusCheck)
							{
								Vector3 vector4 = baseEntity.transform.TransformPoint(Vector3.LerpUnclamped(Vector3.zero, attractionPoint.worldPosition.WithY(0f), 2f));
								float num2 = Vector3.Distance(vector4, position);
								if (num2 < num)
								{
									num = num2;
									vector2 = vector4;
									quaternion = baseEntity.transform.rotation;
								}
							}
							if (magnitude <= this.outerRadius)
							{
								Quaternion quaternion2 = QuaternionEx.LookRotationWithOffset(this.worldPosition, vector3 - place.position, Vector3.up);
								float num3 = Mathf.InverseLerp(this.outerRadius, this.innerRadius, magnitude);
								if (this.lockRotation)
								{
									num3 = 1f;
								}
								if (this.lockRotation)
								{
									Vector3 vector5 = place.rotation.eulerAngles;
									vector5 -= new Vector3(vector5.x % 90f, vector5.y % 90f, vector5.z % 90f);
									place.rotation = Quaternion.Euler(vector5 + baseEntity.transform.eulerAngles);
								}
								else
								{
									place.rotation = Quaternion.Lerp(place.rotation, quaternion2, num3);
								}
								vector = place.position + place.rotation * this.worldPosition;
								Vector3 vector6 = vector3 - vector;
								place.position += vector6 * num3;
							}
						}
					}
				}
			}
		}
		if (num < 3.4028235E+38f && this.ignoreRotationForRadiusCheck)
		{
			place.position = vector2;
			place.rotation = quaternion;
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x04001584 RID: 5508
	public float outerRadius = 1f;

	// Token: 0x04001585 RID: 5509
	public float innerRadius = 0.1f;

	// Token: 0x04001586 RID: 5510
	public string groupName = "wallbottom";

	// Token: 0x04001587 RID: 5511
	public bool lockRotation;

	// Token: 0x04001588 RID: 5512
	public bool ignoreRotationForRadiusCheck;
}
