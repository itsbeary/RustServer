using System;
using UnityEngine;

// Token: 0x0200061F RID: 1567
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/AcquireItem")]
public class MissionObjective_AcquireItem : MissionObjective
{
	// Token: 0x06002E62 RID: 11874 RVA: 0x001166C5 File Offset: 0x001148C5
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x001166D0 File Offset: 0x001148D0
	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
		if (base.IsCompleted(index, instance))
		{
			return;
		}
		if (!base.CanProgress(index, instance))
		{
			return;
		}
		if (type == BaseMission.MissionEventType.ACQUIRE_ITEM)
		{
			if (this.itemShortname == identifier)
			{
				instance.objectiveStatuses[index].genericInt1 += (int)amount;
			}
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x00116752 File Offset: 0x00114952
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002611 RID: 9745
	public string itemShortname;

	// Token: 0x04002612 RID: 9746
	public int targetItemAmount;
}
