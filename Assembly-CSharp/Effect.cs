using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class Effect : EffectData
{
	// Token: 0x06001F45 RID: 8005 RVA: 0x000D4342 File Offset: 0x000D2542
	public Effect()
	{
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000D434A File Offset: 0x000D254A
	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000D4364 File Offset: 0x000D2564
	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000D4384 File Offset: 0x000D2584
	public void Init(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = true;
		this.origin = posLocal;
		this.normal = normLocal;
		this.gameObject = null;
		this.Up = Vector3.zero;
		if (ent != null && !ent.IsValid())
		{
			Debug.LogWarning("Effect.Init - invalid entity");
		}
		this.entity = (ent.IsValid() ? ent.net.ID : default(NetworkableId));
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
		this.bone = boneID;
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000D4424 File Offset: 0x000D2624
	public void Init(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = false;
		this.worldPos = posWorld;
		this.worldNrm = normWorld;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.entity = default(NetworkableId);
		this.origin = this.worldPos;
		this.normal = this.worldNrm;
		this.bone = 0U;
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000D44A5 File Offset: 0x000D26A5
	public void Clear()
	{
		this.worldPos = Vector3.zero;
		this.worldNrm = Vector3.zero;
		this.attached = false;
		this.transform = null;
		this.gameObject = null;
		this.pooledString = null;
		this.broadcast = false;
	}

	// Token: 0x04001848 RID: 6216
	public Vector3 Up;

	// Token: 0x04001849 RID: 6217
	public Vector3 worldPos;

	// Token: 0x0400184A RID: 6218
	public Vector3 worldNrm;

	// Token: 0x0400184B RID: 6219
	public bool attached;

	// Token: 0x0400184C RID: 6220
	public Transform transform;

	// Token: 0x0400184D RID: 6221
	public GameObject gameObject;

	// Token: 0x0400184E RID: 6222
	public string pooledString;

	// Token: 0x0400184F RID: 6223
	public bool broadcast;

	// Token: 0x04001850 RID: 6224
	private static Effect reusableInstace = new Effect();

	// Token: 0x02000CC5 RID: 3269
	public enum Type : uint
	{
		// Token: 0x0400454F RID: 17743
		Generic,
		// Token: 0x04004550 RID: 17744
		Projectile,
		// Token: 0x04004551 RID: 17745
		GenericGlobal
	}

	// Token: 0x02000CC6 RID: 3270
	public static class client
	{
		// Token: 0x06004FD1 RID: 20433 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x001A733E File Offset: 0x001A553E
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = default(Vector3))
		{
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x001A733E File Offset: 0x001A553E
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Vector3 up = default(Vector3), Effect.Type overrideType = Effect.Type.Generic)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x001A733E File Offset: 0x001A553E
		public static void Run(string strName, GameObject obj)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x001A7348 File Offset: 0x001A5548
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.client.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + info.HitNormalLocal * 0.1f, info.HitNormalLocal);
				return;
			}
			Effect.client.Run(effectName, info.HitPositionWorld + info.HitNormalWorld * 0.1f, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x001A73C8 File Offset: 0x001A55C8
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string text = StringPool.Get(info.HitMaterial);
			string text2 = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), text);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), text);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, false, null))
			{
				return;
			}
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					text2 = impactEffect.resourcePath;
				}
				Effect.client.Run(text2, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			else
			{
				Effect.Type type = Effect.Type.Generic;
				Effect.client.Run(text2, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), type);
				Effect.client.Run(decal, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), type);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(text);
					if (info.HitEntity.IsValid())
					{
						Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
					else
					{
						Effect.client.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	// Token: 0x02000CC7 RID: 3271
	public static class server
	{
		// Token: 0x06004FD8 RID: 20440 RVA: 0x001A75BE File Offset: 0x001A57BE
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x001A75E8 File Offset: 0x001A57E8
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x001A7626 File Offset: 0x001A5826
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x001A764C File Offset: 0x001A584C
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004FDC RID: 20444 RVA: 0x001A7688 File Offset: 0x001A5888
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				return;
			}
			Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
		}

		// Token: 0x06004FDD RID: 20445 RVA: 0x001A76E4 File Offset: 0x001A58E4
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string text = StringPool.Get(info.HitMaterial);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, false, null))
			{
				return;
			}
			string text2 = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), text);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), text);
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					text2 = impactEffect.resourcePath;
				}
				Bounds bounds = info.HitEntity.bounds;
				float num = info.HitEntity.BoundsPadding();
				bounds.extents += new Vector3(num, num, num);
				if (!bounds.Contains(info.HitPositionLocal))
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					if (initiatorPlayer != null && initiatorPlayer.GetType() == typeof(BasePlayer))
					{
						float num2 = Mathf.Sqrt(bounds.SqrDistance(info.HitPositionLocal));
						AntiHack.Log(initiatorPlayer, AntiHackType.EffectHack, string.Format("Tried to run an impact effect outside of entity '{0}' bounds by {1}m", info.HitEntity.ShortPrefabName, num2));
					}
					return;
				}
				Effect.server.Run(text2, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
			}
			else
			{
				Effect.server.Run(text2, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(text);
					if (info.HitEntity.IsValid())
					{
						Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
					}
					else
					{
						Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}
}
