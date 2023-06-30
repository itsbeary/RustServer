using System;
using UnityEngine;

// Token: 0x0200049C RID: 1180
public class MagnetSnap
{
	// Token: 0x060026E4 RID: 9956 RVA: 0x000F4105 File Offset: 0x000F2305
	public MagnetSnap(Transform snapLocation)
	{
		this.snapLocation = snapLocation;
		this.prevSnapLocation = snapLocation.position;
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000F4120 File Offset: 0x000F2320
	public void FixedUpdate(Transform target)
	{
		this.PositionTarget(target);
		if (this.snapLocation.hasChanged)
		{
			this.prevSnapLocation = this.snapLocation.position;
			this.snapLocation.hasChanged = false;
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000F4154 File Offset: 0x000F2354
	public void PositionTarget(Transform target)
	{
		if (target == null)
		{
			return;
		}
		Transform transform = target.transform;
		Quaternion quaternion = this.snapLocation.rotation;
		if (Vector3.Angle(transform.forward, this.snapLocation.forward) > 90f)
		{
			quaternion *= Quaternion.Euler(0f, 180f, 0f);
		}
		if (transform.position != this.snapLocation.position)
		{
			transform.position += this.snapLocation.position - this.prevSnapLocation;
			transform.position = Vector3.MoveTowards(transform.position, this.snapLocation.position, 1f * Time.fixedDeltaTime);
		}
		if (transform.rotation != quaternion)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, 40f * Time.fixedDeltaTime);
		}
	}

	// Token: 0x04001F4A RID: 8010
	private Transform snapLocation;

	// Token: 0x04001F4B RID: 8011
	private Vector3 prevSnapLocation;
}
