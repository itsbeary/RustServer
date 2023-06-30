using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class ModelConditionTest_RoofBottom : ModelConditionTest
{
	// Token: 0x06001CBB RID: 7355 RVA: 0x000C77F4 File Offset: 0x000C59F4
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x000C7848 File Offset: 0x000C5A48
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/5"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/6"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}

	// Token: 0x04001545 RID: 5445
	private const string roof_square = "roof/";

	// Token: 0x04001546 RID: 5446
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001547 RID: 5447
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x04001548 RID: 5448
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x04001549 RID: 5449
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x0400154A RID: 5450
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x0400154B RID: 5451
	private static string[] sockets_bot_right = new string[] { "roof/sockets/neighbour/3", "roof.triangle/sockets/neighbour/3" };

	// Token: 0x0400154C RID: 5452
	private static string[] sockets_bot_left = new string[] { "roof/sockets/neighbour/4", "roof.triangle/sockets/neighbour/4" };
}
