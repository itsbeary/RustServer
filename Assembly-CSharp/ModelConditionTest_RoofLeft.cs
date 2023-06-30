using System;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class ModelConditionTest_RoofLeft : ModelConditionTest
{
	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001CBF RID: 7359 RVA: 0x000C7933 File Offset: 0x000C5B33
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofLeft.AngleType)10;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001CC0 RID: 7360 RVA: 0x000C793F File Offset: 0x000C5B3F
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofLeft.AngleType)(-10);
		}
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x000C794C File Offset: 0x000C5B4C
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x000C79A0 File Offset: 0x000C5BA0
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofLeft.sockets_left);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofLeft.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
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
			if (entityLink2.name.EndsWith("sockets/neighbour/3") && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
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

	// Token: 0x0400154D RID: 5453
	public ModelConditionTest_RoofLeft.AngleType angle = ModelConditionTest_RoofLeft.AngleType.None;

	// Token: 0x0400154E RID: 5454
	public ModelConditionTest_RoofLeft.ShapeType shape = ModelConditionTest_RoofLeft.ShapeType.Any;

	// Token: 0x0400154F RID: 5455
	private const string roof_square = "roof/";

	// Token: 0x04001550 RID: 5456
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001551 RID: 5457
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x04001552 RID: 5458
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x04001553 RID: 5459
	private static string[] sockets_left = new string[] { "roof/sockets/neighbour/4", "roof.triangle/sockets/neighbour/4" };

	// Token: 0x02000CA1 RID: 3233
	public enum AngleType
	{
		// Token: 0x0400449A RID: 17562
		None = -1,
		// Token: 0x0400449B RID: 17563
		Straight,
		// Token: 0x0400449C RID: 17564
		Convex60 = 60,
		// Token: 0x0400449D RID: 17565
		Convex90 = 90,
		// Token: 0x0400449E RID: 17566
		Convex120 = 120,
		// Token: 0x0400449F RID: 17567
		Concave30 = -30,
		// Token: 0x040044A0 RID: 17568
		Concave60 = -60,
		// Token: 0x040044A1 RID: 17569
		Concave90 = -90,
		// Token: 0x040044A2 RID: 17570
		Concave120 = -120
	}

	// Token: 0x02000CA2 RID: 3234
	public enum ShapeType
	{
		// Token: 0x040044A4 RID: 17572
		Any = -1,
		// Token: 0x040044A5 RID: 17573
		Square,
		// Token: 0x040044A6 RID: 17574
		Triangle
	}
}
