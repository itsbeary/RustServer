using System;
using UnityEngine;

// Token: 0x0200058B RID: 1419
public class TriggerForce : TriggerBase, IServerComponent
{
	// Token: 0x06002B92 RID: 11154 RVA: 0x00108B38 File Offset: 0x00106D38
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

	// Token: 0x06002B93 RID: 11155 RVA: 0x00108B7C File Offset: 0x00106D7C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		Vector3 vector = base.transform.TransformDirection(this.velocity);
		ent.ApplyInheritedVelocity(vector);
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x00108BA9 File Offset: 0x00106DA9
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x00108BC0 File Offset: 0x00106DC0
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			Vector3 vector = base.transform.TransformDirection(this.velocity);
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if (baseEntity != null)
				{
					baseEntity.ApplyInheritedVelocity(vector);
				}
			}
		}
	}

	// Token: 0x04002379 RID: 9081
	public const float GravityMultiplier = 0.1f;

	// Token: 0x0400237A RID: 9082
	public const float VelocityLerp = 10f;

	// Token: 0x0400237B RID: 9083
	public const float AngularDrag = 10f;

	// Token: 0x0400237C RID: 9084
	public Vector3 velocity = Vector3.forward;
}
