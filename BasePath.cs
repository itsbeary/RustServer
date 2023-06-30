using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class BasePath : MonoBehaviour, IAIPath
{
	// Token: 0x17000208 RID: 520
	// (get) Token: 0x060018AB RID: 6315 RVA: 0x000B7F75 File Offset: 0x000B6175
	public IEnumerable<IAIPathInterestNode> InterestNodes
	{
		get
		{
			return this.interestZones;
		}
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x060018AC RID: 6316 RVA: 0x000B7F7D File Offset: 0x000B617D
	public IEnumerable<IAIPathSpeedZone> SpeedZones
	{
		get
		{
			return this.speedZones;
		}
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x000B7F88 File Offset: 0x000B6188
	private void AddChildren()
	{
		if (this.nodes != null)
		{
			this.nodes.Clear();
			this.nodes.AddRange(base.GetComponentsInChildren<BasePathNode>());
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.Path = this;
			}
		}
		if (this.interestZones != null)
		{
			this.interestZones.Clear();
			this.interestZones.AddRange(base.GetComponentsInChildren<PathInterestNode>());
		}
		if (this.speedZones != null)
		{
			this.speedZones.Clear();
			this.speedZones.AddRange(base.GetComponentsInChildren<PathSpeedZone>());
		}
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x000B8048 File Offset: 0x000B6248
	private void ClearChildren()
	{
		if (this.nodes != null)
		{
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.linked.Clear();
			}
		}
		this.nodes.Clear();
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x000B80B0 File Offset: 0x000B62B0
	public static void AutoGenerateLinks(BasePath path, float maxRange = -1f)
	{
		path.AddChildren();
		foreach (BasePathNode basePathNode in path.nodes)
		{
			if (basePathNode.linked == null)
			{
				basePathNode.linked = new List<BasePathNode>();
			}
			else
			{
				basePathNode.linked.Clear();
			}
			foreach (BasePathNode basePathNode2 in path.nodes)
			{
				if (!(basePathNode == basePathNode2) && (maxRange == -1f || Vector3.Distance(basePathNode.Position, basePathNode2.Position) <= maxRange) && GamePhysics.LineOfSight(basePathNode.Position, basePathNode2.Position, 429990145, null) && GamePhysics.LineOfSight(basePathNode2.Position, basePathNode.Position, 429990145, null))
				{
					basePathNode.linked.Add(basePathNode2);
				}
			}
		}
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x000B81C8 File Offset: 0x000B63C8
	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(basePathNode.Position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(basePathNode);
			}
		}
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x000B8240 File Offset: 0x000B6440
	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		IAIPathNode iaipathNode = this.nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if (!(basePathNode == null) && !(basePathNode.transform == null))
			{
				float sqrMagnitude = (point - basePathNode.Position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					iaipathNode = basePathNode;
				}
			}
		}
		return iaipathNode;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x000B82D8 File Offset: 0x000B64D8
	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while (pathInterestNode == null && num < 20)
		{
			pathInterestNode = this.interestZones[UnityEngine.Random.Range(0, this.interestZones.Count)];
			if ((pathInterestNode.transform.position - from).sqrMagnitude >= dist * dist)
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if (pathInterestNode == null)
		{
			Debug.LogError("REturning default interest zone");
			pathInterestNode = this.interestZones[0];
		}
		return pathInterestNode;
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x0002A507 File Offset: 0x00028707
	public void AddInterestNode(IAIPathInterestNode interestZone)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x0002A507 File Offset: 0x00028707
	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04001159 RID: 4441
	public List<BasePathNode> nodes;

	// Token: 0x0400115A RID: 4442
	public List<PathInterestNode> interestZones;

	// Token: 0x0400115B RID: 4443
	public List<PathSpeedZone> speedZones;
}
