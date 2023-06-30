using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000507 RID: 1287
public class EffectDictionary
{
	// Token: 0x06002975 RID: 10613 RVA: 0x000FEC8A File Offset: 0x000FCE8A
	public static string GetParticle(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("impacts", impactType, materialName);
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000FEC98 File Offset: 0x000FCE98
	public static string GetParticle(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetParticle("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetParticle("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetParticle("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetParticle("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetParticle("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetParticle("blunt", materialName);
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000FED18 File Offset: 0x000FCF18
	public static string GetDecal(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("decals", impactType, materialName);
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000FED28 File Offset: 0x000FCF28
	public static string GetDecal(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetDecal("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetDecal("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetDecal("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetDecal("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetDecal("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetDecal("blunt", materialName);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000FEDA8 File Offset: 0x000FCFA8
	public static string GetDisplacement(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("displacement", impactType, materialName);
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000FEDB8 File Offset: 0x000FCFB8
	private static string LookupEffect(string category, string effect, string material)
	{
		if (EffectDictionary.effectDictionary == null)
		{
			EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
		}
		string text = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
		string[] array;
		if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(text, category, effect, material), out array) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(text, category, effect, "generic"), out array))
		{
			return string.Empty;
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x04002189 RID: 8585
	private static Dictionary<string, string[]> effectDictionary;
}
