using System;
using UnityEngine;

// Token: 0x02000469 RID: 1129
public class BaseTrapTrigger : TriggerBase
{
	// Token: 0x0600257D RID: 9597 RVA: 0x000ECDB0 File Offset: 0x000EAFB0
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

	// Token: 0x0600257E RID: 9598 RVA: 0x000ECDF3 File Offset: 0x000EAFF3
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		this._trap.ObjectEntered(obj);
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x000ECE09 File Offset: 0x000EB009
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this._trap.OnEmpty();
	}

	// Token: 0x04001DA7 RID: 7591
	public BaseTrap _trap;
}
