using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class ModelConditionTest_RoofTop : ModelConditionTest
{
	// Token: 0x06001CCB RID: 7371 RVA: 0x000C7D64 File Offset: 0x000C5F64
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x000C7DB8 File Offset: 0x000C5FB8
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/4"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}

	// Token: 0x0400155B RID: 5467
	private const string roof_square = "roof/";

	// Token: 0x0400155C RID: 5468
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x0400155D RID: 5469
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x0400155E RID: 5470
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x0400155F RID: 5471
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x04001560 RID: 5472
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x04001561 RID: 5473
	private static string[] sockets_top_right = new string[] { "roof/sockets/neighbour/5", "roof.triangle/sockets/neighbour/5" };

	// Token: 0x04001562 RID: 5474
	private static string[] sockets_top_left = new string[] { "roof/sockets/neighbour/6", "roof.triangle/sockets/neighbour/6" };
}
