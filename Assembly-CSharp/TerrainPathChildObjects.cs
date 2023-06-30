using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class TerrainPathChildObjects : MonoBehaviour
{
	// Token: 0x0600300C RID: 12300 RVA: 0x00120DA8 File Offset: 0x0011EFA8
	protected void Awake()
	{
		if (!World.Cached && !World.Networked)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				list.Add(transform.position);
			}
			if (list.Count >= 2)
			{
				InfrastructureType type = this.Type;
				if (type != InfrastructureType.Road)
				{
					if (type == InfrastructureType.Power)
					{
						PathList pathList = new PathList("Powerline " + TerrainMeta.Path.Powerlines.Count, list.ToArray());
						pathList.Width = this.Width;
						pathList.InnerFade = this.Fade * 0.5f;
						pathList.OuterFade = this.Fade * 0.5f;
						pathList.MeshOffset = this.Offset * 0.3f;
						pathList.TerrainOffset = this.Offset;
						pathList.Topology = (int)this.Topology;
						pathList.Splat = (int)this.Splat;
						pathList.Spline = this.Spline;
						pathList.Path.RecalculateTangents();
						TerrainMeta.Path.Powerlines.Add(pathList);
					}
				}
				else
				{
					PathList pathList2 = new PathList("Road " + TerrainMeta.Path.Roads.Count, list.ToArray());
					pathList2.Width = this.Width;
					pathList2.InnerFade = this.Fade * 0.5f;
					pathList2.OuterFade = this.Fade * 0.5f;
					pathList2.MeshOffset = this.Offset * 0.3f;
					pathList2.TerrainOffset = this.Offset;
					pathList2.Topology = (int)this.Topology;
					pathList2.Splat = (int)this.Splat;
					pathList2.Spline = this.Spline;
					pathList2.Path.RecalculateTangents();
					TerrainMeta.Path.Roads.Add(pathList2);
				}
			}
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x00120FE0 File Offset: 0x0011F1E0
	protected void OnDrawGizmos()
	{
		bool flag = false;
		Vector3 vector = Vector3.zero;
		foreach (object obj in base.transform)
		{
			Vector3 position = ((Transform)obj).position;
			if (flag)
			{
				Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GizmosUtil.DrawWirePath(vector, position, 0.5f * this.Width);
			}
			vector = position;
			flag = true;
		}
	}

	// Token: 0x040027A6 RID: 10150
	public bool Spline = true;

	// Token: 0x040027A7 RID: 10151
	public float Width;

	// Token: 0x040027A8 RID: 10152
	public float Offset;

	// Token: 0x040027A9 RID: 10153
	public float Fade;

	// Token: 0x040027AA RID: 10154
	[InspectorFlags]
	public TerrainSplat.Enum Splat = TerrainSplat.Enum.Dirt;

	// Token: 0x040027AB RID: 10155
	[InspectorFlags]
	public TerrainTopology.Enum Topology = TerrainTopology.Enum.Road;

	// Token: 0x040027AC RID: 10156
	public InfrastructureType Type;
}
