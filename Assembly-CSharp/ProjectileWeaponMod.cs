using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000438 RID: 1080
public class ProjectileWeaponMod : BaseEntity
{
	// Token: 0x06002492 RID: 9362 RVA: 0x000E8A16 File Offset: 0x000E6C16
	public override void ServerInit()
	{
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		base.ServerInit();
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000E8A29 File Offset: 0x000E6C29
	public override void PostServerLoad()
	{
		base.limitNetworking = base.HasFlag(BaseEntity.Flags.Disabled);
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000E8A3C File Offset: 0x000E6C3C
	public static float Mult(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		float num = 1f;
		foreach (float num2 in mods)
		{
			num *= num2;
		}
		return num;
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000E8A9C File Offset: 0x000E6C9C
	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Sum();
		}
		return def;
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000E8ACC File Offset: 0x000E6CCC
	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Average();
		}
		return def;
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000E8AFC File Offset: 0x000E6CFC
	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Max();
		}
		return def;
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000E8B2C File Offset: 0x000E6D2C
	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Min();
		}
		return def;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000E8B5C File Offset: 0x000E6D5C
	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value)
	{
		return (from x in (from ProjectileWeaponMod x in parentEnt.children
				where x != null && (!x.needsOnForEffects || x.HasFlag(BaseEntity.Flags.On))
				select x).Select(selector_modifier)
			where x.enabled
			select x).Select(selector_value);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000E8BC8 File Offset: 0x000E6DC8
	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		return parentEnt.children.Cast<ProjectileWeaponMod>().Any((ProjectileWeaponMod x) => x != null && x.IsBroken());
	}

	// Token: 0x04001C69 RID: 7273
	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	// Token: 0x04001C6A RID: 7274
	public bool isSilencer;

	// Token: 0x04001C6B RID: 7275
	[Header("Weapon Basics")]
	public ProjectileWeaponMod.Modifier repeatDelay;

	// Token: 0x04001C6C RID: 7276
	public ProjectileWeaponMod.Modifier projectileVelocity;

	// Token: 0x04001C6D RID: 7277
	public ProjectileWeaponMod.Modifier projectileDamage;

	// Token: 0x04001C6E RID: 7278
	public ProjectileWeaponMod.Modifier projectileDistance;

	// Token: 0x04001C6F RID: 7279
	[Header("Recoil")]
	public ProjectileWeaponMod.Modifier aimsway;

	// Token: 0x04001C70 RID: 7280
	public ProjectileWeaponMod.Modifier aimswaySpeed;

	// Token: 0x04001C71 RID: 7281
	public ProjectileWeaponMod.Modifier recoil;

	// Token: 0x04001C72 RID: 7282
	[Header("Aim Cone")]
	public ProjectileWeaponMod.Modifier sightAimCone;

	// Token: 0x04001C73 RID: 7283
	public ProjectileWeaponMod.Modifier hipAimCone;

	// Token: 0x04001C74 RID: 7284
	[Header("Light Effects")]
	public bool isLight;

	// Token: 0x04001C75 RID: 7285
	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	// Token: 0x04001C76 RID: 7286
	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	// Token: 0x04001C77 RID: 7287
	[Header("Scope")]
	public bool isScope;

	// Token: 0x04001C78 RID: 7288
	public float zoomAmountDisplayOnly;

	// Token: 0x04001C79 RID: 7289
	[Header("Magazine")]
	public ProjectileWeaponMod.Modifier magazineCapacity;

	// Token: 0x04001C7A RID: 7290
	public bool needsOnForEffects;

	// Token: 0x04001C7B RID: 7291
	[Header("Burst")]
	public int burstCount = -1;

	// Token: 0x04001C7C RID: 7292
	public float timeBetweenBursts;

	// Token: 0x02000CFD RID: 3325
	[Serializable]
	public struct Modifier
	{
		// Token: 0x04004636 RID: 17974
		public bool enabled;

		// Token: 0x04004637 RID: 17975
		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		// Token: 0x04004638 RID: 17976
		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}
}
