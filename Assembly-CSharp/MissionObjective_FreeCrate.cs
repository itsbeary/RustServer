using System;
using UnityEngine;

// Token: 0x02000620 RID: 1568
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/FreeCrate")]
public class MissionObjective_FreeCrate : MissionObjective
{
	// Token: 0x06002E66 RID: 11878 RVA: 0x001166C5 File Offset: 0x001148C5
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x00116768 File Offset: 0x00114968
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
		if (type == BaseMission.MissionEventType.FREE_CRATE)
		{
			instance.objectiveStatuses[index].genericInt1 += (int)amount;
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x00116752 File Offset: 0x00114952
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002613 RID: 9747
	public int targetAmount;
}
