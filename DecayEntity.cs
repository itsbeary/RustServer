using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class DecayEntity : BaseCombatEntity
{
	// Token: 0x06002277 RID: 8823 RVA: 0x000DEAE8 File Offset: 0x000DCCE8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.decayEntity = Facepunch.Pool.Get<ProtoBuf.DecayEntity>();
		info.msg.decayEntity.buildingID = this.buildingID;
		if (info.forDisk)
		{
			info.msg.decayEntity.decayTimer = this.decayTimer;
			info.msg.decayEntity.upkeepTimer = this.upkeepTimer;
		}
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000DEB58 File Offset: 0x000DCD58
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity != null)
		{
			this.decayTimer = info.msg.decayEntity.decayTimer;
			this.upkeepTimer = info.msg.decayEntity.upkeepTimer;
			if (this.buildingID != info.msg.decayEntity.buildingID)
			{
				this.AttachToBuilding(info.msg.decayEntity.buildingID);
				if (info.fromDisk)
				{
					BuildingManager.server.LoadBuildingID(this.buildingID);
				}
			}
		}
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000DEBEB File Offset: 0x000DCDEB
	public override void ResetState()
	{
		base.ResetState();
		this.buildingID = 0U;
		if (base.isServer)
		{
			this.decayTimer = 0f;
		}
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000DEC0D File Offset: 0x000DCE0D
	public void AttachToBuilding(uint id)
	{
		if (base.isServer)
		{
			BuildingManager.server.Remove(this);
			this.buildingID = id;
			BuildingManager.server.Add(this);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000DEC3B File Offset: 0x000DCE3B
	public BuildingManager.Building GetBuilding()
	{
		if (base.isServer)
		{
			return BuildingManager.server.GetBuilding(this.buildingID);
		}
		return null;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000DEC58 File Offset: 0x000DCE58
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = this.GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000DEC7C File Offset: 0x000DCE7C
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (this.upkeep == null)
		{
			return;
		}
		float num = this.upkeep.upkeepMultiplier * multiplier;
		if (num == 0f)
		{
			return;
		}
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category == ItemCategory.Resources)
			{
				float num2 = itemAmount.amount * num;
				bool flag = false;
				foreach (ItemAmount itemAmount2 in itemAmounts)
				{
					if (itemAmount2.itemDef == itemAmount.itemDef)
					{
						itemAmount2.amount += num2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					itemAmounts.Add(new ItemAmount(itemAmount.itemDef, num2));
				}
			}
		}
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000DED90 File Offset: 0x000DCF90
	public override void ServerInit()
	{
		base.ServerInit();
		this.decayVariance = UnityEngine.Random.Range(0.95f, 1f);
		this.decay = PrefabAttribute.server.Find<global::Decay>(this.prefabID);
		this.decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(this.prefabID);
		this.upkeep = PrefabAttribute.server.Find<Upkeep>(this.prefabID);
		BuildingManager.server.Add(this);
		if (!Rust.Application.isLoadingSave)
		{
			BuildingManager.server.CheckMerge(this);
		}
		this.lastDecayTick = UnityEngine.Time.time;
	}

	// Token: 0x0600227F RID: 8831 RVA: 0x000DEE22 File Offset: 0x000DD022
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BuildingManager.server.Remove(this);
		BuildingManager.server.CheckSplit(this);
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000DEE40 File Offset: 0x000DD040
	public virtual void AttachToBuilding(global::DecayEntity other)
	{
		if (other != null)
		{
			this.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		global::BuildingBlock nearbyBuildingBlock = this.GetNearbyBuildingBlock();
		if (nearbyBuildingBlock)
		{
			this.AttachToBuilding(nearbyBuildingBlock.buildingID);
		}
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000DEE8C File Offset: 0x000DD08C
	public global::BuildingBlock GetNearbyBuildingBlock()
	{
		float num = float.MaxValue;
		global::BuildingBlock buildingBlock = null;
		Vector3 vector = base.PivotPoint();
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(vector, 1.5f, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock2 = list[i];
			if (buildingBlock2.isServer == base.isServer)
			{
				float num2 = buildingBlock2.SqrDistance(vector);
				if (!buildingBlock2.grounded)
				{
					num2 += 1f;
				}
				if (num2 < num)
				{
					num = num2;
					buildingBlock = buildingBlock2;
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return buildingBlock;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000DEF1E File Offset: 0x000DD11E
	public void ResetUpkeepTime()
	{
		this.upkeepTimer = 0f;
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000DEF2B File Offset: 0x000DD12B
	public void DecayTouch()
	{
		this.decayTimer = 0f;
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000DEF38 File Offset: 0x000DD138
	public void AddUpkeepTime(float time)
	{
		this.upkeepTimer -= time;
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000DEF48 File Offset: 0x000DD148
	public float GetProtectedSeconds()
	{
		return Mathf.Max(0f, -this.upkeepTimer);
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000DEF5C File Offset: 0x000DD15C
	public virtual void DecayTick()
	{
		if (this.decay == null)
		{
			return;
		}
		float num = UnityEngine.Time.time - this.lastDecayTick;
		if (num < ConVar.Decay.tick)
		{
			return;
		}
		this.lastDecayTick = UnityEngine.Time.time;
		if (base.HasParent())
		{
			return;
		}
		if (!this.decay.ShouldDecay(this))
		{
			return;
		}
		float num2 = num * ConVar.Decay.scale;
		if (ConVar.Decay.upkeep)
		{
			this.upkeepTimer += num2;
			if (this.upkeepTimer > 0f)
			{
				BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
				if (buildingPrivilege != null)
				{
					this.upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(this.upkeepTimer, 600f));
				}
			}
			if (this.upkeepTimer < 1f)
			{
				if (base.healthFraction < 1f && ConVar.Decay.upkeep_heal_scale > 0f && base.SecondsSinceAttacked > 600f)
				{
					float num3 = num / this.decay.GetDecayDuration(this) * ConVar.Decay.upkeep_heal_scale;
					this.Heal(this.MaxHealth() * num3);
				}
				return;
			}
			this.upkeepTimer = 1f;
		}
		this.decayTimer += num2;
		if (this.decayTimer < this.decay.GetDecayDelay(this))
		{
			return;
		}
		using (TimeWarning.New("DecayTick", 0))
		{
			float num4 = 1f;
			if (ConVar.Decay.upkeep)
			{
				if (!this.BypassInsideDecayMultiplier && !this.IsOutside())
				{
					num4 *= ConVar.Decay.upkeep_inside_decay_scale;
				}
			}
			else
			{
				for (int i = 0; i < this.decayPoints.Length; i++)
				{
					DecayPoint decayPoint = this.decayPoints[i];
					if (decayPoint.IsOccupied(this))
					{
						num4 -= decayPoint.protection;
					}
				}
			}
			if (num4 > 0f)
			{
				float num5 = num2 / this.decay.GetDecayDuration(this) * this.MaxHealth();
				base.Hurt(num5 * num4 * this.decayVariance, DamageType.Decay, null, true);
			}
		}
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000DF15C File Offset: 0x000DD35C
	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		this.DecayTouch();
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000DF16C File Offset: 0x000DD36C
	public override void OnKilled(HitInfo info)
	{
		if (this.debrisPrefab.isValid)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisPrefab.resourcePath, base.transform.position, base.transform.rotation * Quaternion.Euler(this.debrisRotationOffset), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06002289 RID: 8841 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool BypassInsideDecayMultiplier
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04001A88 RID: 6792
	public GameObjectRef debrisPrefab;

	// Token: 0x04001A89 RID: 6793
	public Vector3 debrisRotationOffset = Vector3.zero;

	// Token: 0x04001A8A RID: 6794
	[NonSerialized]
	public uint buildingID;

	// Token: 0x04001A8B RID: 6795
	private float decayTimer;

	// Token: 0x04001A8C RID: 6796
	private float upkeepTimer;

	// Token: 0x04001A8D RID: 6797
	private Upkeep upkeep;

	// Token: 0x04001A8E RID: 6798
	private global::Decay decay;

	// Token: 0x04001A8F RID: 6799
	private DecayPoint[] decayPoints;

	// Token: 0x04001A90 RID: 6800
	private float lastDecayTick;

	// Token: 0x04001A91 RID: 6801
	private float decayVariance = 1f;
}
