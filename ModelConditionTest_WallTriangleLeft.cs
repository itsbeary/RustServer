using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class ModelConditionTest_WallTriangleLeft : ModelConditionTest
{
	// Token: 0x06001CDF RID: 7391 RVA: 0x000C825C File Offset: 0x000C645C
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/stability/1"))
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
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x000C83A0 File Offset: 0x000C65A0
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x000C83C3 File Offset: 0x000C65C3
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleLeft.CheckCondition(ent);
	}

	// Token: 0x0400156D RID: 5485
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x0400156E RID: 5486
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x0400156F RID: 5487
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04001570 RID: 5488
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001571 RID: 5489
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x04001572 RID: 5490
	private const string socket_6 = "wall/sockets/stability/1";

	// Token: 0x04001573 RID: 5491
	private const string socket = "wall/sockets/neighbour/1";
}
