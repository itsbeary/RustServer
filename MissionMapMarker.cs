using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FD RID: 2045
public class MissionMapMarker : MonoBehaviour
{
	// Token: 0x0600359E RID: 13726 RVA: 0x00146DC4 File Offset: 0x00144FC4
	public void Populate(BaseMission.MissionInstance mission)
	{
		BaseMission mission2 = mission.GetMission();
		this.Icon.sprite = mission2.icon;
		this.TooltipComponent.token = mission2.missionName.token;
		this.TooltipComponent.Text = mission2.missionName.english;
	}

	// Token: 0x04002E1E RID: 11806
	public Image Icon;

	// Token: 0x04002E1F RID: 11807
	public Tooltip TooltipComponent;
}
