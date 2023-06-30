using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200059B RID: 1435
public class TriggerPlayerTimer : TriggerBase
{
	// Token: 0x06002BDD RID: 11229 RVA: 0x00109ED0 File Offset: 0x001080D0
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			BasePlayer basePlayer;
			if ((basePlayer = baseEntity as BasePlayer) != null && baseEntity.isServer && !basePlayer.isMounted)
			{
				return baseEntity.gameObject;
			}
		}
		return obj;
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x00109F18 File Offset: 0x00108118
	internal override void OnObjects()
	{
		base.OnObjects();
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x00109F38 File Offset: 0x00108138
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.DamageTarget));
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x00109F54 File Offset: 0x00108154
	private void DamageTarget()
	{
		bool flag = false;
		using (HashSet<BaseEntity>.Enumerator enumerator = this.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BasePlayer basePlayer;
				if ((basePlayer = enumerator.Current as BasePlayer) != null && !basePlayer.isMounted)
				{
					flag = true;
				}
			}
		}
		if (flag && this.TargetEntity != null)
		{
			this.TargetEntity.OnAttacked(new HitInfo(null, this.TargetEntity, DamageType.Generic, this.DamageAmount));
		}
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}

	// Token: 0x040023B0 RID: 9136
	public BaseEntity TargetEntity;

	// Token: 0x040023B1 RID: 9137
	public float DamageAmount = 20f;

	// Token: 0x040023B2 RID: 9138
	public float TimeToDamage = 3f;
}
