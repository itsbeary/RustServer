using System;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public class TriggerWetness : TriggerBase
{
	// Token: 0x06002C07 RID: 11271 RVA: 0x0010A8A8 File Offset: 0x00108AA8
	public float WorkoutWetness(Vector3 position)
	{
		if (this.ApplyLocalHeightCheck && base.transform.InverseTransformPoint(position).y < this.MinLocalHeight)
		{
			return 0f;
		}
		float num = Vector3Ex.Distance2D(this.OriginTransform.position, position) / this.TargetCollider.radius;
		num = Mathf.Clamp01(num);
		num = 1f - num;
		return Mathf.Lerp(0f, this.Wetness, num);
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x0010A91C File Offset: 0x00108B1C
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
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x040023C6 RID: 9158
	public float Wetness = 0.25f;

	// Token: 0x040023C7 RID: 9159
	public SphereCollider TargetCollider;

	// Token: 0x040023C8 RID: 9160
	public Transform OriginTransform;

	// Token: 0x040023C9 RID: 9161
	public bool ApplyLocalHeightCheck;

	// Token: 0x040023CA RID: 9162
	public float MinLocalHeight;
}
