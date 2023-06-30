using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B55 RID: 2901
	public static class AStarPath
	{
		// Token: 0x06004636 RID: 17974 RVA: 0x001992D5 File Offset: 0x001974D5
		private static float Heuristic(IAIPathNode from, IAIPathNode to)
		{
			return Vector3.Distance(from.Position, to.Position);
		}

		// Token: 0x06004637 RID: 17975 RVA: 0x001992E8 File Offset: 0x001974E8
		public static bool FindPath(IAIPathNode start, IAIPathNode goal, out Stack<IAIPathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool flag = false;
			if (start == goal)
			{
				return false;
			}
			AStarNodeList astarNodeList = new AStarNodeList();
			HashSet<IAIPathNode> hashSet = new HashSet<IAIPathNode>();
			AStarNode astarNode = new AStarNode(0f, AStarPath.Heuristic(start, goal), null, start);
			astarNodeList.Add(astarNode);
			while (astarNodeList.Count > 0)
			{
				AStarNode astarNode2 = astarNodeList[0];
				astarNodeList.RemoveAt(0);
				hashSet.Add(astarNode2.Node);
				if (astarNode2.Satisfies(goal))
				{
					path = new Stack<IAIPathNode>();
					pathCost = 0f;
					while (astarNode2.Parent != null)
					{
						pathCost += astarNode2.F;
						path.Push(astarNode2.Node);
						astarNode2 = astarNode2.Parent;
					}
					if (astarNode2 != null)
					{
						path.Push(astarNode2.Node);
					}
					flag = true;
					break;
				}
				foreach (IAIPathNode iaipathNode in astarNode2.Node.Linked)
				{
					if (!hashSet.Contains(iaipathNode))
					{
						float num = astarNode2.G + AStarPath.Heuristic(astarNode2.Node, iaipathNode);
						AStarNode astarNode3 = astarNodeList.GetAStarNodeOf(iaipathNode);
						if (astarNode3 == null)
						{
							astarNode3 = new AStarNode(num, AStarPath.Heuristic(iaipathNode, goal), astarNode2, iaipathNode);
							astarNodeList.Add(astarNode3);
							astarNodeList.AStarNodeSort();
						}
						else if (num < astarNode3.G)
						{
							astarNode3.Update(num, astarNode3.H, astarNode2, iaipathNode);
							astarNodeList.AStarNodeSort();
						}
					}
				}
			}
			return flag;
		}
	}
}
