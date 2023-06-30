using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class ModelConditionTest_WallCornerLeft : ModelConditionTest
{
	// Token: 0x06001CD9 RID: 7385 RVA: 0x000C8014 File Offset: 0x000C6214
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(ModelConditionTest_WallCornerLeft.sockets);
		if (entityLink == null)
		{
			return false;
		}
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			EntityLink entityLink2 = entityLink.connections[i];
			BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
			if (!(buildingBlock2 == null))
			{
				float num = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
				if (entityLink2.name.EndsWith("sockets/stability/2"))
				{
					if (num > -10f || num < -100f)
					{
						return false;
					}
				}
				else
				{
					if (num < 10f && num > -10f)
					{
						return false;
					}
					if (num < -10f)
					{
						return false;
					}
					if (buildingBlock2.grade == buildingBlock.grade)
					{
						flag = true;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x04001569 RID: 5481
	private const string socket = "sockets/stability/2";

	// Token: 0x0400156A RID: 5482
	private static string[] sockets = new string[] { "wall/sockets/stability/2", "wall.half/sockets/stability/2", "wall.low/sockets/stability/2", "wall.doorway/sockets/stability/2", "wall.window/sockets/stability/2" };
}
