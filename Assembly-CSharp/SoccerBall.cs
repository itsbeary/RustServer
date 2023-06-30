using System;
using UnityEngine;

// Token: 0x020004AE RID: 1198
public class SoccerBall : BaseCombatEntity
{
	// Token: 0x0600273E RID: 10046 RVA: 0x000F4EFC File Offset: 0x000F30FC
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (collision.impulse.magnitude > 0f && collision.collider.attachedRigidbody != null && !collision.collider.attachedRigidbody.HasComponent<SoccerBall>())
		{
			Vector3 vector = this.rigidBody.position - collision.collider.attachedRigidbody.position;
			float magnitude = collision.impulse.magnitude;
			this.rigidBody.AddForce(vector * magnitude * this.additionalForceMultiplier + Vector3.up * magnitude * this.upForceMultiplier, ForceMode.Impulse);
		}
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000F4FB8 File Offset: 0x000F31B8
	public override void Hurt(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		float num = 0f;
		foreach (float num2 in info.damageTypes.types)
		{
			if ((int)num2 == 16 || (int)num2 == 22)
			{
				num += num2 * this.explosionForceMultiplier;
			}
			else
			{
				num += num2 * this.otherForceMultiplier;
			}
		}
		if (num > 3f)
		{
			this.rigidBody.AddExplosionForce(num, info.HitPositionWorld, 0.25f, 0.5f);
		}
		base.Hurt(info);
	}

	// Token: 0x04001FA9 RID: 8105
	[Header("Soccer Ball")]
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04001FAA RID: 8106
	[SerializeField]
	private float additionalForceMultiplier = 0.2f;

	// Token: 0x04001FAB RID: 8107
	[SerializeField]
	private float upForceMultiplier = 0.15f;

	// Token: 0x04001FAC RID: 8108
	[SerializeField]
	private DamageRenderer damageRenderer;

	// Token: 0x04001FAD RID: 8109
	[SerializeField]
	private float explosionForceMultiplier = 40f;

	// Token: 0x04001FAE RID: 8110
	[SerializeField]
	private float otherForceMultiplier = 10f;
}
