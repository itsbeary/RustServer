using System;
using UnityEngine;

// Token: 0x02000623 RID: 1571
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Move")]
public class MissionObjective_Move : MissionObjective
{
	// Token: 0x06002E72 RID: 11890 RVA: 0x00116A43 File Offset: 0x00114C43
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
		instance.missionLocation = instance.GetMissionPoint(this.positionName, playerFor);
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x00116A68 File Offset: 0x00114C68
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
		if (!this.ShouldThink(index, instance))
		{
			return;
		}
		Vector3 missionPoint = instance.GetMissionPoint(this.positionName, assignee);
		if ((this.use2D ? Vector3Ex.Distance2D(missionPoint, assignee.transform.position) : Vector3.Distance(missionPoint, assignee.transform.position)) <= this.distForCompletion)
		{
			this.CompleteObjective(index, instance, assignee);
			assignee.MissionDirty(true);
		}
	}

	// Token: 0x0400261A RID: 9754
	public string positionName = "default";

	// Token: 0x0400261B RID: 9755
	public float distForCompletion = 3f;

	// Token: 0x0400261C RID: 9756
	public bool use2D;
}
