using System;
using UnityEngine;

// Token: 0x02000396 RID: 918
public interface IAIGroupable
{
	// Token: 0x06002089 RID: 8329
	bool AddMember(IAIGroupable member);

	// Token: 0x0600208A RID: 8330
	void RemoveMember(IAIGroupable member);

	// Token: 0x0600208B RID: 8331
	void JoinGroup(IAIGroupable leader, BaseEntity leaderEntity);

	// Token: 0x0600208C RID: 8332
	void SetGroupRoamRootPosition(Vector3 rootPos);

	// Token: 0x0600208D RID: 8333
	bool InGroup();

	// Token: 0x0600208E RID: 8334
	void LeaveGroup();

	// Token: 0x0600208F RID: 8335
	void SetUngrouped();
}
