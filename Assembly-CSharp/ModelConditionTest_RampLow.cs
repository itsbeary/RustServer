using System;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class ModelConditionTest_RampLow : ModelConditionTest
{
	// Token: 0x06001CB8 RID: 7352 RVA: 0x000C7778 File Offset: 0x000C5978
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.375f, 0f), new Vector3(3f, 0.75f, 3f));
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x000C77CC File Offset: 0x000C59CC
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x04001544 RID: 5444
	private const string socket = "ramp/sockets/block-male/1";
}
