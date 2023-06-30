using System;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class TriggerRagdollRelocate : TriggerBase
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x0010A1D4 File Offset: 0x001083D4
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		BaseEntity baseEntity = obj.transform.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer)
		{
			this.RepositionTransform(baseEntity.transform);
		}
		Ragdoll componentInParent = obj.GetComponentInParent<Ragdoll>();
		if (componentInParent != null)
		{
			this.RepositionTransform(componentInParent.transform);
			foreach (Rigidbody rigidbody in componentInParent.rigidbodies)
			{
				if (rigidbody.transform.position.y < base.transform.position.y)
				{
					this.RepositionTransform(rigidbody.transform);
				}
			}
		}
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x0010A29C File Offset: 0x0010849C
	private void RepositionTransform(Transform t)
	{
		Vector3 vector = this.targetLocation.InverseTransformPoint(t.position);
		vector.y = 0f;
		vector = this.targetLocation.TransformPoint(vector);
		t.position = vector;
	}

	// Token: 0x040023B7 RID: 9143
	public Transform targetLocation;
}
