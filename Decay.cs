using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
public abstract class Decay : PrefabAttribute, IServerComponent
{
	// Token: 0x0600226D RID: 8813 RVA: 0x000DE87C File Offset: 0x000DCA7C
	protected float GetDecayDelay(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.delay_override > 0f)
			{
				return ConVar.Decay.delay_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.delay_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.delay_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.delay_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.delay_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.delay_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 64800f;
			case BuildingGrade.Enum.Stone:
				return 64800f;
			case BuildingGrade.Enum.Metal:
				return 64800f;
			case BuildingGrade.Enum.TopTier:
				return 86400f;
			}
		}
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000DE930 File Offset: 0x000DCB30
	protected float GetDecayDuration(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.duration_override > 0f)
			{
				return ConVar.Decay.duration_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.duration_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.duration_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.duration_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.duration_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.duration_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 86400f;
			case BuildingGrade.Enum.Stone:
				return 172800f;
			case BuildingGrade.Enum.Metal:
				return 259200f;
			case BuildingGrade.Enum.TopTier:
				return 432000f;
			}
		}
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000DE9E4 File Offset: 0x000DCBE4
	public static void BuildingDecayTouch(BuildingBlock buildingBlock)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(buildingBlock.transform.position, 40f, list, 2097408, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			DecayEntity decayEntity = list[i];
			BuildingBlock buildingBlock2 = decayEntity as BuildingBlock;
			if (!buildingBlock2 || buildingBlock2.buildingID == buildingBlock.buildingID)
			{
				decayEntity.DecayTouch();
			}
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000DEA5E File Offset: 0x000DCC5E
	public static void EntityLinkDecayTouch(BaseEntity ent)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		ent.EntityLinkBroadcast<DecayEntity>(delegate(DecayEntity decayEnt)
		{
			decayEnt.DecayTouch();
		});
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000DEA90 File Offset: 0x000DCC90
	public static void RadialDecayTouch(Vector3 pos, float radius, int mask)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(pos, radius, list, mask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].DecayTouch();
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06002273 RID: 8819
	public abstract float GetDecayDelay(BaseEntity entity);

	// Token: 0x06002274 RID: 8820
	public abstract float GetDecayDuration(BaseEntity entity);

	// Token: 0x06002275 RID: 8821 RVA: 0x000DEAD9 File Offset: 0x000DCCD9
	protected override Type GetIndexedType()
	{
		return typeof(global::Decay);
	}

	// Token: 0x04001A87 RID: 6791
	private const float hours = 3600f;
}
