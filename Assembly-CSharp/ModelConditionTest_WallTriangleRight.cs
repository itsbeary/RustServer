using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
	// Token: 0x06001CE3 RID: 7395 RVA: 0x000C83CC File Offset: 0x000C65CC
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/stability/2"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null))
			{
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x000C8518 File Offset: 0x000C6718
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x000C853B File Offset: 0x000C673B
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}

	// Token: 0x04001574 RID: 5492
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x04001575 RID: 5493
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04001576 RID: 5494
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04001577 RID: 5495
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001578 RID: 5496
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x04001579 RID: 5497
	private const string socket_6 = "wall/sockets/stability/2";

	// Token: 0x0400157A RID: 5498
	private const string socket = "wall/sockets/neighbour/1";
}
