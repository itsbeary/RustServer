using System;

// Token: 0x02000618 RID: 1560
public interface IMissionEntityListener
{
	// Token: 0x06002E3D RID: 11837
	void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance);

	// Token: 0x06002E3E RID: 11838
	void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance);
}
