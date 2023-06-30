using System;
using Rust;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
[RequireComponent(typeof(Collider))]
public class TakeCollisionDamage : FacepunchBehaviour
{
	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06002734 RID: 10036 RVA: 0x000F4CDB File Offset: 0x000F2EDB
	private bool IsServer
	{
		get
		{
			return this.entity.isServer;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06002735 RID: 10037 RVA: 0x000F4CE8 File Offset: 0x000F2EE8
	private bool IsClient
	{
		get
		{
			return this.entity.isClient;
		}
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000F4CF8 File Offset: 0x000F2EF8
	protected void OnCollisionEnter(Collision collision)
	{
		if (this.IsClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		Rigidbody rigidbody = collision.rigidbody;
		float num = ((rigidbody == null) ? 100f : rigidbody.mass);
		float num2 = collision.relativeVelocity.magnitude * (this.entity.RealisticMass + num) / Time.fixedDeltaTime;
		float num3 = Mathf.InverseLerp(this.forceForAnyDamage, this.forceForMaxDamage, num2);
		if (num3 > 0f)
		{
			this.pendingDamage = Mathf.Max(this.pendingDamage, Mathf.Lerp(this.minDamage, this.maxDamage, num3));
			if (this.pendingDamage > this.entity.Health())
			{
				TakeCollisionDamage.ICanRestoreVelocity canRestoreVelocity = collision.gameObject.ToBaseEntity() as TakeCollisionDamage.ICanRestoreVelocity;
				if (canRestoreVelocity != null)
				{
					canRestoreVelocity.RestoreVelocity(collision.relativeVelocity * this.velocityRestorePercent);
				}
			}
			base.Invoke(new Action(this.DoDamage), 0f);
		}
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000F4E05 File Offset: 0x000F3005
	protected void OnDestroy()
	{
		base.CancelInvoke(new Action(this.DoDamage));
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000F4E1C File Offset: 0x000F301C
	private void DoDamage()
	{
		if (this.entity == null || this.entity.IsDead() || this.entity.IsDestroyed)
		{
			return;
		}
		if (this.pendingDamage > 0f)
		{
			this.entity.Hurt(this.pendingDamage, DamageType.Collision, null, false);
			this.pendingDamage = 0f;
		}
	}

	// Token: 0x04001F7F RID: 8063
	[SerializeField]
	private BaseCombatEntity entity;

	// Token: 0x04001F80 RID: 8064
	[SerializeField]
	private float minDamage = 1f;

	// Token: 0x04001F81 RID: 8065
	[SerializeField]
	private float maxDamage = 250f;

	// Token: 0x04001F82 RID: 8066
	[SerializeField]
	private float forceForAnyDamage = 20000f;

	// Token: 0x04001F83 RID: 8067
	[SerializeField]
	private float forceForMaxDamage = 1000000f;

	// Token: 0x04001F84 RID: 8068
	[SerializeField]
	private float velocityRestorePercent = 0.75f;

	// Token: 0x04001F85 RID: 8069
	private float pendingDamage;

	// Token: 0x02000D22 RID: 3362
	public interface ICanRestoreVelocity
	{
		// Token: 0x06005072 RID: 20594
		void RestoreVelocity(Vector3 amount);
	}
}
