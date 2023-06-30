using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000584 RID: 1412
public class TriggerAchievement : TriggerBase
{
	// Token: 0x06002B66 RID: 11110 RVA: 0x00107E7F File Offset: 0x0010607F
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x00107E87 File Offset: 0x00106087
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x00107E94 File Offset: 0x00106094
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient && this.serverSide)
		{
			return null;
		}
		if (baseEntity.isServer && !this.serverSide)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x00107EF4 File Offset: 0x001060F4
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		BasePlayer component = ent.GetComponent<BasePlayer>();
		if (component == null || !component.IsAlive() || component.IsSleeping() || component.IsNpc)
		{
			return;
		}
		if (this.triggeredPlayers.Contains(component.userID))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.requiredVehicleName))
		{
			BaseVehicle mountedVehicle = component.GetMountedVehicle();
			if (mountedVehicle == null)
			{
				return;
			}
			if (!mountedVehicle.ShortPrefabName.Contains(this.requiredVehicleName))
			{
				return;
			}
		}
		if (this.serverSide)
		{
			if (!string.IsNullOrEmpty(this.achievementOnEnter))
			{
				component.GiveAchievement(this.achievementOnEnter);
			}
			if (!string.IsNullOrEmpty(this.statToIncrease))
			{
				component.stats.Add(this.statToIncrease, 1, Stats.Steam);
				component.stats.Save(true);
			}
			this.triggeredPlayers.Add(component.userID);
		}
	}

	// Token: 0x04002364 RID: 9060
	public string statToIncrease = "";

	// Token: 0x04002365 RID: 9061
	public string achievementOnEnter = "";

	// Token: 0x04002366 RID: 9062
	public string requiredVehicleName = "";

	// Token: 0x04002367 RID: 9063
	[Tooltip("Always set to true, clientside does not work, currently")]
	public bool serverSide = true;

	// Token: 0x04002368 RID: 9064
	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();
}
