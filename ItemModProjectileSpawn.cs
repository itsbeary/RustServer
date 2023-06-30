using System;
using ConVar;
using UnityEngine;

// Token: 0x02000600 RID: 1536
public class ItemModProjectileSpawn : ItemModProjectile
{
	// Token: 0x06002DD1 RID: 11729 RVA: 0x00113C80 File Offset: 0x00111E80
	public override void ServerProjectileHit(HitInfo info)
	{
		for (int i = 0; i < this.numToCreateChances; i++)
		{
			if (this.createOnImpact.isValid && UnityEngine.Random.Range(0f, 1f) < this.createOnImpactChance)
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 pointStart = info.PointStart;
				Vector3 normalized = info.ProjectileVelocity.normalized;
				Vector3 normalized2 = info.HitNormalWorld.normalized;
				Vector3 vector = hitPositionWorld - normalized * 0.1f;
				Quaternion quaternion = Quaternion.LookRotation(-normalized);
				int num = (ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688);
				if (GamePhysics.LineOfSight(pointStart, vector, num, null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnImpact.resourcePath, default(Vector3), default(Quaternion), true);
					if (baseEntity)
					{
						baseEntity.transform.position = vector;
						baseEntity.transform.rotation = quaternion;
						baseEntity.Spawn();
						if (this.spreadAngle > 0f)
						{
							Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, normalized2, true);
							baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(1f, 3f));
						}
					}
				}
			}
		}
		base.ServerProjectileHit(info);
	}

	// Token: 0x04002583 RID: 9603
	public float createOnImpactChance;

	// Token: 0x04002584 RID: 9604
	public GameObjectRef createOnImpact = new GameObjectRef();

	// Token: 0x04002585 RID: 9605
	public float spreadAngle = 30f;

	// Token: 0x04002586 RID: 9606
	public float spreadVelocityMin = 1f;

	// Token: 0x04002587 RID: 9607
	public float spreadVelocityMax = 3f;

	// Token: 0x04002588 RID: 9608
	public int numToCreateChances = 1;
}
