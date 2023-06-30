using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000588 RID: 1416
public class TriggerComfort : TriggerBase
{
	// Token: 0x06002B85 RID: 11141 RVA: 0x00108790 File Offset: 0x00106990
	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x001087B4 File Offset: 0x001069B4
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
		return baseEntity.gameObject;
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x001087F8 File Offset: 0x001069F8
	public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
	{
		float num = Vector3.Distance(base.gameObject.transform.position, position);
		float num2 = 1f - Mathf.Clamp(num - this.minComfortRange, 0f, num / (this.triggerSize - this.minComfortRange));
		float num3 = 0f;
		foreach (BasePlayer basePlayer in this._players)
		{
			if (!(basePlayer == forPlayer))
			{
				num3 += 0.25f * (basePlayer.IsSleeping() ? 0.5f : 1f) * (basePlayer.IsAlive() ? 1f : 0f);
			}
		}
		float num4 = 0f + num3;
		return (this.baseComfort + num4) * num2;
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x001088DC File Offset: 0x00106ADC
	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Add(basePlayer);
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x00108908 File Offset: 0x00106B08
	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Remove(basePlayer);
	}

	// Token: 0x0400236F RID: 9071
	public float triggerSize;

	// Token: 0x04002370 RID: 9072
	public float baseComfort = 0.5f;

	// Token: 0x04002371 RID: 9073
	public float minComfortRange = 2.5f;

	// Token: 0x04002372 RID: 9074
	private const float perPlayerComfortBonus = 0.25f;

	// Token: 0x04002373 RID: 9075
	private const float bonusComfort = 0f;

	// Token: 0x04002374 RID: 9076
	private List<BasePlayer> _players = new List<BasePlayer>();
}
