using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A2 RID: 1442
public class TriggerWakeAIZ : TriggerBase, IServerComponent
{
	// Token: 0x06002BFE RID: 11262 RVA: 0x0010A638 File Offset: 0x00108838
	public void Init(AIInformationZone zone = null)
	{
		if (zone != null)
		{
			this.aiz = zone;
		}
		else if (this.zones == null || this.zones.Count == 0)
		{
			Transform transform = base.transform.parent;
			if (transform == null)
			{
				transform = base.transform;
			}
			this.aiz = transform.GetComponentInChildren<AIInformationZone>();
		}
		this.SetZonesSleeping(true);
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x0010A69B File Offset: 0x0010889B
	private void Awake()
	{
		this.Init(null);
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x0010A6A4 File Offset: 0x001088A4
	private void SetZonesSleeping(bool flag)
	{
		if (this.aiz != null)
		{
			if (flag)
			{
				this.aiz.SleepAI();
			}
			else
			{
				this.aiz.WakeAI();
			}
		}
		if (this.zones != null && this.zones.Count > 0)
		{
			foreach (AIInformationZone aiinformationZone in this.zones)
			{
				if (aiinformationZone != null)
				{
					if (flag)
					{
						aiinformationZone.SleepAI();
					}
					else
					{
						aiinformationZone.WakeAI();
					}
				}
			}
		}
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x0010A74C File Offset: 0x0010894C
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
		if (baseEntity.isClient)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (basePlayer != null && basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002C02 RID: 11266 RVA: 0x0010A7AC File Offset: 0x001089AC
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		base.CancelInvoke(new Action(this.SleepAI));
		this.SetZonesSleeping(false);
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x0010A800 File Offset: 0x00108A00
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		if (this.entityContents == null || this.entityContents.Count == 0)
		{
			this.DelayedSleepAI();
		}
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x0010A853 File Offset: 0x00108A53
	private void DelayedSleepAI()
	{
		base.CancelInvoke(new Action(this.SleepAI));
		base.Invoke(new Action(this.SleepAI), this.SleepDelaySeconds);
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x0010A87F File Offset: 0x00108A7F
	private void SleepAI()
	{
		this.SetZonesSleeping(true);
	}

	// Token: 0x040023C3 RID: 9155
	public float SleepDelaySeconds = 30f;

	// Token: 0x040023C4 RID: 9156
	public List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x040023C5 RID: 9157
	private AIInformationZone aiz;
}
