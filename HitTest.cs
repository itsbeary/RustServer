using System;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class HitTest
{
	// Token: 0x060029EF RID: 10735 RVA: 0x001011CC File Offset: 0x000FF3CC
	public void CopyFrom(HitTest other, bool copyHitInfo = false)
	{
		this.type = other.type;
		this.AttackRay = other.AttackRay;
		this.Radius = other.Radius;
		this.Forgiveness = other.Forgiveness;
		this.MaxDistance = other.MaxDistance;
		this.RayHit = other.RayHit;
		this.damageProperties = other.damageProperties;
		this.ignoreEntity = other.ignoreEntity;
		if (copyHitInfo)
		{
			this.HitEntity = other.HitEntity;
			this.HitPoint = other.HitPoint;
			this.HitNormal = other.HitNormal;
			this.HitDistance = other.HitDistance;
			this.HitTransform = other.HitTransform;
			this.HitPart = other.HitPart;
			this.HitMaterial = other.HitMaterial;
			this.MultiHit = other.MultiHit;
			this.BestHit = other.BestHit;
			this.DidHit = other.DidHit;
		}
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x001012B4 File Offset: 0x000FF4B4
	public Vector3 HitPointWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformPoint(this.HitPoint);
		}
		return this.HitPoint;
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x00101300 File Offset: 0x000FF500
	public Vector3 HitNormalWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformDirection(this.HitNormal);
		}
		return this.HitNormal;
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x0010134C File Offset: 0x000FF54C
	public void Clear()
	{
		this.type = HitTest.Type.Generic;
		this.AttackRay = default(Ray);
		this.Radius = 0f;
		this.Forgiveness = 0f;
		this.MaxDistance = 0f;
		this.RayHit = default(RaycastHit);
		this.MultiHit = false;
		this.BestHit = false;
		this.DidHit = false;
		this.damageProperties = null;
		this.gameObject = null;
		this.collider = null;
		this.ignoreEntity = null;
		this.HitEntity = null;
		this.HitPoint = default(Vector3);
		this.HitNormal = default(Vector3);
		this.HitDistance = 0f;
		this.HitTransform = null;
		this.HitPart = 0U;
		this.HitMaterial = null;
	}

	// Token: 0x0400220B RID: 8715
	public HitTest.Type type;

	// Token: 0x0400220C RID: 8716
	public Ray AttackRay;

	// Token: 0x0400220D RID: 8717
	public float Radius;

	// Token: 0x0400220E RID: 8718
	public float Forgiveness;

	// Token: 0x0400220F RID: 8719
	public float MaxDistance;

	// Token: 0x04002210 RID: 8720
	public RaycastHit RayHit;

	// Token: 0x04002211 RID: 8721
	public bool MultiHit;

	// Token: 0x04002212 RID: 8722
	public bool BestHit;

	// Token: 0x04002213 RID: 8723
	public bool DidHit;

	// Token: 0x04002214 RID: 8724
	public DamageProperties damageProperties;

	// Token: 0x04002215 RID: 8725
	public GameObject gameObject;

	// Token: 0x04002216 RID: 8726
	public Collider collider;

	// Token: 0x04002217 RID: 8727
	public BaseEntity ignoreEntity;

	// Token: 0x04002218 RID: 8728
	public BaseEntity HitEntity;

	// Token: 0x04002219 RID: 8729
	public Vector3 HitPoint;

	// Token: 0x0400221A RID: 8730
	public Vector3 HitNormal;

	// Token: 0x0400221B RID: 8731
	public float HitDistance;

	// Token: 0x0400221C RID: 8732
	public Transform HitTransform;

	// Token: 0x0400221D RID: 8733
	public uint HitPart;

	// Token: 0x0400221E RID: 8734
	public string HitMaterial;

	// Token: 0x02000D57 RID: 3415
	public enum Type
	{
		// Token: 0x04004794 RID: 18324
		Generic,
		// Token: 0x04004795 RID: 18325
		ProjectileEffect,
		// Token: 0x04004796 RID: 18326
		Projectile,
		// Token: 0x04004797 RID: 18327
		MeleeAttack,
		// Token: 0x04004798 RID: 18328
		Use
	}
}
