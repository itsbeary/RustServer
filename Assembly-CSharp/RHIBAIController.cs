using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000478 RID: 1144
public class RHIBAIController : FacepunchBehaviour
{
	// Token: 0x060025DA RID: 9690 RVA: 0x000EECF4 File Offset: 0x000ECEF4
	[ContextMenu("Calculate Path")]
	public void SetupPatrolPath()
	{
		float x = TerrainMeta.Size.x;
		float num = x * 2f * 3.1415927f;
		float num2 = 30f;
		int num3 = Mathf.CeilToInt(num / num2);
		this.nodes = new List<Vector3>();
		float num4 = x;
		float num5 = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num6 = (float)i / (float)num3 * 360f;
			this.nodes.Add(new Vector3(Mathf.Sin(num6 * 0.017453292f) * num4, num5, Mathf.Cos(num6 * 0.017453292f) * num4));
		}
		float num7 = 2f;
		float num8 = 200f;
		float num9 = 150f;
		float num10 = 8f;
		bool flag = true;
		int num11 = 1;
		float num12 = 20f;
		Vector3[] array = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(num12, 0f, 0f),
			new Vector3(-num12, 0f, 0f),
			new Vector3(0f, 0f, num12),
			new Vector3(0f, 0f, -num12)
		};
		while (flag)
		{
			Debug.Log("Loop # :" + num11);
			num11++;
			flag = false;
			for (int j = 0; j < num3; j++)
			{
				Vector3 vector = this.nodes[j];
				int num13 = ((j == 0) ? (num3 - 1) : (j - 1));
				int num14 = ((j == num3 - 1) ? 0 : (j + 1));
				Vector3 vector2 = this.nodes[num14];
				Vector3 vector3 = this.nodes[num13];
				Vector3 vector4 = vector;
				Vector3 normalized = (Vector3.zero - vector).normalized;
				Vector3 vector5 = vector + normalized * num7;
				if (Vector3.Distance(vector5, vector2) <= num8 && Vector3.Distance(vector5, vector3) <= num8)
				{
					bool flag2 = true;
					for (int k = 0; k < array.Length; k++)
					{
						Vector3 vector6 = vector5 + array[k];
						if (this.GetWaterDepth(vector6) < num10)
						{
							flag2 = false;
						}
						Vector3 vector7 = normalized;
						if (vector6 != Vector3.zero)
						{
							vector7 = (vector6 - vector4).normalized;
						}
						RaycastHit raycastHit;
						if (Physics.Raycast(vector4, vector7, out raycastHit, num9, 1218511105))
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						flag = true;
						this.nodes[j] = vector5;
					}
				}
			}
		}
		List<int> list = new List<int>();
		LineUtility.Simplify(this.nodes, 15f, list);
		List<Vector3> list2 = this.nodes;
		this.nodes = new List<Vector3>();
		foreach (int num15 in list)
		{
			this.nodes.Add(list2[num15]);
		}
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000EF014 File Offset: 0x000ED214
	public float GetWaterDepth(Vector3 pos)
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000EF048 File Offset: 0x000ED248
	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolClose != null)
		{
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolClose.Count; i++)
			{
				Vector3 vector = TerrainMeta.Path.OceanPatrolClose[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(vector, 3f);
				Vector3 vector2 = ((i + 1 == TerrainMeta.Path.OceanPatrolClose.Count) ? TerrainMeta.Path.OceanPatrolClose[0] : TerrainMeta.Path.OceanPatrolClose[i + 1]);
				Gizmos.DrawLine(vector, vector2);
			}
		}
	}

	// Token: 0x04001E02 RID: 7682
	public List<Vector3> nodes = new List<Vector3>();
}
