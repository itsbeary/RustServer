using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class ModelConditionTest_RoofRight : ModelConditionTest
{
	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06001CC5 RID: 7365 RVA: 0x000C7B4B File Offset: 0x000C5D4B
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofRight.AngleType)10;
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06001CC6 RID: 7366 RVA: 0x000C7B57 File Offset: 0x000C5D57
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofRight.AngleType)(-10);
		}
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x000C7B64 File Offset: 0x000C5D64
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(-3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x000C7BB8 File Offset: 0x000C5DB8
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofRight.sockets_right);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofRight.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/4"))
				{
					return false;
				}
			}
			return true;
		}
		if (entityLink.IsEmpty())
		{
			return false;
		}
		bool flag = false;
		for (int j = 0; j < entityLink.connections.Count; j++)
		{
			EntityLink entityLink2 = entityLink.connections[j];
			if (entityLink2.name.EndsWith("sockets/neighbour/4") && (this.shape != ModelConditionTest_RoofRight.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofRight.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = -Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
					if (num2 < (float)(num - 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else if (num2 > (float)(num + 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else
					{
						flag = true;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x04001554 RID: 5460
	public ModelConditionTest_RoofRight.AngleType angle = ModelConditionTest_RoofRight.AngleType.None;

	// Token: 0x04001555 RID: 5461
	public ModelConditionTest_RoofRight.ShapeType shape = ModelConditionTest_RoofRight.ShapeType.Any;

	// Token: 0x04001556 RID: 5462
	private const string roof_square = "roof/";

	// Token: 0x04001557 RID: 5463
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001558 RID: 5464
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x04001559 RID: 5465
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x0400155A RID: 5466
	private static string[] sockets_right = new string[] { "roof/sockets/neighbour/3", "roof.triangle/sockets/neighbour/3" };

	// Token: 0x02000CA3 RID: 3235
	public enum AngleType
	{
		// Token: 0x040044A8 RID: 17576
		None = -1,
		// Token: 0x040044A9 RID: 17577
		Straight,
		// Token: 0x040044AA RID: 17578
		Convex60 = 60,
		// Token: 0x040044AB RID: 17579
		Convex90 = 90,
		// Token: 0x040044AC RID: 17580
		Convex120 = 120,
		// Token: 0x040044AD RID: 17581
		Concave30 = -30,
		// Token: 0x040044AE RID: 17582
		Concave60 = -60,
		// Token: 0x040044AF RID: 17583
		Concave90 = -90,
		// Token: 0x040044B0 RID: 17584
		Concave120 = -120
	}

	// Token: 0x02000CA4 RID: 3236
	public enum ShapeType
	{
		// Token: 0x040044B2 RID: 17586
		Any = -1,
		// Token: 0x040044B3 RID: 17587
		Square,
		// Token: 0x040044B4 RID: 17588
		Triangle
	}
}
