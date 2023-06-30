using System;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
public class FlameExplosive : TimedExplosive
{
	// Token: 0x060022DE RID: 8926 RVA: 0x000E023C File Offset: 0x000DE43C
	public override void Explode()
	{
		this.FlameExplode(this.forceUpForExplosion ? Vector3.up : (-base.transform.forward));
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000E0264 File Offset: 0x000DE464
	public void FlameExplode(Vector3 surfaceNormal)
	{
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (this.blockCreateUnderwater && WaterLevel.Test(position, true, false, null))
		{
			base.Explode();
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		int num = 0;
		while ((float)num < this.numToCreate)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnExplode.resourcePath, position, default(Quaternion), true);
			if (baseEntity)
			{
				float num2 = (float)num / this.numToCreate;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle * this.spreadCurve.Evaluate(num2), surfaceNormal, true);
				baseEntity.transform.SetPositionAndRotation(position, Quaternion.LookRotation(modifiedAimConeDirection));
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.Spawn();
				Vector3 vector = modifiedAimConeDirection.normalized * UnityEngine.Random.Range(this.minVelocity, this.maxVelocity) * this.velocityCurve.Evaluate(num2 * UnityEngine.Random.Range(1f, 1.1f));
				FireBall component2 = baseEntity.GetComponent<FireBall>();
				if (component2 != null)
				{
					component2.SetDelayedVelocity(vector);
				}
				else
				{
					baseEntity.SetVelocity(vector);
				}
			}
			num++;
		}
		base.Explode();
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000E03C5 File Offset: 0x000DE5C5
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.FlameExplode(info.normal);
	}

	// Token: 0x04001AB9 RID: 6841
	public GameObjectRef createOnExplode;

	// Token: 0x04001ABA RID: 6842
	public bool blockCreateUnderwater;

	// Token: 0x04001ABB RID: 6843
	public float numToCreate = 10f;

	// Token: 0x04001ABC RID: 6844
	public float minVelocity = 2f;

	// Token: 0x04001ABD RID: 6845
	public float maxVelocity = 5f;

	// Token: 0x04001ABE RID: 6846
	public float spreadAngle = 90f;

	// Token: 0x04001ABF RID: 6847
	public bool forceUpForExplosion;

	// Token: 0x04001AC0 RID: 6848
	public AnimationCurve velocityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04001AC1 RID: 6849
	public AnimationCurve spreadCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});
}
