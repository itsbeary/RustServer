using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class RuntimePath : IAIPath
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x060018ED RID: 6381 RVA: 0x000B8A17 File Offset: 0x000B6C17
	// (set) Token: 0x060018EE RID: 6382 RVA: 0x000B8A1F File Offset: 0x000B6C1F
	public IAIPathNode[] Nodes { get; set; } = new IAIPathNode[0];

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060018EF RID: 6383 RVA: 0x000B8A28 File Offset: 0x000B6C28
	public IEnumerable<IAIPathSpeedZone> SpeedZones
	{
		get
		{
			return this.speedZones;
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060018F0 RID: 6384 RVA: 0x000B8A30 File Offset: 0x000B6C30
	public IEnumerable<IAIPathInterestNode> InterestNodes
	{
		get
		{
			return this.interestNodes;
		}
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x000B8A38 File Offset: 0x000B6C38
	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		IAIPathNode iaipathNode = this.Nodes[0];
		float num = float.PositiveInfinity;
		foreach (IAIPathNode iaipathNode2 in this.Nodes)
		{
			float sqrMagnitude = (point - iaipathNode2.Position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				iaipathNode = iaipathNode2;
			}
		}
		return iaipathNode;
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x000B8A94 File Offset: 0x000B6C94
	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		foreach (IAIPathNode iaipathNode in this.Nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(iaipathNode.Position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(iaipathNode);
			}
		}
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x000B8AE8 File Offset: 0x000B6CE8
	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		IAIPathInterestNode iaipathInterestNode = null;
		int num = 0;
		while (iaipathInterestNode == null && num < 20)
		{
			iaipathInterestNode = this.interestNodes[UnityEngine.Random.Range(0, this.interestNodes.Count)];
			if ((iaipathInterestNode.Position - from).sqrMagnitude >= dist * dist)
			{
				break;
			}
			iaipathInterestNode = null;
			num++;
		}
		if (iaipathInterestNode == null)
		{
			Debug.LogError("Returning default interest zone");
			iaipathInterestNode = this.interestNodes[0];
		}
		return iaipathInterestNode;
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x000B8B5A File Offset: 0x000B6D5A
	public void AddInterestNode(IAIPathInterestNode interestNode)
	{
		if (this.interestNodes.Contains(interestNode))
		{
			return;
		}
		this.interestNodes.Add(interestNode);
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x000B8B77 File Offset: 0x000B6D77
	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		if (this.speedZones.Contains(speedZone))
		{
			return;
		}
		this.speedZones.Add(speedZone);
	}

	// Token: 0x0400117E RID: 4478
	private List<IAIPathSpeedZone> speedZones = new List<IAIPathSpeedZone>();

	// Token: 0x0400117F RID: 4479
	private List<IAIPathInterestNode> interestNodes = new List<IAIPathInterestNode>();
}
