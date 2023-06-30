using System;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class ModelConditionTest_RampHigh : ModelConditionTest
{
	// Token: 0x06001CB5 RID: 7349 RVA: 0x000C7700 File Offset: 0x000C5900
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.75f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000C7754 File Offset: 0x000C5954
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && entityLink.IsEmpty();
	}

	// Token: 0x04001543 RID: 5443
	private const string socket = "ramp/sockets/block-male/1";
}
