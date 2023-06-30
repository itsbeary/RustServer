using System;
using System.Collections.Generic;

namespace Rust.AI
{
	// Token: 0x02000B57 RID: 2903
	public class AStarNodeList : List<AStarNode>
	{
		// Token: 0x0600463E RID: 17982 RVA: 0x00199504 File Offset: 0x00197704
		public bool Contains(IAIPathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x00199540 File Offset: 0x00197740
		public AStarNode GetAStarNodeOf(IAIPathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return astarNode;
				}
			}
			return null;
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x0019957A File Offset: 0x0019777A
		public void AStarNodeSort()
		{
			base.Sort(this.comparer);
		}

		// Token: 0x04003F2D RID: 16173
		private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

		// Token: 0x02000FB6 RID: 4022
		private class AStarNodeComparer : IComparer<AStarNode>
		{
			// Token: 0x060055A6 RID: 21926 RVA: 0x001BAAD3 File Offset: 0x001B8CD3
			int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
