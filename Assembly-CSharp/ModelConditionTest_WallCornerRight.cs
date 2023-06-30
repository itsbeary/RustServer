using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class ModelConditionTest_WallCornerRight : ModelConditionTest
{
	// Token: 0x06001CDC RID: 7388 RVA: 0x000C8138 File Offset: 0x000C6338
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(ModelConditionTest_WallCornerRight.sockets);
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
				if (entityLink2.name.EndsWith("sockets/stability/1"))
				{
					if (num < 10f || num > 100f)
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
					if (num > 10f)
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

	// Token: 0x0400156B RID: 5483
	private const string socket = "sockets/stability/1";

	// Token: 0x0400156C RID: 5484
	private static string[] sockets = new string[] { "wall/sockets/stability/1", "wall.half/sockets/stability/1", "wall.low/sockets/stability/1", "wall.doorway/sockets/stability/1", "wall.window/sockets/stability/1" };
}
