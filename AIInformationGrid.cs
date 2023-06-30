using System;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class AIInformationGrid : MonoBehaviour
{
	// Token: 0x0600196D RID: 6509 RVA: 0x000BA750 File Offset: 0x000B8950
	[ContextMenu("Init")]
	public void Init()
	{
		AIInformationZone component = base.GetComponent<AIInformationZone>();
		if (component == null)
		{
			Debug.LogWarning("Unable to Init AIInformationGrid, no AIInformationZone found!");
			return;
		}
		this.BoundingBox = component.bounds;
		this.BoundingBox.center = base.transform.position + component.bounds.center + new Vector3(0f, this.BoundingBox.extents.y, 0f);
		float num = this.BoundingBox.extents.x * 2f;
		float num2 = this.BoundingBox.extents.z * 2f;
		this.xCellCount = (int)Mathf.Ceil(num / (float)this.CellSize);
		this.zCellCount = (int)Mathf.Ceil(num2 / (float)this.CellSize);
		this.Cells = new AIInformationCell[this.xCellCount * this.zCellCount];
		Vector3 min = this.BoundingBox.min;
		this.origin = min;
		min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
		min.z = this.BoundingBox.min.z + (float)this.CellSize / 2f;
		for (int i = 0; i < this.zCellCount; i++)
		{
			for (int j = 0; j < this.xCellCount; j++)
			{
				Vector3 vector = min;
				Bounds bounds = new Bounds(vector, new Vector3((float)this.CellSize, this.BoundingBox.extents.y * 2f, (float)this.CellSize));
				this.Cells[this.GetIndex(j, i)] = new AIInformationCell(bounds, base.gameObject, j, i);
				min.x += (float)this.CellSize;
			}
			min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
			min.z += (float)this.CellSize;
		}
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000BA96B File Offset: 0x000B8B6B
	private int GetIndex(int x, int z)
	{
		return z * this.xCellCount + x;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x000BA977 File Offset: 0x000B8B77
	public AIInformationCell CellAt(int x, int z)
	{
		return this.Cells[this.GetIndex(x, z)];
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x000BA988 File Offset: 0x000B8B88
	public AIMovePoint[] GetMovePointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AIMovePoint aimovePoint in cellsInRange[i].MovePoints.Items)
					{
						this.movePointResults[pointCount] = aimovePoint;
						pointCount++;
					}
				}
			}
		}
		return this.movePointResults;
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x000BAA18 File Offset: 0x000B8C18
	public AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AICoverPoint aicoverPoint in cellsInRange[i].CoverPoints.Items)
					{
						this.coverPointResults[pointCount] = aicoverPoint;
						pointCount++;
					}
				}
			}
		}
		return this.coverPointResults;
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000BAAA8 File Offset: 0x000B8CA8
	public AIInformationCell[] GetCellsInRange(Vector3 position, float maxRange, out int cellCount)
	{
		cellCount = 0;
		int num = (int)(maxRange / (float)this.CellSize);
		AIInformationCell cell = this.GetCell(position);
		if (cell == null)
		{
			return this.resultCells;
		}
		int num2 = Mathf.Max(cell.X - num, 0);
		int num3 = Mathf.Min(cell.X + num, this.xCellCount - 1);
		int num4 = Mathf.Max(cell.Z - num, 0);
		int num5 = Mathf.Min(cell.Z + num, this.zCellCount - 1);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				this.resultCells[cellCount] = this.CellAt(j, i);
				cellCount++;
				if (cellCount >= 512)
				{
					return this.resultCells;
				}
			}
		}
		return this.resultCells;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000BAB74 File Offset: 0x000B8D74
	public AIInformationCell GetCell(Vector3 position)
	{
		if (this.Cells == null)
		{
			return null;
		}
		Vector3 vector = position - this.origin;
		if (vector.x < 0f || vector.z < 0f)
		{
			return null;
		}
		int num = (int)(vector.x / (float)this.CellSize);
		int num2 = (int)(vector.z / (float)this.CellSize);
		if (num < 0 || num >= this.xCellCount)
		{
			return null;
		}
		if (num2 < 0 || num2 >= this.zCellCount)
		{
			return null;
		}
		return this.CellAt(num, num2);
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000BABFA File Offset: 0x000B8DFA
	public void OnDrawGizmos()
	{
		this.DebugDraw();
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000BAC04 File Offset: 0x000B8E04
	public void DebugDraw()
	{
		if (this.Cells == null)
		{
			return;
		}
		foreach (AIInformationCell aiinformationCell in this.Cells)
		{
			if (aiinformationCell != null)
			{
				aiinformationCell.DebugDraw(Color.white, false, 1f);
			}
		}
	}

	// Token: 0x04001238 RID: 4664
	public int CellSize = 10;

	// Token: 0x04001239 RID: 4665
	public Bounds BoundingBox;

	// Token: 0x0400123A RID: 4666
	public AIInformationCell[] Cells;

	// Token: 0x0400123B RID: 4667
	private Vector3 origin;

	// Token: 0x0400123C RID: 4668
	private int xCellCount;

	// Token: 0x0400123D RID: 4669
	private int zCellCount;

	// Token: 0x0400123E RID: 4670
	private const int maxPointResults = 2048;

	// Token: 0x0400123F RID: 4671
	private AIMovePoint[] movePointResults = new AIMovePoint[2048];

	// Token: 0x04001240 RID: 4672
	private AICoverPoint[] coverPointResults = new AICoverPoint[2048];

	// Token: 0x04001241 RID: 4673
	private const int maxCellResults = 512;

	// Token: 0x04001242 RID: 4674
	private AIInformationCell[] resultCells = new AIInformationCell[512];
}
