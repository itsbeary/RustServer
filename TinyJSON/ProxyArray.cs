using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x020009DC RID: 2524
	public sealed class ProxyArray : Variant, IEnumerable<Variant>, IEnumerable
	{
		// Token: 0x06003C0E RID: 15374 RVA: 0x0016276B File Offset: 0x0016096B
		public ProxyArray()
		{
			this.list = new List<Variant>();
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x0016277E File Offset: 0x0016097E
		IEnumerator<Variant> IEnumerable<Variant>.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003C10 RID: 15376 RVA: 0x0016277E File Offset: 0x0016097E
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003C11 RID: 15377 RVA: 0x00162790 File Offset: 0x00160990
		public void Add(Variant item)
		{
			this.list.Add(item);
		}

		// Token: 0x170004DC RID: 1244
		public override Variant this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06003C14 RID: 15380 RVA: 0x001627BB File Offset: 0x001609BB
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x06003C15 RID: 15381 RVA: 0x001627C8 File Offset: 0x001609C8
		internal bool CanBeMultiRankArray(int[] rankLengths)
		{
			return this.CanBeMultiRankArray(0, rankLengths);
		}

		// Token: 0x06003C16 RID: 15382 RVA: 0x001627D4 File Offset: 0x001609D4
		private bool CanBeMultiRankArray(int rank, int[] rankLengths)
		{
			int count = this.list.Count;
			rankLengths[rank] = count;
			if (rank == rankLengths.Length - 1)
			{
				return true;
			}
			ProxyArray proxyArray = this.list[0] as ProxyArray;
			if (proxyArray == null)
			{
				return false;
			}
			int count2 = proxyArray.Count;
			for (int i = 1; i < count; i++)
			{
				ProxyArray proxyArray2 = this.list[i] as ProxyArray;
				if (proxyArray2 == null)
				{
					return false;
				}
				if (proxyArray2.Count != count2)
				{
					return false;
				}
				if (!proxyArray2.CanBeMultiRankArray(rank + 1, rankLengths))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040036DF RID: 14047
		private readonly List<Variant> list;
	}
}
