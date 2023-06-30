using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class CargoMoveTest : FacepunchBehaviour
{
	// Token: 0x060000C5 RID: 197 RVA: 0x00005F68 File Offset: 0x00004168
	private void Awake()
	{
		base.Invoke(new Action(this.FindInitialNode), 2f);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00005F81 File Offset: 0x00004181
	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00005F8F File Offset: 0x0000418F
	private void Update()
	{
		this.UpdateMovement();
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00005F98 File Offset: 0x00004198
	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 vector = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		Vector3 normalized = (vector - base.transform.position).normalized;
		float num = Vector3.Dot(base.transform.forward, normalized);
		float num2 = Mathf.InverseLerp(0.5f, 1f, num);
		float num3 = Vector3.Dot(base.transform.right, normalized);
		float num4 = 5f;
		float num5 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num3));
		this.turnScale = Mathf.Lerp(this.turnScale, num5, Time.deltaTime * 0.2f);
		float num6 = (float)((num3 < 0f) ? (-1) : 1);
		base.transform.Rotate(Vector3.up, num4 * Time.deltaTime * this.turnScale * num6, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, num2, Time.deltaTime * 0.2f);
		base.transform.position += base.transform.forward * 5f * Time.deltaTime * this.currentThrottle;
		if (Vector3.Distance(base.transform.position, vector) < 60f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000614C File Offset: 0x0000434C
	public int GetClosestNodeToUs()
	{
		int num = 0;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 vector = TerrainMeta.Path.OceanPatrolFar[i];
			float num3 = Vector3.Distance(base.transform.position, vector);
			if (num3 < num2)
			{
				num = i;
				num2 = num3;
			}
		}
		return num;
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000061AC File Offset: 0x000043AC
	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolFar != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex], 10f);
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
			{
				Vector3 vector = TerrainMeta.Path.OceanPatrolFar[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(vector, 3f);
				Vector3 vector2 = ((i + 1 == TerrainMeta.Path.OceanPatrolFar.Count) ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[i + 1]);
				Gizmos.DrawLine(vector, vector2);
			}
		}
	}

	// Token: 0x040000C4 RID: 196
	public int targetNodeIndex = -1;

	// Token: 0x040000C5 RID: 197
	private float currentThrottle;

	// Token: 0x040000C6 RID: 198
	private float turnScale;
}
