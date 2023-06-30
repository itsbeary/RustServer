using System;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public class MissionObjective : ScriptableObject
{
	// Token: 0x06002E57 RID: 11863 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void MissionStarted(int index, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x001165C1 File Offset: 0x001147C1
	public virtual void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		instance.objectiveStatuses[index].started = true;
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x001165D8 File Offset: 0x001147D8
	public bool IsStarted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].started;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x001165E7 File Offset: 0x001147E7
	public bool CanProgress(int index, BaseMission.MissionInstance instance)
	{
		return !instance.GetMission().objectives[index].onlyProgressIfStarted || this.IsStarted(index, instance);
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x00116608 File Offset: 0x00114808
	public bool ShouldObjectiveStart(int index, BaseMission.MissionInstance instance)
	{
		foreach (int num in instance.GetMission().objectives[index].startAfterCompletedObjectives)
		{
			if (!instance.objectiveStatuses[num].completed && !instance.objectiveStatuses[num].failed)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x0011665B File Offset: 0x0011485B
	public bool IsCompleted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].completed || instance.objectiveStatuses[index].failed;
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x0011667B File Offset: 0x0011487B
	public virtual bool ShouldThink(int index, BaseMission.MissionInstance instance)
	{
		return !this.IsCompleted(index, instance);
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x00116688 File Offset: 0x00114888
	public virtual void CompleteObjective(int index, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		instance.objectiveStatuses[index].completed = true;
		instance.GetMission().OnObjectiveCompleted(index, instance, playerFor);
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x001166A6 File Offset: 0x001148A6
	public virtual void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		if (this.ShouldObjectiveStart(index, instance) && !this.IsStarted(index, instance))
		{
			this.ObjectiveStarted(assignee, index, instance);
		}
	}
}
