using System;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
public class TriggerWorkbench : TriggerBase
{
	// Token: 0x06002C0A RID: 11274 RVA: 0x0010A974 File Offset: 0x00108B74
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

	// Token: 0x06002C0B RID: 11275 RVA: 0x0010A9B7 File Offset: 0x00108BB7
	public float WorkbenchLevel()
	{
		return (float)this.parentBench.Workbenchlevel;
	}

	// Token: 0x040023CB RID: 9163
	public Workbench parentBench;
}
