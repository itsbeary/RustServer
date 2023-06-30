using System;
using System.Collections.Generic;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000585 RID: 1413
public class TriggerAnalytic : TriggerBase, IServerComponent
{
	// Token: 0x06002B6B RID: 11115 RVA: 0x0010801C File Offset: 0x0010621C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (!Analytics.Server.Enabled)
		{
			return null;
		}
		BasePlayer basePlayer;
		if ((basePlayer = obj.ToBaseEntity() as BasePlayer) != null && !basePlayer.IsNpc && basePlayer.isServer)
		{
			return basePlayer.gameObject;
		}
		return null;
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x0010805C File Offset: 0x0010625C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (!Analytics.Server.Enabled)
		{
			return;
		}
		base.OnEntityEnter(ent);
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null && !basePlayer.IsNpc)
		{
			this.CheckTimeouts();
			if (this.IsPlayerValid(basePlayer))
			{
				Analytics.Server.Trigger(this.AnalyticMessage);
				this.recentEntrances.Add(new TriggerAnalytic.RecentPlayerEntrance
				{
					Player = basePlayer,
					Time = 0f
				});
			}
		}
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x001080D8 File Offset: 0x001062D8
	private void CheckTimeouts()
	{
		for (int i = this.recentEntrances.Count - 1; i >= 0; i--)
		{
			if (this.recentEntrances[i].Time > this.Timeout)
			{
				this.recentEntrances.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x00108128 File Offset: 0x00106328
	private bool IsPlayerValid(BasePlayer p)
	{
		for (int i = 0; i < this.recentEntrances.Count; i++)
		{
			if (this.recentEntrances[i].Player == p)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002369 RID: 9065
	public string AnalyticMessage;

	// Token: 0x0400236A RID: 9066
	public float Timeout = 120f;

	// Token: 0x0400236B RID: 9067
	private List<TriggerAnalytic.RecentPlayerEntrance> recentEntrances = new List<TriggerAnalytic.RecentPlayerEntrance>();

	// Token: 0x02000D76 RID: 3446
	private struct RecentPlayerEntrance
	{
		// Token: 0x04004806 RID: 18438
		public BasePlayer Player;

		// Token: 0x04004807 RID: 18439
		public TimeSince Time;
	}
}
