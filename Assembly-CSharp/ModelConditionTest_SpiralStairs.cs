using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class ModelConditionTest_SpiralStairs : ModelConditionTest
{
	// Token: 0x06001CD1 RID: 7377 RVA: 0x000C7ED0 File Offset: 0x000C60D0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 2.35f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x000C7F24 File Offset: 0x000C6124
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_SpiralStairs.stairs_sockets_female);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock2 = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
			{
				return false;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_SpiralStairs.floor_sockets_female);
		return entityLink2 == null || entityLink2.IsEmpty();
	}

	// Token: 0x04001564 RID: 5476
	private const string stairs_socket_female = "sockets/stairs-female/1";

	// Token: 0x04001565 RID: 5477
	private static string[] stairs_sockets_female = new string[] { "block.stair.spiral/sockets/stairs-female/1", "block.stair.spiral.triangle/sockets/stairs-female/1" };

	// Token: 0x04001566 RID: 5478
	private const string floor_socket_female = "sockets/floor-female/1";

	// Token: 0x04001567 RID: 5479
	private static string[] floor_sockets_female = new string[] { "block.stair.spiral/sockets/floor-female/1", "block.stair.spiral.triangle/sockets/floor-female/1" };
}
