using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000589 RID: 1417
public class TriggerDanceAchievement : TriggerBase
{
	// Token: 0x06002B8B RID: 11147 RVA: 0x0010895B File Offset: 0x00106B5B
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x00108963 File Offset: 0x00106B63
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x00108970 File Offset: 0x00106B70
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
		if (!(baseEntity is BasePlayer))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x001089C0 File Offset: 0x00106BC0
	public void NotifyDanceStarted()
	{
		if (this.entityContents == null)
		{
			return;
		}
		int num = 0;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity.ToPlayer() != null && baseEntity.ToPlayer().CurrentGestureIsDance)
			{
				num++;
				if (num >= this.RequiredPlayerCount)
				{
					break;
				}
			}
		}
		if (num >= this.RequiredPlayerCount)
		{
			foreach (BaseEntity baseEntity2 in this.entityContents)
			{
				if (!this.triggeredPlayers.Contains(baseEntity2.net.ID) && baseEntity2.ToPlayer() != null)
				{
					baseEntity2.ToPlayer().GiveAchievement(this.AchievementName);
					this.triggeredPlayers.Add(baseEntity2.net.ID);
				}
			}
		}
	}

	// Token: 0x04002375 RID: 9077
	public int RequiredPlayerCount = 3;

	// Token: 0x04002376 RID: 9078
	public string AchievementName;

	// Token: 0x04002377 RID: 9079
	[NonSerialized]
	private List<NetworkableId> triggeredPlayers = new List<NetworkableId>();
}
