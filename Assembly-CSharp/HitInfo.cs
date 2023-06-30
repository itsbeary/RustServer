using System;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000522 RID: 1314
public class HitInfo
{
	// Token: 0x060029F4 RID: 10740 RVA: 0x00101409 File Offset: 0x000FF609
	public bool IsProjectile()
	{
		return this.ProjectileID != 0;
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x060029F5 RID: 10741 RVA: 0x00101414 File Offset: 0x000FF614
	public global::BasePlayer InitiatorPlayer
	{
		get
		{
			if (!this.Initiator)
			{
				return null;
			}
			return this.Initiator.ToPlayer();
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x060029F6 RID: 10742 RVA: 0x00101430 File Offset: 0x000FF630
	public Vector3 attackNormal
	{
		get
		{
			return (this.PointEnd - this.PointStart).normalized;
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060029F7 RID: 10743 RVA: 0x00101456 File Offset: 0x000FF656
	public bool hasDamage
	{
		get
		{
			return this.damageTypes.Total() > 0f;
		}
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x0010146A File Offset: 0x000FF66A
	public HitInfo()
	{
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x001014A0 File Offset: 0x000FF6A0
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		this.Initiator = attacker;
		this.HitEntity = target;
		this.HitPositionWorld = vhitPosition;
		if (attacker != null)
		{
			this.PointStart = attacker.transform.position;
		}
		this.damageTypes.Add(type, damageAmount);
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x0010151C File Offset: 0x000FF71C
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount)
		: this(attacker, target, type, damageAmount, target.transform.position)
	{
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x00101534 File Offset: 0x000FF734
	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		this.HitEntity = null;
		this.PointStart = attack.pointStart;
		this.PointEnd = attack.pointEnd;
		if (attack.hitID.IsValid)
		{
			this.DidHit = true;
			if (serverSide)
			{
				this.HitEntity = global::BaseNetworkable.serverEntities.Find(attack.hitID) as global::BaseEntity;
			}
			if (this.HitEntity)
			{
				this.HitBone = attack.hitBone;
				this.HitPart = attack.hitPartID;
			}
		}
		this.DidHit = true;
		this.HitPositionLocal = attack.hitPositionLocal;
		this.HitPositionWorld = attack.hitPositionWorld;
		this.HitNormalLocal = attack.hitNormalLocal.normalized;
		this.HitNormalWorld = attack.hitNormalWorld.normalized;
		this.HitMaterial = attack.hitMaterialID;
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x060029FC RID: 10748 RVA: 0x00101604 File Offset: 0x000FF804
	public bool isHeadshot
	{
		get
		{
			if (this.HitEntity == null)
			{
				return false;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return false;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			return boneProperty != null && boneProperty.area == HitArea.Head;
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x060029FD RID: 10749 RVA: 0x00101668 File Offset: 0x000FF868
	public Translate.Phrase bonePhrase
	{
		get
		{
			if (this.HitEntity == null)
			{
				return null;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return null;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return null;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return null;
			}
			return boneProperty.name;
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x060029FE RID: 10750 RVA: 0x001016CC File Offset: 0x000FF8CC
	public string boneName
	{
		get
		{
			Translate.Phrase bonePhrase = this.bonePhrase;
			if (bonePhrase != null)
			{
				return bonePhrase.english;
			}
			return "N/A";
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x060029FF RID: 10751 RVA: 0x001016F0 File Offset: 0x000FF8F0
	public HitArea boneArea
	{
		get
		{
			if (this.HitEntity == null)
			{
				return (HitArea)(-1);
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return (HitArea)(-1);
			}
			return baseCombatEntity.SkeletonLookup(this.HitBone);
		}
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x00101730 File Offset: 0x000FF930
	public Vector3 PositionOnRay(Vector3 position)
	{
		Ray ray = new Ray(this.PointStart, this.attackNormal);
		if (this.ProjectilePrefab == null)
		{
			return ray.ClosestPoint(position);
		}
		Sphere sphere = new Sphere(position, this.ProjectilePrefab.thickness);
		RaycastHit raycastHit;
		if (sphere.Trace(ray, out raycastHit, float.PositiveInfinity))
		{
			return raycastHit.point;
		}
		return position;
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x00101793 File Offset: 0x000FF993
	public Vector3 HitPositionOnRay()
	{
		return this.PositionOnRay(this.HitPositionWorld);
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x001017A4 File Offset: 0x000FF9A4
	public bool IsNaNOrInfinity()
	{
		return this.PointStart.IsNaNOrInfinity() || this.PointEnd.IsNaNOrInfinity() || this.HitPositionWorld.IsNaNOrInfinity() || this.HitPositionLocal.IsNaNOrInfinity() || this.HitNormalWorld.IsNaNOrInfinity() || this.HitNormalLocal.IsNaNOrInfinity() || this.ProjectileVelocity.IsNaNOrInfinity() || float.IsNaN(this.ProjectileDistance) || float.IsInfinity(this.ProjectileDistance) || float.IsNaN(this.ProjectileIntegrity) || float.IsInfinity(this.ProjectileIntegrity) || float.IsNaN(this.ProjectileTravelTime) || float.IsInfinity(this.ProjectileTravelTime) || float.IsNaN(this.ProjectileTrajectoryMismatch) || float.IsInfinity(this.ProjectileTrajectoryMismatch);
	}

	// Token: 0x0400221F RID: 8735
	public global::BaseEntity Initiator;

	// Token: 0x04002220 RID: 8736
	public global::BaseEntity WeaponPrefab;

	// Token: 0x04002221 RID: 8737
	public AttackEntity Weapon;

	// Token: 0x04002222 RID: 8738
	public bool DoHitEffects = true;

	// Token: 0x04002223 RID: 8739
	public bool DoDecals = true;

	// Token: 0x04002224 RID: 8740
	public bool IsPredicting;

	// Token: 0x04002225 RID: 8741
	public bool UseProtection = true;

	// Token: 0x04002226 RID: 8742
	public Connection Predicted;

	// Token: 0x04002227 RID: 8743
	public bool DidHit;

	// Token: 0x04002228 RID: 8744
	public global::BaseEntity HitEntity;

	// Token: 0x04002229 RID: 8745
	public uint HitBone;

	// Token: 0x0400222A RID: 8746
	public uint HitPart;

	// Token: 0x0400222B RID: 8747
	public uint HitMaterial;

	// Token: 0x0400222C RID: 8748
	public Vector3 HitPositionWorld;

	// Token: 0x0400222D RID: 8749
	public Vector3 HitPositionLocal;

	// Token: 0x0400222E RID: 8750
	public Vector3 HitNormalWorld;

	// Token: 0x0400222F RID: 8751
	public Vector3 HitNormalLocal;

	// Token: 0x04002230 RID: 8752
	public Vector3 PointStart;

	// Token: 0x04002231 RID: 8753
	public Vector3 PointEnd;

	// Token: 0x04002232 RID: 8754
	public int ProjectileID;

	// Token: 0x04002233 RID: 8755
	public int ProjectileHits;

	// Token: 0x04002234 RID: 8756
	public float ProjectileDistance;

	// Token: 0x04002235 RID: 8757
	public float ProjectileIntegrity;

	// Token: 0x04002236 RID: 8758
	public float ProjectileTravelTime;

	// Token: 0x04002237 RID: 8759
	public float ProjectileTrajectoryMismatch;

	// Token: 0x04002238 RID: 8760
	public Vector3 ProjectileVelocity;

	// Token: 0x04002239 RID: 8761
	public Projectile ProjectilePrefab;

	// Token: 0x0400223A RID: 8762
	public PhysicMaterial material;

	// Token: 0x0400223B RID: 8763
	public DamageProperties damageProperties;

	// Token: 0x0400223C RID: 8764
	public DamageTypeList damageTypes = new DamageTypeList();

	// Token: 0x0400223D RID: 8765
	public bool CanGather;

	// Token: 0x0400223E RID: 8766
	public bool DidGather;

	// Token: 0x0400223F RID: 8767
	public float gatherScale = 1f;
}
