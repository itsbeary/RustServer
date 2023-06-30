using System;
using UnityEngine;

// Token: 0x02000621 RID: 1569
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Harvest")]
public class MissionObjective_Harvest : MissionObjective
{
	// Token: 0x06002E6A RID: 11882 RVA: 0x001166C5 File Offset: 0x001148C5
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x001167DC File Offset: 0x001149DC
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
		if (type == BaseMission.MissionEventType.HARVEST)
		{
			string[] array = this.itemShortnames;
			int i = 0;
			while (i < array.Length)
			{
				if (array[i] == identifier)
				{
					instance.objectiveStatuses[index].genericInt1 += (int)amount;
					if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
					{
						this.CompleteObjective(index, instance, playerFor);
						playerFor.MissionDirty(true);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x00116752 File Offset: 0x00114952
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002614 RID: 9748
	public string[] itemShortnames;

	// Token: 0x04002615 RID: 9749
	public int targetItemAmount;
}
