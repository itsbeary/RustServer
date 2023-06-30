using System;
using UnityEngine;

// Token: 0x02000617 RID: 1559
public class MissionEntity : BaseMonoBehaviour, IOnParentDestroying
{
	// Token: 0x06002E37 RID: 11831 RVA: 0x00115FBC File Offset: 0x001141BC
	public void OnParentDestroying()
	{
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x00115FC4 File Offset: 0x001141C4
	public virtual void Setup(BasePlayer assignee, BaseMission.MissionInstance instance, bool wantsSuccessCleanup, bool wantsFailedCleanup)
	{
		this.cleanupOnMissionFailed = wantsFailedCleanup;
		this.cleanupOnMissionSuccess = wantsSuccessCleanup;
		BaseEntity entity = this.GetEntity();
		if (entity)
		{
			entity.SendMessage("MissionSetupPlayer", assignee, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x00115FFC File Offset: 0x001141FC
	public virtual void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x00116028 File Offset: 0x00114228
	public virtual void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionEnded(assignee, instance);
		}
		if (instance.createdEntities.Contains(this))
		{
			instance.createdEntities.Remove(this);
		}
		if ((this.cleanupOnMissionSuccess && (instance.status == BaseMission.MissionStatus.Completed || instance.status == BaseMission.MissionStatus.Accomplished)) || (this.cleanupOnMissionFailed && instance.status == BaseMission.MissionStatus.Failed))
		{
			BaseEntity entity = this.GetEntity();
			if (entity)
			{
				entity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x001160B8 File Offset: 0x001142B8
	public BaseEntity GetEntity()
	{
		return base.GetComponent<BaseEntity>();
	}

	// Token: 0x040025F0 RID: 9712
	public bool cleanupOnMissionSuccess = true;

	// Token: 0x040025F1 RID: 9713
	public bool cleanupOnMissionFailed = true;
}
