using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class Projectile : BaseMonoBehaviour
{
	// Token: 0x06001E24 RID: 7716 RVA: 0x000CDB84 File Offset: 0x000CBD84
	public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
	{
		float num = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
		float num2 = scale * (mod.damageOffset + mod.damageScale * num);
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			info.damageTypes.Add(damageTypeEntry.type, damageTypeEntry.amount * num2);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				" Projectile damage: ",
				info.damageTypes.Total(),
				" (scalar=",
				num2,
				")"
			}));
		}
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x000CDC8C File Offset: 0x000CBE8C
	public static uint FleshMaterialID()
	{
		if (Projectile._fleshMaterialID == 0U)
		{
			Projectile._fleshMaterialID = StringPool.Get("flesh");
		}
		return Projectile._fleshMaterialID;
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x000CDCA9 File Offset: 0x000CBEA9
	public static uint WaterMaterialID()
	{
		if (Projectile._waterMaterialID == 0U)
		{
			Projectile._waterMaterialID = StringPool.Get("Water");
		}
		return Projectile._waterMaterialID;
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000CDCC6 File Offset: 0x000CBEC6
	public static bool IsWaterMaterial(string hitMaterial)
	{
		if (Projectile.cachedWaterString == 0U)
		{
			Projectile.cachedWaterString = StringPool.Get("Water");
		}
		return StringPool.Get(hitMaterial) == Projectile.cachedWaterString;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000CDCF0 File Offset: 0x000CBEF0
	public static bool ShouldStopProjectile(RaycastHit hit)
	{
		BaseEntity entity = hit.GetEntity();
		return !(entity != null) || entity.ShouldBlockProjectiles();
	}

	// Token: 0x04001713 RID: 5907
	public const float moveDeltaTime = 0.03125f;

	// Token: 0x04001714 RID: 5908
	public const float lifeTime = 8f;

	// Token: 0x04001715 RID: 5909
	[Header("Attributes")]
	public Vector3 initialVelocity;

	// Token: 0x04001716 RID: 5910
	public float drag;

	// Token: 0x04001717 RID: 5911
	public float gravityModifier = 1f;

	// Token: 0x04001718 RID: 5912
	public float thickness;

	// Token: 0x04001719 RID: 5913
	[Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
	public float initialDistance;

	// Token: 0x0400171A RID: 5914
	[Header("Impact Rules")]
	public bool remainInWorld;

	// Token: 0x0400171B RID: 5915
	[Range(0f, 1f)]
	public float stickProbability = 1f;

	// Token: 0x0400171C RID: 5916
	[Range(0f, 1f)]
	public float breakProbability;

	// Token: 0x0400171D RID: 5917
	[Range(0f, 1f)]
	public float conditionLoss;

	// Token: 0x0400171E RID: 5918
	[Range(0f, 1f)]
	public float ricochetChance;

	// Token: 0x0400171F RID: 5919
	public float penetrationPower = 1f;

	// Token: 0x04001720 RID: 5920
	[Range(0f, 1f)]
	public float waterIntegrityLoss = 0.1f;

	// Token: 0x04001721 RID: 5921
	[Header("Damage")]
	public DamageProperties damageProperties;

	// Token: 0x04001722 RID: 5922
	[Horizontal(2, -1)]
	public MinMax damageDistances = new MinMax(10f, 100f);

	// Token: 0x04001723 RID: 5923
	[Horizontal(2, -1)]
	public MinMax damageMultipliers = new MinMax(1f, 0.8f);

	// Token: 0x04001724 RID: 5924
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04001725 RID: 5925
	[Header("Rendering")]
	public ScaleRenderer rendererToScale;

	// Token: 0x04001726 RID: 5926
	public ScaleRenderer firstPersonRenderer;

	// Token: 0x04001727 RID: 5927
	public bool createDecals = true;

	// Token: 0x04001728 RID: 5928
	[Header("Effects")]
	public bool doDefaultHitEffects = true;

	// Token: 0x04001729 RID: 5929
	[Header("Audio")]
	public SoundDefinition flybySound;

	// Token: 0x0400172A RID: 5930
	public float flybySoundDistance = 7f;

	// Token: 0x0400172B RID: 5931
	public SoundDefinition closeFlybySound;

	// Token: 0x0400172C RID: 5932
	public float closeFlybyDistance = 3f;

	// Token: 0x0400172D RID: 5933
	[Header("Tumble")]
	public float tumbleSpeed;

	// Token: 0x0400172E RID: 5934
	public Vector3 tumbleAxis = Vector3.right;

	// Token: 0x0400172F RID: 5935
	[Header("Swim")]
	public Vector3 swimScale;

	// Token: 0x04001730 RID: 5936
	public Vector3 swimSpeed;

	// Token: 0x04001731 RID: 5937
	[NonSerialized]
	public BasePlayer owner;

	// Token: 0x04001732 RID: 5938
	[NonSerialized]
	public AttackEntity sourceWeaponPrefab;

	// Token: 0x04001733 RID: 5939
	[NonSerialized]
	public Projectile sourceProjectilePrefab;

	// Token: 0x04001734 RID: 5940
	[NonSerialized]
	public ItemModProjectile mod;

	// Token: 0x04001735 RID: 5941
	[NonSerialized]
	public int projectileID;

	// Token: 0x04001736 RID: 5942
	[NonSerialized]
	public int seed;

	// Token: 0x04001737 RID: 5943
	[NonSerialized]
	public bool clientsideEffect;

	// Token: 0x04001738 RID: 5944
	[NonSerialized]
	public bool clientsideAttack;

	// Token: 0x04001739 RID: 5945
	[NonSerialized]
	public float integrity = 1f;

	// Token: 0x0400173A RID: 5946
	[NonSerialized]
	public float maxDistance = float.PositiveInfinity;

	// Token: 0x0400173B RID: 5947
	[NonSerialized]
	public Projectile.Modifier modifier = Projectile.Modifier.Default;

	// Token: 0x0400173C RID: 5948
	[NonSerialized]
	public bool invisible;

	// Token: 0x0400173D RID: 5949
	private static uint _fleshMaterialID;

	// Token: 0x0400173E RID: 5950
	private static uint _waterMaterialID;

	// Token: 0x0400173F RID: 5951
	private static uint cachedWaterString;

	// Token: 0x02000CB3 RID: 3251
	public struct Modifier
	{
		// Token: 0x040044EA RID: 17642
		public float damageScale;

		// Token: 0x040044EB RID: 17643
		public float damageOffset;

		// Token: 0x040044EC RID: 17644
		public float distanceScale;

		// Token: 0x040044ED RID: 17645
		public float distanceOffset;

		// Token: 0x040044EE RID: 17646
		public static Projectile.Modifier Default = new Projectile.Modifier
		{
			damageScale = 1f,
			damageOffset = 0f,
			distanceScale = 1f,
			distanceOffset = 0f
		};
	}
}
