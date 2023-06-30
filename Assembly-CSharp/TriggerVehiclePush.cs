using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	// Token: 0x060000F1 RID: 241 RVA: 0x00006D34 File Offset: 0x00004F34
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity is BuildingBlock)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060000F2 RID: 242 RVA: 0x00006D81 File Offset: 0x00004F81
	public int ContentsCount
	{
		get
		{
			HashSet<BaseEntity> entityContents = this.entityContents;
			if (entityContents == null)
			{
				return 0;
			}
			return entityContents.Count;
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00006D94 File Offset: 0x00004F94
	public void FixedUpdate()
	{
		if (this.thisEntity == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity.IsValid() && !baseEntity.EqualNetID(this.thisEntity))
			{
				Rigidbody rigidbody = baseEntity.GetComponent<Rigidbody>();
				if (rigidbody == null && this.allowParentRigidbody)
				{
					rigidbody = baseEntity.GetComponentInParent<Rigidbody>();
				}
				if (rigidbody && !rigidbody.isKinematic)
				{
					float num = Vector3Ex.Distance2D(this.useRigidbodyPosition ? rigidbody.transform.position : baseEntity.transform.position, base.transform.position);
					float num2 = 1f - Mathf.InverseLerp(this.minRadius, this.maxRadius, num);
					float num3 = 1f - Mathf.InverseLerp(this.minRadius - 1f, this.minRadius, num);
					Vector3 vector = baseEntity.ClosestPoint(position);
					Vector3 vector2 = Vector3Ex.Direction2D(vector, position);
					vector2 = Vector3Ex.Direction2D(this.useCentreOfMass ? rigidbody.worldCenterOfMass : vector, position);
					if (this.snapToAxis)
					{
						Vector3 vector3 = base.transform.InverseTransformDirection(vector2);
						if (Vector3.Angle(vector3, this.axisToSnapTo) < Vector3.Angle(vector3, -this.axisToSnapTo))
						{
							vector2 = base.transform.TransformDirection(this.axisToSnapTo);
						}
						else
						{
							vector2 = -base.transform.TransformDirection(this.axisToSnapTo);
						}
					}
					rigidbody.AddForceAtPosition(vector2 * this.maxPushVelocity * num2, vector, ForceMode.Acceleration);
					if (num3 > 0f)
					{
						rigidbody.AddForceAtPosition(vector2 * 1f * num3, vector, ForceMode.VelocityChange);
					}
				}
			}
		}
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00006FAC File Offset: 0x000051AC
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.minRadius);
		Gizmos.color = new Color(0.5f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(base.transform.position, this.maxRadius);
		if (this.snapToAxis)
		{
			Gizmos.color = Color.cyan;
			Vector3 vector = base.transform.TransformDirection(this.axisToSnapTo);
			Gizmos.DrawLine(base.transform.position + vector, base.transform.position - vector);
		}
	}

	// Token: 0x04000109 RID: 265
	public BaseEntity thisEntity;

	// Token: 0x0400010A RID: 266
	public float maxPushVelocity = 10f;

	// Token: 0x0400010B RID: 267
	public float minRadius;

	// Token: 0x0400010C RID: 268
	public float maxRadius;

	// Token: 0x0400010D RID: 269
	public bool snapToAxis;

	// Token: 0x0400010E RID: 270
	public Vector3 axisToSnapTo = Vector3.right;

	// Token: 0x0400010F RID: 271
	public bool allowParentRigidbody;

	// Token: 0x04000110 RID: 272
	public bool useRigidbodyPosition;

	// Token: 0x04000111 RID: 273
	public bool useCentreOfMass;
}
