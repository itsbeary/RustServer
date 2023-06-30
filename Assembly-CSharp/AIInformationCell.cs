using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class AIInformationCell
{
	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001962 RID: 6498 RVA: 0x000BA528 File Offset: 0x000B8728
	public int X { get; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001963 RID: 6499 RVA: 0x000BA530 File Offset: 0x000B8730
	public int Z { get; }

	// Token: 0x06001964 RID: 6500 RVA: 0x000BA538 File Offset: 0x000B8738
	public AIInformationCell(Bounds bounds, GameObject root, int x, int z)
	{
		this.BoundingBox = bounds;
		this.X = x;
		this.Z = z;
		this.MovePoints.Init(bounds, root);
		this.CoverPoints.Init(bounds, root);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x000BA59C File Offset: 0x000B879C
	public void DebugDraw(Color color, bool points, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireCube(this.BoundingBox.center, this.BoundingBox.size * scale);
		Gizmos.color = color2;
		if (points)
		{
			foreach (AIMovePoint aimovePoint in this.MovePoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aimovePoint.transform.position);
			}
			foreach (AICoverPoint aicoverPoint in this.CoverPoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aicoverPoint.transform.position);
			}
		}
	}

	// Token: 0x04001231 RID: 4657
	public Bounds BoundingBox;

	// Token: 0x04001232 RID: 4658
	public List<AIInformationCell> NeighbourCells = new List<AIInformationCell>();

	// Token: 0x04001233 RID: 4659
	public AIInformationCellContents<AIMovePoint> MovePoints = new AIInformationCellContents<AIMovePoint>();

	// Token: 0x04001234 RID: 4660
	public AIInformationCellContents<AICoverPoint> CoverPoints = new AIInformationCellContents<AICoverPoint>();
}
