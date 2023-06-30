using System;

// Token: 0x0200026B RID: 619
public class ModelConditionTest_RoofTriangle : ModelConditionTest
{
	// Token: 0x06001CCF RID: 7375 RVA: 0x000C7EA4 File Offset: 0x000C60A4
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/wall-female");
		return entityLink == null || entityLink.IsEmpty();
	}

	// Token: 0x04001563 RID: 5475
	private const string socket = "roof/sockets/wall-female";
}
