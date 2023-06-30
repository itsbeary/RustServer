using System;
using UnityEngine;

// Token: 0x02000678 RID: 1656
public class DungeonGridCell : MonoBehaviour
{
	// Token: 0x06002FE8 RID: 12264 RVA: 0x00120694 File Offset: 0x0011E894
	public bool ShouldAvoid(uint id)
	{
		GameObjectRef[] avoidNeighbours = this.AvoidNeighbours;
		for (int i = 0; i < avoidNeighbours.Length; i++)
		{
			if (avoidNeighbours[i].resourceID == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x001206C4 File Offset: 0x0011E8C4
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridCells.Add(this);
		}
	}

	// Token: 0x04002766 RID: 10086
	public DungeonGridConnectionType North;

	// Token: 0x04002767 RID: 10087
	public DungeonGridConnectionType South;

	// Token: 0x04002768 RID: 10088
	public DungeonGridConnectionType West;

	// Token: 0x04002769 RID: 10089
	public DungeonGridConnectionType East;

	// Token: 0x0400276A RID: 10090
	public DungeonGridConnectionVariant NorthVariant;

	// Token: 0x0400276B RID: 10091
	public DungeonGridConnectionVariant SouthVariant;

	// Token: 0x0400276C RID: 10092
	public DungeonGridConnectionVariant WestVariant;

	// Token: 0x0400276D RID: 10093
	public DungeonGridConnectionVariant EastVariant;

	// Token: 0x0400276E RID: 10094
	public GameObjectRef[] AvoidNeighbours;

	// Token: 0x0400276F RID: 10095
	public MeshRenderer[] MapRenderers;
}
