using System;

namespace Rust.AI
{
	// Token: 0x02000B56 RID: 2902
	public class AStarNode
	{
		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06004638 RID: 17976 RVA: 0x00199484 File Offset: 0x00197684
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x06004639 RID: 17977 RVA: 0x00199493 File Offset: 0x00197693
		public AStarNode(float g, float h, AStarNode parent, IAIPathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x001994B8 File Offset: 0x001976B8
		public void Update(float g, float h, AStarNode parent, IAIPathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x001994D7 File Offset: 0x001976D7
		public bool Satisfies(IAIPathNode node)
		{
			return this.Node == node;
		}

		// Token: 0x0600463C RID: 17980 RVA: 0x001994E2 File Offset: 0x001976E2
		public static bool operator <(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F < rhs.F;
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x001994F2 File Offset: 0x001976F2
		public static bool operator >(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F > rhs.F;
		}

		// Token: 0x04003F29 RID: 16169
		public AStarNode Parent;

		// Token: 0x04003F2A RID: 16170
		public float G;

		// Token: 0x04003F2B RID: 16171
		public float H;

		// Token: 0x04003F2C RID: 16172
		public IAIPathNode Node;
	}
}
