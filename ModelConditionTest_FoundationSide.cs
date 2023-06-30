using System;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class ModelConditionTest_FoundationSide : ModelConditionTest
{
	// Token: 0x06001CAC RID: 7340 RVA: 0x000C74E8 File Offset: 0x000C56E8
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(1.5f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x000C753C File Offset: 0x000C573C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		Vector3 vector = this.worldRotation * Vector3.right;
		if (name.Contains("foundation.triangle"))
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/1";
			}
			if (vector.x < -0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/2";
			}
			if (vector.x > 0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/3";
				return;
			}
		}
		else
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/1";
			}
			if (vector.z > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/3";
			}
			if (vector.x < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/2";
			}
			if (vector.x > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/4";
			}
		}
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x000C7610 File Offset: 0x000C5810
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(this.socket);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token == "foundation_steps"))
			{
				if (buildingBlock.grade == BuildingGrade.Enum.TopTier)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Metal)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Stone)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0400153B RID: 5435
	private const string square_south = "foundation/sockets/foundation-top/1";

	// Token: 0x0400153C RID: 5436
	private const string square_north = "foundation/sockets/foundation-top/3";

	// Token: 0x0400153D RID: 5437
	private const string square_west = "foundation/sockets/foundation-top/2";

	// Token: 0x0400153E RID: 5438
	private const string square_east = "foundation/sockets/foundation-top/4";

	// Token: 0x0400153F RID: 5439
	private const string triangle_south = "foundation.triangle/sockets/foundation-top/1";

	// Token: 0x04001540 RID: 5440
	private const string triangle_northwest = "foundation.triangle/sockets/foundation-top/2";

	// Token: 0x04001541 RID: 5441
	private const string triangle_northeast = "foundation.triangle/sockets/foundation-top/3";

	// Token: 0x04001542 RID: 5442
	private string socket = string.Empty;
}
