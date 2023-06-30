using System;
using UnityEngine;

// Token: 0x02000465 RID: 1125
public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06002543 RID: 9539 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasRangeLimit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000EB7E8 File Offset: 0x000E99E8
	public float GetMaxRange(float maxFuseTime)
	{
		if (this.gravityModifier == 0f)
		{
			return float.PositiveInfinity;
		}
		float num = Mathf.Sin(1.5707964f) * this.speed * this.speed / -(Physics.gravity.y * this.gravityModifier);
		float num2 = this.speed * maxFuseTime;
		return Mathf.Min(num, num2);
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06002545 RID: 9541 RVA: 0x000EB842 File Offset: 0x000E9A42
	protected virtual int mask
	{
		get
		{
			return 1237003025;
		}
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06002546 RID: 9542 RVA: 0x000EB849 File Offset: 0x000E9A49
	// (set) Token: 0x06002547 RID: 9543 RVA: 0x000EB851 File Offset: 0x000E9A51
	public Vector3 CurrentVelocity { get; protected set; }

	// Token: 0x06002548 RID: 9544 RVA: 0x000EB85A File Offset: 0x000E9A5A
	protected void FixedUpdate()
	{
		if (base.baseEntity != null && base.baseEntity.isServer)
		{
			this.DoMovement();
		}
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x000EB880 File Offset: 0x000E9A80
	public virtual bool DoMovement()
	{
		if (this.impacted)
		{
			return false;
		}
		this.CurrentVelocity += Physics.gravity * this.gravityModifier * Time.fixedDeltaTime * Time.timeScale;
		Vector3 vector = this.CurrentVelocity;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float num = Time.time + this.swimRandom;
			Vector3 vector2 = new Vector3(Mathf.Sin(num * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(num * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(num * this.swimSpeed.z) * this.swimScale.z);
			vector2 = base.transform.InverseTransformDirection(vector2);
			vector += vector2;
		}
		float num2 = vector.magnitude * Time.fixedDeltaTime;
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		if (GamePhysics.Trace(new Ray(position, vector.normalized), this.radius, out raycastHit, num2 + this.scanRange, this.mask, QueryTriggerInteraction.Ignore, null))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (this.IsAValidHit(entity))
			{
				ColliderInfo colliderInfo = ((raycastHit.collider != null) ? raycastHit.collider.GetComponent<ColliderInfo>() : null);
				if (colliderInfo == null || colliderInfo.HasFlag(ColliderInfo.Flags.Shootable))
				{
					base.transform.position += base.transform.forward * Mathf.Max(0f, raycastHit.distance - 0.1f);
					ServerProjectile.IProjectileImpact component = base.GetComponent<ServerProjectile.IProjectileImpact>();
					if (component != null)
					{
						component.ProjectileImpact(raycastHit, position);
					}
					this.impacted = true;
					return false;
				}
			}
		}
		base.transform.position += base.transform.forward * num2;
		base.transform.rotation = Quaternion.LookRotation(vector.normalized);
		return true;
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000EBAC0 File Offset: 0x000E9CC0
	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		return !hitEnt.IsValid() || !base.baseEntity.creatorEntity.IsValid() || hitEnt.net.ID != base.baseEntity.creatorEntity.net.ID;
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000EBB0E File Offset: 0x000E9D0E
	public virtual void InitializeVelocity(Vector3 overrideVel)
	{
		base.transform.rotation = Quaternion.LookRotation(overrideVel.normalized);
		this.initialVelocity = overrideVel;
		this.CurrentVelocity = overrideVel;
	}

	// Token: 0x04001D7B RID: 7547
	public Vector3 initialVelocity;

	// Token: 0x04001D7C RID: 7548
	public float drag;

	// Token: 0x04001D7D RID: 7549
	public float gravityModifier = 1f;

	// Token: 0x04001D7E RID: 7550
	public float speed = 15f;

	// Token: 0x04001D7F RID: 7551
	public float scanRange;

	// Token: 0x04001D80 RID: 7552
	public Vector3 swimScale;

	// Token: 0x04001D81 RID: 7553
	public Vector3 swimSpeed;

	// Token: 0x04001D82 RID: 7554
	public float radius;

	// Token: 0x04001D83 RID: 7555
	private bool impacted;

	// Token: 0x04001D84 RID: 7556
	private float swimRandom;

	// Token: 0x02000D08 RID: 3336
	public interface IProjectileImpact
	{
		// Token: 0x0600502E RID: 20526
		void ProjectileImpact(RaycastHit hitInfo, Vector3 rayOrigin);
	}
}
