using System;
using UnityEngine;

// Token: 0x02000594 RID: 1428
public class TriggerNotifyEntity : TriggerBase, IPrefabPreProcess
{
	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06002BB9 RID: 11193 RVA: 0x0010953E File Offset: 0x0010773E
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x0010960E File Offset: 0x0010780E
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityEnter(ent);
		}
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x0010964C File Offset: 0x0010784C
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityLeave(ent);
		}
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x0010968A File Offset: 0x0010788A
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}

	// Token: 0x0400239B RID: 9115
	public GameObject notifyTarget;

	// Token: 0x0400239C RID: 9116
	private INotifyEntityTrigger toNotify;

	// Token: 0x0400239D RID: 9117
	public bool runClientside = true;

	// Token: 0x0400239E RID: 9118
	public bool runServerside = true;
}
