using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007D RID: 125
public class GrowableEntity : BaseCombatEntity, IInstanceDataReceiver
{
	// Token: 0x06000B99 RID: 2969 RVA: 0x00066A88 File Offset: 0x00064C88
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GrowableEntity.OnRpcMessage", 0))
		{
			if (rpc == 759768385U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EatFruit ");
				}
				using (TimeWarning.New("RPC_EatFruit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(759768385U, "RPC_EatFruit", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(759768385U, "RPC_EatFruit", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_EatFruit(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_EatFruit");
					}
				}
				return true;
			}
			if (rpc == 598660365U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickFruit ");
				}
				using (TimeWarning.New("RPC_PickFruit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(598660365U, "RPC_PickFruit", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(598660365U, "RPC_PickFruit", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PickFruit(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_PickFruit");
					}
				}
				return true;
			}
			if (rpc == 3465633431U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickFruitAll ");
				}
				using (TimeWarning.New("RPC_PickFruitAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3465633431U, "RPC_PickFruitAll", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3465633431U, "RPC_PickFruitAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PickFruitAll(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_PickFruitAll");
					}
				}
				return true;
			}
			if (rpc == 1959480148U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RemoveDying ");
				}
				using (TimeWarning.New("RPC_RemoveDying", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1959480148U, "RPC_RemoveDying", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RemoveDying(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_RemoveDying");
					}
				}
				return true;
			}
			if (rpc == 1771718099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RemoveDyingAll ");
				}
				using (TimeWarning.New("RPC_RemoveDyingAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1771718099U, "RPC_RemoveDyingAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RemoveDyingAll(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_RemoveDyingAll");
					}
				}
				return true;
			}
			if (rpc == 232075937U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestQualityUpdate ");
				}
				using (TimeWarning.New("RPC_RequestQualityUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(232075937U, "RPC_RequestQualityUpdate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestQualityUpdate(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_RequestQualityUpdate");
					}
				}
				return true;
			}
			if (rpc == 2222960834U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeClone ");
				}
				using (TimeWarning.New("RPC_TakeClone", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2222960834U, "RPC_TakeClone", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeClone(rpcmessage7);
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_TakeClone");
					}
				}
				return true;
			}
			if (rpc == 95639240U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeCloneAll ");
				}
				using (TimeWarning.New("RPC_TakeCloneAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(95639240U, "RPC_TakeCloneAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeCloneAll(rpcmessage8);
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_TakeCloneAll");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00067608 File Offset: 0x00065808
	public void QueueForQualityUpdate()
	{
		global::GrowableEntity.growableEntityUpdateQueue.Add(this);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x00067618 File Offset: 0x00065818
	public void CalculateQualities(bool firstTime, bool forceArtificialLightUpdates = false, bool forceArtificialTemperatureUpdates = false)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.sunExposure == null)
		{
			this.sunExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 30f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.SunRaycast)
			};
		}
		if (this.artificialLightExposure == null)
		{
			this.artificialLightExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 60f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.CalculateArtificialLightExposure)
			};
		}
		if (this.artificialTemperatureExposure == null)
		{
			this.artificialTemperatureExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 60f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.CalculateArtificialTemperature)
			};
		}
		if (forceArtificialTemperatureUpdates)
		{
			this.artificialTemperatureExposure.ForceNextRun();
		}
		this.CalculateLightQuality(forceArtificialLightUpdates || firstTime);
		this.CalculateWaterQuality();
		this.CalculateWaterConsumption();
		this.CalculateGroundQuality(firstTime);
		this.CalculateTemperatureQuality();
		this.CalculateOverallQuality();
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x00067716 File Offset: 0x00065916
	private void CalculateQualities_Water()
	{
		this.CalculateWaterQuality();
		this.CalculateWaterConsumption();
		this.CalculateOverallQuality();
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0006772C File Offset: 0x0006592C
	public void CalculateLightQuality(bool forceArtificalUpdate)
	{
		float num = Mathf.Clamp01(this.Properties.timeOfDayHappiness.Evaluate(TOD_Sky.Instance.Cycle.Hour));
		if (!ConVar.Server.plantlightdetection)
		{
			this.LightQuality = num;
			return;
		}
		this.LightQuality = this.CalculateSunExposure(forceArtificalUpdate) * num;
		if (this.LightQuality <= 0f)
		{
			this.LightQuality = this.GetArtificialLightExposure(forceArtificalUpdate);
		}
		this.LightQuality = global::GrowableEntity.RemapValue(this.LightQuality, 0f, this.Properties.OptimalLightQuality, 0f, 1f);
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x000677C4 File Offset: 0x000659C4
	private float CalculateSunExposure(bool force)
	{
		if (TOD_Sky.Instance.IsNight)
		{
			return 0f;
		}
		if (this.GetPlanter() != null)
		{
			return this.GetPlanter().GetSunExposure();
		}
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(force);
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00067813 File Offset: 0x00065A13
	private float SunRaycast()
	{
		return global::GrowableEntity.SunRaycast(base.transform.position + new Vector3(0f, 1f, 0f));
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0006783E File Offset: 0x00065A3E
	private float GetArtificialLightExposure(bool force)
	{
		if (this.GetPlanter() != null)
		{
			return this.GetPlanter().GetArtificialLightExposure();
		}
		TimeCachedValue<float> timeCachedValue = this.artificialLightExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(force);
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00067870 File Offset: 0x00065A70
	private float CalculateArtificialLightExposure()
	{
		return global::GrowableEntity.CalculateArtificialLightExposure(base.transform);
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00067880 File Offset: 0x00065A80
	public static float CalculateArtificialLightExposure(Transform forTransform)
	{
		float num = 0f;
		List<CeilingLight> list = Facepunch.Pool.GetList<CeilingLight>();
		global::Vis.Entities<CeilingLight>(forTransform.position + new Vector3(0f, ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 256, QueryTriggerInteraction.Collide);
		using (List<CeilingLight>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsOn())
				{
					num = 1f;
					break;
				}
			}
		}
		Facepunch.Pool.FreeList<CeilingLight>(ref list);
		return num;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0006791C File Offset: 0x00065B1C
	public static float SunRaycast(Vector3 checkPosition)
	{
		Vector3 normalized = (TOD_Sky.Instance.Components.Sun.transform.position - checkPosition).normalized;
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(checkPosition, normalized, out raycastHit, 100f, 10551297))
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x00067974 File Offset: 0x00065B74
	public void CalculateWaterQuality()
	{
		if (this.GetPlanter() != null)
		{
			float soilSaturationFraction = this.planter.soilSaturationFraction;
			if (soilSaturationFraction > ConVar.Server.optimalPlanterQualitySaturation)
			{
				this.WaterQuality = global::GrowableEntity.RemapValue(soilSaturationFraction, ConVar.Server.optimalPlanterQualitySaturation, 1f, 1f, 0.6f);
			}
			else
			{
				this.WaterQuality = global::GrowableEntity.RemapValue(soilSaturationFraction, 0f, ConVar.Server.optimalPlanterQualitySaturation, 0f, 1f);
			}
		}
		else
		{
			TerrainBiome.Enum biomeMaxType = (TerrainBiome.Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(base.transform.position, -1);
			if (biomeMaxType - TerrainBiome.Enum.Arid > 1 && biomeMaxType != TerrainBiome.Enum.Tundra)
			{
				if (biomeMaxType == TerrainBiome.Enum.Arctic)
				{
					this.WaterQuality = 0.1f;
				}
				else
				{
					this.WaterQuality = 0f;
				}
			}
			else
			{
				this.WaterQuality = 0.3f;
			}
		}
		this.WaterQuality = Mathf.Clamp01(this.WaterQuality);
		this.WaterQuality = global::GrowableEntity.RemapValue(this.WaterQuality, 0f, this.Properties.OptimalWaterQuality, 0f, 1f);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x00067A70 File Offset: 0x00065C70
	public void CalculateGroundQuality(bool firstCheck)
	{
		if (this.underWater && !firstCheck)
		{
			this.GroundQuality = 0f;
			return;
		}
		if (firstCheck)
		{
			Vector3 position = base.transform.position;
			if (WaterLevel.Test(position, true, true, this))
			{
				this.underWater = true;
				this.GroundQuality = 0f;
				return;
			}
			this.underWater = false;
			this.terrainTypeValue = this.GetGroundTypeValue(position);
		}
		if (this.GetPlanter() != null)
		{
			this.GroundQuality = 0.6f;
			this.GroundQuality += (this.Fertilized ? 0.4f : 0f);
		}
		else
		{
			this.GroundQuality = this.terrainTypeValue;
			float num = (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.2f;
			float num2 = this.GroundQuality + num;
			this.GroundQuality = Mathf.Min(0.6f, num2);
		}
		this.GroundQuality = global::GrowableEntity.RemapValue(this.GroundQuality, 0f, this.Properties.OptimalGroundQuality, 0f, 1f);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x00067B78 File Offset: 0x00065D78
	private float GetGroundTypeValue(Vector3 pos)
	{
		TerrainSplat.Enum splatMaxType = (TerrainSplat.Enum)TerrainMeta.SplatMap.GetSplatMaxType(pos, -1);
		if (splatMaxType <= TerrainSplat.Enum.Grass)
		{
			switch (splatMaxType)
			{
			case TerrainSplat.Enum.Dirt:
				return 0.3f;
			case TerrainSplat.Enum.Snow:
				return 0f;
			case (TerrainSplat.Enum)3:
				break;
			case TerrainSplat.Enum.Sand:
				return 0f;
			default:
				if (splatMaxType == TerrainSplat.Enum.Rock)
				{
					return 0f;
				}
				if (splatMaxType == TerrainSplat.Enum.Grass)
				{
					return 0.3f;
				}
				break;
			}
		}
		else
		{
			if (splatMaxType == TerrainSplat.Enum.Forest)
			{
				return 0.2f;
			}
			if (splatMaxType == TerrainSplat.Enum.Stones)
			{
				return 0f;
			}
			if (splatMaxType == TerrainSplat.Enum.Gravel)
			{
				return 0f;
			}
		}
		return 0.5f;
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00067C04 File Offset: 0x00065E04
	private void CalculateTemperatureQuality()
	{
		this.TemperatureQuality = Mathf.Clamp01(this.Properties.temperatureHappiness.Evaluate(this.CurrentTemperature));
		float num = (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.05f;
		this.TemperatureQuality = Mathf.Clamp01(this.TemperatureQuality + num);
		this.TemperatureQuality = global::GrowableEntity.RemapValue(this.TemperatureQuality, 0f, this.Properties.OptimalTemperatureQuality, 0f, 1f);
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x00067C84 File Offset: 0x00065E84
	public float CalculateOverallQuality()
	{
		float num = 1f;
		if (ConVar.Server.useMinimumPlantCondition)
		{
			num = Mathf.Min(num, this.LightQuality);
			num = Mathf.Min(num, this.WaterQuality);
			num = Mathf.Min(num, this.GroundQuality);
			num = Mathf.Min(num, this.TemperatureQuality);
		}
		else
		{
			num = this.LightQuality * this.WaterQuality * this.GroundQuality * this.TemperatureQuality;
		}
		this.OverallQuality = num;
		return this.OverallQuality;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x00067D00 File Offset: 0x00065F00
	public void CalculateWaterConsumption()
	{
		float num = this.Properties.temperatureWaterRequirementMultiplier.Evaluate(this.CurrentTemperature);
		float num2 = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.WaterRequirement) * 0.1f;
		this.WaterConsumption = this.Properties.WaterIntake * num * num2;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x00067D53 File Offset: 0x00065F53
	private float CalculateArtificialTemperature()
	{
		return global::GrowableEntity.CalculateArtificialTemperature(base.transform);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00067D60 File Offset: 0x00065F60
	public static float CalculateArtificialTemperature(Transform forTransform)
	{
		Vector3 position = forTransform.position;
		List<GrowableHeatSource> list = Facepunch.Pool.GetList<GrowableHeatSource>();
		global::Vis.Components<GrowableHeatSource>(position, ConVar.Server.artificialTemperatureGrowableRange, list, 256, QueryTriggerInteraction.Collide);
		float num = 0f;
		foreach (GrowableHeatSource growableHeatSource in list)
		{
			num = Mathf.Max(growableHeatSource.ApplyHeat(position), num);
		}
		Facepunch.Pool.FreeList<GrowableHeatSource>(ref list);
		return num;
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00067DE0 File Offset: 0x00065FE0
	public int CalculateMarketValue()
	{
		int num = this.Properties.BaseMarketValue;
		int num2 = this.Genes.GetPositiveGeneCount() * 10;
		int num3 = this.Genes.GetNegativeGeneCount() * -10;
		num += num2;
		num += num3;
		return Mathf.Max(0, num);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00067E28 File Offset: 0x00066028
	private static float RemapValue(float inValue, float minA, float maxA, float minB, float maxB)
	{
		if (inValue >= maxA)
		{
			return maxB;
		}
		float num = Mathf.InverseLerp(minA, maxA, inValue);
		return Mathf.Lerp(minB, maxB, num);
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x00067E4E File Offset: 0x0006604E
	public bool IsFood()
	{
		return this.Properties.pickupItem.category == ItemCategory.Food && this.Properties.pickupItem.GetComponent<ItemModConsume>() != null;
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000BAF RID: 2991 RVA: 0x00067E7C File Offset: 0x0006607C
	public float CurrentTemperature
	{
		get
		{
			if (this.GetPlanter() != null)
			{
				return this.GetPlanter().GetPlantTemperature();
			}
			float temperature = Climate.GetTemperature(base.transform.position);
			TimeCachedValue<float> timeCachedValue = this.artificialTemperatureExposure;
			return temperature + ((timeCachedValue != null) ? timeCachedValue.Get(false) : 0f);
		}
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00067ECC File Offset: 0x000660CC
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RunUpdate), global::GrowableEntity.ThinkDeltaTime, global::GrowableEntity.ThinkDeltaTime, global::GrowableEntity.ThinkDeltaTime * 0.1f);
		base.health = 10f;
		this.ResetSeason();
		this.Genes.GenerateRandom(this);
		if (!Rust.Application.isLoadingSave)
		{
			this.CalculateQualities(true, false, false);
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00067F34 File Offset: 0x00066134
	public PlanterBox GetPlanter()
	{
		if (this.planter == null)
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				this.planter = parentEntity as PlanterBox;
			}
		}
		return this.planter;
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00067F71 File Offset: 0x00066171
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		this.planter = newParent as PlanterBox;
		if (!Rust.Application.isLoadingSave && this.planter != null)
		{
			this.planter.FertilizeGrowables();
		}
		this.CalculateQualities(true, false, false);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00067FB0 File Offset: 0x000661B0
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.CalculateQualities(true, false, false);
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00067FC1 File Offset: 0x000661C1
	public void ResetSeason()
	{
		this.Yield = 0f;
		this.yieldPool = 0f;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00067FDC File Offset: 0x000661DC
	private void RunUpdate()
	{
		if (this.IsDead())
		{
			return;
		}
		this.CalculateQualities(false, false, false);
		float num = this.CalculateOverallQuality();
		float num2 = this.UpdateAge(num);
		this.UpdateHealthAndYield(num, num2);
		if (base.health <= 0f)
		{
			this.Die(null);
			return;
		}
		this.UpdateState();
		this.ConsumeWater();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0006803C File Offset: 0x0006623C
	private float UpdateAge(float overallQuality)
	{
		this.Age += this.growDeltaTime;
		float num = (this.currentStage.IgnoreConditions ? 1f : (Mathf.Max(overallQuality, 0f) * this.GetGrowthBonus(overallQuality)));
		float num2 = this.growDeltaTime * num;
		this.stageAge += num2;
		return num2;
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0006809C File Offset: 0x0006629C
	private void UpdateHealthAndYield(float overallQuality, float actualStageAgeIncrease)
	{
		if (this.GetPlanter() == null && UnityEngine.Random.Range(0f, 1f) <= ConVar.Server.nonPlanterDeathChancePerTick)
		{
			base.health = 0f;
			return;
		}
		if (overallQuality <= 0f)
		{
			this.ApplyDeathRate();
		}
		base.health += overallQuality * this.currentStage.health * this.growDeltaTime;
		if (this.yieldPool > 0f)
		{
			float num = this.currentStage.yield / (this.currentStage.lifeLengthSeconds / this.growDeltaTime);
			float num2 = Mathf.Min(this.yieldPool, num * (actualStageAgeIncrease / this.growDeltaTime));
			this.yieldPool -= num;
			float num3 = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) * 0.25f;
			this.Yield += num2 * 1f * num3;
		}
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0006818C File Offset: 0x0006638C
	private void ApplyDeathRate()
	{
		float num = 0f;
		if (this.WaterQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.LightQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.GroundQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.TemperatureQuality <= 0f)
		{
			num += 0.1f;
		}
		base.health -= num;
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00068204 File Offset: 0x00066404
	private float GetGrowthBonus(float overallQuality)
	{
		float num = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.GrowthSpeed) * 0.25f;
		if (overallQuality <= 0f)
		{
			num = 1f;
		}
		return num;
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0006823C File Offset: 0x0006643C
	private PlantProperties.State UpdateState()
	{
		if (this.stageAge <= this.currentStage.lifeLengthSeconds)
		{
			return this.State;
		}
		if (this.State == PlantProperties.State.Dying)
		{
			this.Die(null);
			return PlantProperties.State.Dying;
		}
		if (this.currentStage.nextState <= this.State)
		{
			this.seasons++;
		}
		if (this.seasons >= this.Properties.MaxSeasons)
		{
			this.ChangeState(PlantProperties.State.Dying, true, false);
		}
		else
		{
			this.ChangeState(this.currentStage.nextState, true, false);
		}
		return this.State;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x000682D0 File Offset: 0x000664D0
	private void ConsumeWater()
	{
		if (this.State == PlantProperties.State.Dying)
		{
			return;
		}
		if (this.GetPlanter() == null)
		{
			return;
		}
		int num = Mathf.CeilToInt(Mathf.Min((float)this.planter.soilSaturation, this.WaterConsumption));
		if ((float)num > 0f)
		{
			this.planter.ConsumeWater(num, this);
		}
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0006832A File Offset: 0x0006652A
	public void Fertilize()
	{
		if (this.Fertilized)
		{
			return;
		}
		this.Fertilized = true;
		this.CalculateQualities(false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0006834C File Offset: 0x0006654C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeClone(global::BaseEntity.RPCMessage msg)
	{
		this.TakeClones(msg.player);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0006835C File Offset: 0x0006655C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeCloneAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity growableEntity;
				if (baseEntity != this && (growableEntity = baseEntity as global::GrowableEntity) != null)
				{
					list.Add(growableEntity);
				}
			}
			foreach (global::GrowableEntity growableEntity2 in list)
			{
				growableEntity2.TakeClones(msg.player);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.TakeClones(msg.player);
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x00068434 File Offset: 0x00066634
	private void TakeClones(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!this.CanClone())
		{
			return;
		}
		int num = this.Properties.BaseCloneCount + this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) / 2;
		if (num <= 0)
		{
			return;
		}
		global::Item item = ItemManager.Create(this.Properties.CloneItem, num, 0UL);
		GrowableGeneEncoding.EncodeGenesToItem(this, item);
		Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, this, player, null);
		player.GiveItem(item, global::BaseEntity.GiveItemReason.ResourceHarvested);
		if (this.Properties.pickEffect.isValid)
		{
			Effect.server.Run(this.Properties.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		this.Die(null);
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x000684F0 File Offset: 0x000666F0
	public void PickFruit(global::BasePlayer player, bool eat = false)
	{
		if (!this.CanPick())
		{
			return;
		}
		this.harvests++;
		this.GiveFruit(player, this.CurrentPickAmount, eat);
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(this.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(player, base.transform.position);
		}
		this.ResetSeason();
		if (this.Properties.pickEffect.isValid)
		{
			Effect.server.Run(this.Properties.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.harvests < this.Properties.maxHarvests)
		{
			this.ChangeState(PlantProperties.State.Mature, true, false);
			return;
		}
		if (this.Properties.disappearAfterHarvest)
		{
			this.Die(null);
			return;
		}
		this.ChangeState(PlantProperties.State.Dying, true, false);
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x000685C8 File Offset: 0x000667C8
	private void GiveFruit(global::BasePlayer player, int amount, bool eat)
	{
		if (amount <= 0)
		{
			return;
		}
		bool enabled = this.Properties.pickupItem.condition.enabled;
		if (enabled)
		{
			for (int i = 0; i < amount; i++)
			{
				this.GiveFruit(player, 1, enabled, eat);
			}
			return;
		}
		this.GiveFruit(player, amount, enabled, eat);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00068614 File Offset: 0x00066814
	private void GiveFruit(global::BasePlayer player, int amount, bool applyCondition, bool eat)
	{
		global::Item item = ItemManager.Create(this.Properties.pickupItem, amount, 0UL);
		if (applyCondition)
		{
			item.conditionNormalized = this.Properties.fruitVisualScaleCurve.Evaluate(this.StageProgressFraction);
		}
		if (eat && player != null && this.IsFood())
		{
			ItemModConsume component = item.info.GetComponent<ItemModConsume>();
			if (component != null)
			{
				component.DoAction(item, player);
				return;
			}
		}
		if (player != null)
		{
			Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, this, player, null);
			player.GiveItem(item, global::BaseEntity.GiveItemReason.ResourceHarvested);
			return;
		}
		item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up * 1f, default(Quaternion));
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x000686F0 File Offset: 0x000668F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickFruit(global::BaseEntity.RPCMessage msg)
	{
		this.PickFruit(msg.player, false);
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x000686FF File Offset: 0x000668FF
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_EatFruit(global::BaseEntity.RPCMessage msg)
	{
		this.PickFruit(msg.player, true);
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x00068710 File Offset: 0x00066910
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickFruitAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity growableEntity;
				if (baseEntity != this && (growableEntity = baseEntity as global::GrowableEntity) != null)
				{
					list.Add(growableEntity);
				}
			}
			foreach (global::GrowableEntity growableEntity2 in list)
			{
				growableEntity2.PickFruit(msg.player, false);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.PickFruit(msg.player, false);
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x000687E8 File Offset: 0x000669E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_RemoveDying(global::BaseEntity.RPCMessage msg)
	{
		this.RemoveDying(msg.player);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x000687F8 File Offset: 0x000669F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_RemoveDyingAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity growableEntity;
				if (baseEntity != this && (growableEntity = baseEntity as global::GrowableEntity) != null)
				{
					list.Add(growableEntity);
				}
			}
			foreach (global::GrowableEntity growableEntity2 in list)
			{
				growableEntity2.RemoveDying(msg.player);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.RemoveDying(msg.player);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x000688D0 File Offset: 0x00066AD0
	public void RemoveDying(global::BasePlayer receiver)
	{
		if (this.State != PlantProperties.State.Dying)
		{
			return;
		}
		if (this.Properties.removeDyingItem == null)
		{
			return;
		}
		if (this.Properties.removeDyingEffect.isValid)
		{
			Effect.server.Run(this.Properties.removeDyingEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		global::Item item = ItemManager.Create(this.Properties.removeDyingItem, 1, 0UL);
		if (receiver != null)
		{
			receiver.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		}
		else
		{
			item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up * 1f, default(Quaternion));
		}
		this.Die(null);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x000689A0 File Offset: 0x00066BA0
	[ServerVar(ServerAdmin = true)]
	public static void GrowAll(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (!basePlayer.IsAdmin)
		{
			return;
		}
		List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
		global::Vis.Entities<global::GrowableEntity>(basePlayer.ServerPosition, 6f, list, -1, QueryTriggerInteraction.Collide);
		foreach (global::GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				growableEntity.ChangeState(growableEntity.currentStage.nextState, false, false);
			}
		}
		Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00068A34 File Offset: 0x00066C34
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_RequestQualityUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null)
		{
			ProtoBuf.GrowableEntity growableEntity = Facepunch.Pool.Get<ProtoBuf.GrowableEntity>();
			growableEntity.lightModifier = this.LightQuality;
			growableEntity.groundModifier = this.GroundQuality;
			growableEntity.waterModifier = this.WaterQuality;
			growableEntity.happiness = this.OverallQuality;
			growableEntity.temperatureModifier = this.TemperatureQuality;
			growableEntity.waterConsumption = this.WaterConsumption;
			base.ClientRPCPlayer<ProtoBuf.GrowableEntity>(null, msg.player, "RPC_ReceiveQualityUpdate", growableEntity);
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000BCB RID: 3019 RVA: 0x00068AB0 File Offset: 0x00066CB0
	// (set) Token: 0x06000BCC RID: 3020 RVA: 0x00068AB8 File Offset: 0x00066CB8
	public PlantProperties.State State { get; private set; }

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000BCD RID: 3021 RVA: 0x00068AC1 File Offset: 0x00066CC1
	// (set) Token: 0x06000BCE RID: 3022 RVA: 0x00068AC9 File Offset: 0x00066CC9
	public float Age { get; private set; }

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000BCF RID: 3023 RVA: 0x00068AD2 File Offset: 0x00066CD2
	// (set) Token: 0x06000BD0 RID: 3024 RVA: 0x00068ADA File Offset: 0x00066CDA
	public float LightQuality { get; private set; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000BD1 RID: 3025 RVA: 0x00068AE3 File Offset: 0x00066CE3
	// (set) Token: 0x06000BD2 RID: 3026 RVA: 0x00068AEB File Offset: 0x00066CEB
	public float GroundQuality { get; private set; } = 1f;

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x00068AF4 File Offset: 0x00066CF4
	// (set) Token: 0x06000BD4 RID: 3028 RVA: 0x00068AFC File Offset: 0x00066CFC
	public float WaterQuality { get; private set; }

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x00068B05 File Offset: 0x00066D05
	// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x00068B0D File Offset: 0x00066D0D
	public float WaterConsumption { get; private set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x00068B16 File Offset: 0x00066D16
	// (set) Token: 0x06000BD8 RID: 3032 RVA: 0x00068B1E File Offset: 0x00066D1E
	public bool Fertilized { get; private set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x00068B27 File Offset: 0x00066D27
	// (set) Token: 0x06000BDA RID: 3034 RVA: 0x00068B2F File Offset: 0x00066D2F
	public float TemperatureQuality { get; private set; }

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000BDB RID: 3035 RVA: 0x00068B38 File Offset: 0x00066D38
	// (set) Token: 0x06000BDC RID: 3036 RVA: 0x00068B40 File Offset: 0x00066D40
	public float OverallQuality { get; private set; }

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x00068B49 File Offset: 0x00066D49
	// (set) Token: 0x06000BDE RID: 3038 RVA: 0x00068B51 File Offset: 0x00066D51
	public float Yield { get; private set; }

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000BDF RID: 3039 RVA: 0x00068B5C File Offset: 0x00066D5C
	public float StageProgressFraction
	{
		get
		{
			return this.stageAge / this.currentStage.lifeLengthSeconds;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x00068B7E File Offset: 0x00066D7E
	private PlantProperties.Stage currentStage
	{
		get
		{
			return this.Properties.stages[(int)this.State];
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x00068B96 File Offset: 0x00066D96
	public static float ThinkDeltaTime
	{
		get
		{
			return ConVar.Server.planttick;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x00068B9D File Offset: 0x00066D9D
	private float growDeltaTime
	{
		get
		{
			return ConVar.Server.planttick * ConVar.Server.planttickscale;
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00068BAA File Offset: 0x00066DAA
	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		GrowableGeneEncoding.DecodeIntToGenes(data.dataInt, this.Genes);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(data.dataInt, this.Genes);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x00068BCE File Offset: 0x00066DCE
	public override void ResetState()
	{
		base.ResetState();
		this.State = PlantProperties.State.Seed;
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00068BDD File Offset: 0x00066DDD
	public bool CanPick()
	{
		return this.currentStage.resources > 0f;
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00068BF1 File Offset: 0x00066DF1
	public int CurrentPickAmount
	{
		get
		{
			return Mathf.RoundToInt(this.CurrentPickAmountFloat);
		}
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00068BFE File Offset: 0x00066DFE
	public float CurrentPickAmountFloat
	{
		get
		{
			return (this.currentStage.resources + this.Yield) * (float)this.Properties.pickupMultiplier;
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00068C1F File Offset: 0x00066E1F
	public bool CanTakeSeeds()
	{
		return this.currentStage.resources > 0f && this.Properties.SeedItem != null;
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00068C46 File Offset: 0x00066E46
	public bool CanClone()
	{
		return this.currentStage.resources > 0f && this.Properties.CloneItem != null;
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00068C70 File Offset: 0x00066E70
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.growableEntity = Facepunch.Pool.Get<ProtoBuf.GrowableEntity>();
		info.msg.growableEntity.state = (int)this.State;
		info.msg.growableEntity.totalAge = this.Age;
		info.msg.growableEntity.stageAge = this.stageAge;
		info.msg.growableEntity.yieldFraction = this.Yield;
		info.msg.growableEntity.yieldPool = this.yieldPool;
		info.msg.growableEntity.fertilized = this.Fertilized;
		if (this.Genes != null)
		{
			this.Genes.Save(info);
		}
		if (!info.forDisk)
		{
			info.msg.growableEntity.lightModifier = this.LightQuality;
			info.msg.growableEntity.groundModifier = this.GroundQuality;
			info.msg.growableEntity.waterModifier = this.WaterQuality;
			info.msg.growableEntity.happiness = this.OverallQuality;
			info.msg.growableEntity.temperatureModifier = this.TemperatureQuality;
			info.msg.growableEntity.waterConsumption = this.WaterConsumption;
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00068DBC File Offset: 0x00066FBC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.growableEntity != null)
		{
			this.Age = info.msg.growableEntity.totalAge;
			this.stageAge = info.msg.growableEntity.stageAge;
			this.Yield = info.msg.growableEntity.yieldFraction;
			this.Fertilized = info.msg.growableEntity.fertilized;
			this.yieldPool = info.msg.growableEntity.yieldPool;
			this.Genes.Load(info);
			this.ChangeState((PlantProperties.State)info.msg.growableEntity.state, false, true);
			return;
		}
		this.Genes.GenerateRandom(this);
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00068E80 File Offset: 0x00067080
	private void ChangeState(PlantProperties.State state, bool resetAge, bool loading = false)
	{
		if (base.isServer && this.State == state)
		{
			return;
		}
		this.State = state;
		if (base.isServer)
		{
			if (!loading)
			{
				if (this.currentStage.resources > 0f)
				{
					this.yieldPool = this.currentStage.yield;
				}
				if (state == PlantProperties.State.Crossbreed)
				{
					if (this.Properties.CrossBreedEffect.isValid)
					{
						Effect.server.Run(this.Properties.CrossBreedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
					}
					GrowableGenetics.CrossBreed(this);
				}
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			if (resetAge)
			{
				this.stageAge = 0f;
			}
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00068F2C File Offset: 0x0006712C
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		PlanterBox planterBox;
		if (parent != null && (planterBox = parent as PlanterBox) != null)
		{
			planterBox.OnPlantInserted(this, deployedBy);
		}
	}

	// Token: 0x04000784 RID: 1924
	private const float artificalLightQuality = 1f;

	// Token: 0x04000785 RID: 1925
	private const float planterGroundModifierBase = 0.6f;

	// Token: 0x04000786 RID: 1926
	private const float fertilizerGroundModifierBonus = 0.4f;

	// Token: 0x04000787 RID: 1927
	private const float growthGeneSpeedMultiplier = 0.25f;

	// Token: 0x04000788 RID: 1928
	private const float waterGeneRequirementMultiplier = 0.1f;

	// Token: 0x04000789 RID: 1929
	private const float hardinessGeneModifierBonus = 0.2f;

	// Token: 0x0400078A RID: 1930
	private const float hardinessGeneTemperatureModifierBonus = 0.05f;

	// Token: 0x0400078B RID: 1931
	private const float baseYieldIncreaseMultiplier = 1f;

	// Token: 0x0400078C RID: 1932
	private const float yieldGeneBonusMultiplier = 0.25f;

	// Token: 0x0400078D RID: 1933
	private const float maxNonPlanterGroundQuality = 0.6f;

	// Token: 0x0400078E RID: 1934
	private const float deathRatePerQuality = 0.1f;

	// Token: 0x0400078F RID: 1935
	private TimeCachedValue<float> sunExposure;

	// Token: 0x04000790 RID: 1936
	private TimeCachedValue<float> artificialLightExposure;

	// Token: 0x04000791 RID: 1937
	private TimeCachedValue<float> artificialTemperatureExposure;

	// Token: 0x04000792 RID: 1938
	[ServerVar]
	[Help("How many miliseconds to budget for processing growable quality updates per frame")]
	public static float framebudgetms = 0.25f;

	// Token: 0x04000793 RID: 1939
	public static global::GrowableEntity.GrowableEntityUpdateQueue growableEntityUpdateQueue = new global::GrowableEntity.GrowableEntityUpdateQueue();

	// Token: 0x04000794 RID: 1940
	private bool underWater;

	// Token: 0x04000795 RID: 1941
	private int seasons;

	// Token: 0x04000796 RID: 1942
	private int harvests;

	// Token: 0x04000797 RID: 1943
	private float terrainTypeValue;

	// Token: 0x04000798 RID: 1944
	private float yieldPool;

	// Token: 0x04000799 RID: 1945
	private PlanterBox planter;

	// Token: 0x0400079A RID: 1946
	public PlantProperties Properties;

	// Token: 0x0400079B RID: 1947
	public ItemDefinition SourceItemDef;

	// Token: 0x040007A6 RID: 1958
	private float stageAge;

	// Token: 0x040007A7 RID: 1959
	public GrowableGenes Genes = new GrowableGenes();

	// Token: 0x040007A8 RID: 1960
	private const float startingHealth = 10f;

	// Token: 0x02000BDB RID: 3035
	public class GrowableEntityUpdateQueue : ObjectWorkQueue<global::GrowableEntity>
	{
		// Token: 0x06004DE2 RID: 19938 RVA: 0x001A1BB3 File Offset: 0x0019FDB3
		protected override void RunJob(global::GrowableEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.CalculateQualities_Water();
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x001A1BC5 File Offset: 0x0019FDC5
		protected override bool ShouldAdd(global::GrowableEntity entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
