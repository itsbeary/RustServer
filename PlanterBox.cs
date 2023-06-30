using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AE RID: 174
public class PlanterBox : StorageContainer, ISplashable
{
	// Token: 0x06000FC9 RID: 4041 RVA: 0x00083FA4 File Offset: 0x000821A4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlanterBox.OnRpcMessage", 0))
		{
			if (rpc == 2965786167U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestSaturationUpdate ");
				}
				using (TimeWarning.New("RPC_RequestSaturationUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2965786167U, "RPC_RequestSaturationUpdate", this, player, 3f))
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
							this.RPC_RequestSaturationUpdate(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_RequestSaturationUpdate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0008410C File Offset: 0x0008230C
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		base.inventory.SetOnlyAllowedItem(this.allowedItem);
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		this.sunExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 30f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateSunExposure)
		};
		this.artificialLightExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialLightExposure)
		};
		this.plantTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 20f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculatePlantTemperature)
		};
		this.plantArtificalTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialTemperature)
		};
		this.lastRainCheck = 0f;
		base.InvokeRandomized(new Action(this.CalculateRainFactor), 20f, 30f, 15f);
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0008426C File Offset: 0x0008246C
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (added && this.ItemIsFertilizer(item))
		{
			this.FertilizeGrowables();
		}
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00084288 File Offset: 0x00082488
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && this.ItemIsFertilizer(item);
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0008429B File Offset: 0x0008249B
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000842B2 File Offset: 0x000824B2
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.resource = Facepunch.Pool.Get<BaseResource>();
		info.msg.resource.stage = this.soilSaturation;
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x000842E1 File Offset: 0x000824E1
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			this.soilSaturation = info.msg.resource.stage;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x0008430D File Offset: 0x0008250D
	public float soilSaturationFraction
	{
		get
		{
			return (float)this.soilSaturation / (float)this.soilSaturationMax;
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x0008431E File Offset: 0x0008251E
	public int availableIdealWaterCapacity
	{
		get
		{
			return Mathf.Max(this.availableIdealWaterCapacity, Mathf.Max(this.idealSaturation - this.soilSaturation, 0));
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0008433E File Offset: 0x0008253E
	public int availableWaterCapacity
	{
		get
		{
			return this.soilSaturationMax - this.soilSaturation;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x0008434D File Offset: 0x0008254D
	public int idealSaturation
	{
		get
		{
			return Mathf.FloorToInt((float)this.soilSaturationMax * ConVar.Server.optimalPlanterQualitySaturation);
		}
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x00084361 File Offset: 0x00082561
	public bool BelowMinimumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction < PlanterBox.MinimumSaturationTriggerLevel;
		}
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00084370 File Offset: 0x00082570
	public bool AboveMaximumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction > PlanterBox.MaximumSaturationTriggerLevel;
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x00084380 File Offset: 0x00082580
	public void FertilizeGrowables()
	{
		int num = this.GetFertilizerCount();
		if (num <= 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			if (!(baseEntity == null))
			{
				global::GrowableEntity growableEntity = baseEntity as global::GrowableEntity;
				if (!(growableEntity == null) && !growableEntity.Fertilized && this.ConsumeFertilizer())
				{
					growableEntity.Fertilize();
					num--;
					if (num == 0)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00084410 File Offset: 0x00082610
	public int GetFertilizerCount()
	{
		int num = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00084458 File Offset: 0x00082658
	public bool ConsumeFertilizer()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				int num = Mathf.Min(1, slot.amount);
				if (num > 0)
				{
					slot.UseItem(num);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000844B0 File Offset: 0x000826B0
	public int ConsumeWater(int amount, global::GrowableEntity ignoreEntity = null)
	{
		int num = Mathf.Min(amount, this.soilSaturation);
		this.soilSaturation -= num;
		this.RefreshGrowables(ignoreEntity);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return num;
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x000844E8 File Offset: 0x000826E8
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && !(splashType == null) && splashType.shortname != null && (splashType.shortname == "water.salt" || this.soilSaturation < this.soilSaturationMax);
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00084534 File Offset: 0x00082734
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (splashType.shortname == "water.salt")
		{
			this.soilSaturation = 0;
			this.RefreshGrowables(null);
			if (this.lastSplashNetworkUpdate > 60f)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.lastSplashNetworkUpdate = 0f;
			}
			return amount;
		}
		int num = Mathf.Min(this.availableWaterCapacity, amount);
		this.soilSaturation += num;
		this.RefreshGrowables(null);
		if (this.lastSplashNetworkUpdate > 60f)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.lastSplashNetworkUpdate = 0f;
		}
		return num;
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x000845D8 File Offset: 0x000827D8
	private void RefreshGrowables(global::GrowableEntity ignoreEntity = null)
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			global::GrowableEntity growableEntity;
			if (!(baseEntity == null) && !(baseEntity == ignoreEntity) && (growableEntity = baseEntity as global::GrowableEntity) != null)
			{
				growableEntity.QueueForQualityUpdate();
			}
		}
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x00084650 File Offset: 0x00082850
	public void ForceLightUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue != null)
		{
			timeCachedValue.ForceNextRun();
		}
		TimeCachedValue<float> timeCachedValue2 = this.artificialLightExposure;
		if (timeCachedValue2 == null)
		{
			return;
		}
		timeCachedValue2.ForceNextRun();
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00084673 File Offset: 0x00082873
	public void ForceTemperatureUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.plantArtificalTemperature;
		if (timeCachedValue == null)
		{
			return;
		}
		timeCachedValue.ForceNextRun();
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00084685 File Offset: 0x00082885
	public float GetSunExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00067813 File Offset: 0x00065A13
	private float CalculateSunExposure()
	{
		return global::GrowableEntity.SunRaycast(base.transform.position + new Vector3(0f, 1f, 0f));
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0008469D File Offset: 0x0008289D
	public float GetArtificialLightExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.artificialLightExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00067870 File Offset: 0x00065A70
	private float CalculateArtificialLightExposure()
	{
		return global::GrowableEntity.CalculateArtificialLightExposure(base.transform);
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x000846B5 File Offset: 0x000828B5
	public float GetPlantTemperature()
	{
		TimeCachedValue<float> timeCachedValue = this.plantTemperature;
		float num = ((timeCachedValue != null) ? timeCachedValue.Get(false) : 0f);
		TimeCachedValue<float> timeCachedValue2 = this.plantArtificalTemperature;
		return num + ((timeCachedValue2 != null) ? timeCachedValue2.Get(false) : 0f);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x000846E6 File Offset: 0x000828E6
	private float CalculatePlantTemperature()
	{
		return Mathf.Max(Climate.GetTemperature(base.transform.position), 15f);
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x00084704 File Offset: 0x00082904
	private void CalculateRainFactor()
	{
		if (this.sunExposure.Get(false) > 0f)
		{
			float rain = Climate.GetRain(base.transform.position);
			if (rain > 0f)
			{
				this.soilSaturation = Mathf.Clamp(this.soilSaturation + Mathf.RoundToInt(4f * rain * this.lastRainCheck), 0, this.soilSaturationMax);
				this.RefreshGrowables(null);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		this.lastRainCheck = 0f;
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00067D53 File Offset: 0x00065F53
	private float CalculateArtificialTemperature()
	{
		return global::GrowableEntity.CalculateArtificialTemperature(base.transform);
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x0008478C File Offset: 0x0008298C
	public void OnPlantInserted(global::GrowableEntity entity, global::BasePlayer byPlayer)
	{
		if (!GameInfo.HasAchievements)
		{
			return;
		}
		List<uint> list = Facepunch.Pool.GetList<uint>();
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::GrowableEntity growableEntity;
				if ((growableEntity = enumerator.Current as global::GrowableEntity) != null && !list.Contains(growableEntity.prefabID))
				{
					list.Add(growableEntity.prefabID);
				}
			}
		}
		if (list.Count == 9)
		{
			byPlayer.GiveAchievement("HONEST_WORK");
		}
		Facepunch.Pool.FreeList<uint>(ref list);
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00084824 File Offset: 0x00082A24
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_RequestSaturationUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null)
		{
			base.ClientRPCPlayer<int>(null, msg.player, "RPC_ReceiveSaturationUpdate", this.soilSaturation);
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000A41 RID: 2625
	public int soilSaturation;

	// Token: 0x04000A42 RID: 2626
	public int soilSaturationMax = 8000;

	// Token: 0x04000A43 RID: 2627
	public MeshRenderer soilRenderer;

	// Token: 0x04000A44 RID: 2628
	private static readonly float MinimumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation - 0.2f;

	// Token: 0x04000A45 RID: 2629
	private static readonly float MaximumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation + 0.1f;

	// Token: 0x04000A46 RID: 2630
	private TimeCachedValue<float> sunExposure;

	// Token: 0x04000A47 RID: 2631
	private TimeCachedValue<float> artificialLightExposure;

	// Token: 0x04000A48 RID: 2632
	private TimeCachedValue<float> plantTemperature;

	// Token: 0x04000A49 RID: 2633
	private TimeCachedValue<float> plantArtificalTemperature;

	// Token: 0x04000A4A RID: 2634
	private TimeSince lastSplashNetworkUpdate;

	// Token: 0x04000A4B RID: 2635
	private TimeSince lastRainCheck;
}
