using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000590 RID: 1424
public class TriggerMount : TriggerBase, IServerComponent
{
	// Token: 0x06002BA9 RID: 11177 RVA: 0x00109284 File Offset: 0x00107484
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity.ToPlayer();
		if (basePlayer == null || basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x001092C4 File Offset: 0x001074C4
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.entryInfo == null)
		{
			this.entryInfo = new Dictionary<BaseEntity, TriggerMount.EntryInfo>();
		}
		this.entryInfo.Add(ent, new TriggerMount.EntryInfo(Time.time, ent.transform.position));
		base.Invoke(new Action(this.CheckForMount), 3.6f);
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x00109323 File Offset: 0x00107523
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryInfo != null)
		{
			this.entryInfo.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x0010934C File Offset: 0x0010754C
	private void CheckForMount()
	{
		if (this.entityContents == null || this.entryInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseEntity, TriggerMount.EntryInfo> keyValuePair in this.entryInfo)
		{
			BaseEntity key = keyValuePair.Key;
			if (key.IsValid())
			{
				TriggerMount.EntryInfo value = keyValuePair.Value;
				BasePlayer basePlayer = key.ToPlayer();
				bool flag = (basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying;
				if (basePlayer != null && basePlayer.IsAlive() && !flag)
				{
					bool flag2 = false;
					if (!basePlayer.isMounted && !basePlayer.IsSleeping() && value.entryTime + 3.5f < Time.time && Vector3.Distance(key.transform.position, value.entryPos) < 0.5f)
					{
						BaseVehicle componentInParent = base.GetComponentInParent<BaseVehicle>();
						if (componentInParent != null && !componentInParent.IsDead())
						{
							componentInParent.AttemptMount(basePlayer, true);
							flag2 = true;
						}
					}
					if (!flag2)
					{
						value.Set(Time.time, key.transform.position);
						base.Invoke(new Action(this.CheckForMount), 3.6f);
					}
				}
			}
		}
	}

	// Token: 0x04002391 RID: 9105
	private const float MOUNT_DELAY = 3.5f;

	// Token: 0x04002392 RID: 9106
	private const float MAX_MOVE = 0.5f;

	// Token: 0x04002393 RID: 9107
	private Dictionary<BaseEntity, TriggerMount.EntryInfo> entryInfo;

	// Token: 0x02000D79 RID: 3449
	private class EntryInfo
	{
		// Token: 0x06005129 RID: 20777 RVA: 0x001AB781 File Offset: 0x001A9981
		public EntryInfo(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		// Token: 0x0600512A RID: 20778 RVA: 0x001AB797 File Offset: 0x001A9997
		public void Set(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		// Token: 0x0400480B RID: 18443
		public float entryTime;

		// Token: 0x0400480C RID: 18444
		public Vector3 entryPos;
	}
}
