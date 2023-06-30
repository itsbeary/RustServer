using System;
using UnityEngine;

// Token: 0x02000592 RID: 1426
public class TriggerNotify : TriggerBase, IPrefabPreProcess
{
	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06002BB2 RID: 11186 RVA: 0x0010953E File Offset: 0x0010773E
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x00109558 File Offset: 0x00107758
	internal override void OnObjects()
	{
		base.OnObjects();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnObjects(this);
		}
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x00109595 File Offset: 0x00107795
	internal override void OnEmpty()
	{
		base.OnEmpty();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEmpty();
		}
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x001095D1 File Offset: 0x001077D1
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}

	// Token: 0x04002397 RID: 9111
	public GameObject notifyTarget;

	// Token: 0x04002398 RID: 9112
	private INotifyTrigger toNotify;

	// Token: 0x04002399 RID: 9113
	public bool runClientside = true;

	// Token: 0x0400239A RID: 9114
	public bool runServerside = true;
}
