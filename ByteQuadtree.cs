using System;
using UnityEngine;

// Token: 0x02000657 RID: 1623
[Serializable]
public sealed class ByteQuadtree
{
	// Token: 0x06002EEB RID: 12011 RVA: 0x0011A2F0 File Offset: 0x001184F0
	public void UpdateValues(byte[] baseValues)
	{
		this.size = Mathf.RoundToInt(Mathf.Sqrt((float)baseValues.Length));
		this.levels = Mathf.RoundToInt(Mathf.Max(Mathf.Log((float)this.size, 2f), 0f)) + 1;
		this.values = new ByteMap[this.levels];
		this.values[0] = new ByteMap(this.size, baseValues, 1);
		for (int i = 1; i < this.levels; i++)
		{
			ByteMap byteMap = this.values[i - 1];
			ByteMap byteMap2 = (this.values[i] = this.CreateLevel(i));
			for (int j = 0; j < byteMap2.Size; j++)
			{
				for (int k = 0; k < byteMap2.Size; k++)
				{
					byteMap2[k, j] = byteMap[2 * k, 2 * j] + byteMap[2 * k + 1, 2 * j] + byteMap[2 * k, 2 * j + 1] + byteMap[2 * k + 1, 2 * j + 1];
				}
			}
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06002EEC RID: 12012 RVA: 0x0011A40B File Offset: 0x0011860B
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002EED RID: 12013 RVA: 0x0011A413 File Offset: 0x00118613
	public ByteQuadtree.Element Root
	{
		get
		{
			return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
		}
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x0011A428 File Offset: 0x00118628
	private ByteMap CreateLevel(int level)
	{
		int num = 1 << this.levels - level - 1;
		int num2 = 1 + (level + 3) / 4;
		return new ByteMap(num, num2);
	}

	// Token: 0x040026CA RID: 9930
	[SerializeField]
	private int size;

	// Token: 0x040026CB RID: 9931
	[SerializeField]
	private int levels;

	// Token: 0x040026CC RID: 9932
	[SerializeField]
	private ByteMap[] values;

	// Token: 0x02000DB6 RID: 3510
	public struct Element
	{
		// Token: 0x0600516D RID: 20845 RVA: 0x001AC05D File Offset: 0x001AA25D
		public Element(ByteQuadtree source, int x, int y, int level)
		{
			this.source = source;
			this.x = x;
			this.y = y;
			this.level = level;
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x0600516E RID: 20846 RVA: 0x001AC07C File Offset: 0x001AA27C
		public bool IsLeaf
		{
			get
			{
				return this.level == 0;
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x0600516F RID: 20847 RVA: 0x001AC087 File Offset: 0x001AA287
		public bool IsRoot
		{
			get
			{
				return this.level == this.source.levels - 1;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06005170 RID: 20848 RVA: 0x001AC09E File Offset: 0x001AA29E
		public int ByteMap
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06005171 RID: 20849 RVA: 0x001AC0A6 File Offset: 0x001AA2A6
		public uint Value
		{
			get
			{
				return this.source.values[this.level][this.x, this.y];
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06005172 RID: 20850 RVA: 0x001AC0CB File Offset: 0x001AA2CB
		public Vector2 Coords
		{
			get
			{
				return new Vector2((float)this.x, (float)this.y);
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06005173 RID: 20851 RVA: 0x001AC0E0 File Offset: 0x001AA2E0
		public int Depth
		{
			get
			{
				return this.source.levels - this.level - 1;
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06005174 RID: 20852 RVA: 0x001AC0F6 File Offset: 0x001AA2F6
		public ByteQuadtree.Element Parent
		{
			get
			{
				if (this.IsRoot)
				{
					throw new Exception("Element is the root and therefore has no parent.");
				}
				return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
			}
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06005175 RID: 20853 RVA: 0x001AC12E File Offset: 0x001AA32E
		public ByteQuadtree.Element Child1
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06005176 RID: 20854 RVA: 0x001AC166 File Offset: 0x001AA366
		public ByteQuadtree.Element Child2
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06005177 RID: 20855 RVA: 0x001AC1A0 File Offset: 0x001AA3A0
		public ByteQuadtree.Element Child3
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06005178 RID: 20856 RVA: 0x001AC1DA File Offset: 0x001AA3DA
		public ByteQuadtree.Element Child4
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06005179 RID: 20857 RVA: 0x001AC218 File Offset: 0x001AA418
		public ByteQuadtree.Element MaxChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				if (value >= value2 && value >= value3 && value >= value4)
				{
					return child;
				}
				if (value2 >= value3 && value2 >= value4)
				{
					return child2;
				}
				if (value3 >= value4)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x0600517A RID: 20858 RVA: 0x001AC290 File Offset: 0x001AA490
		public ByteQuadtree.Element RandChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				float num = value + value2 + value3 + value4;
				float value5 = UnityEngine.Random.value;
				if (value / num >= value5)
				{
					return child;
				}
				if ((value + value2) / num >= value5)
				{
					return child2;
				}
				if ((value + value2 + value3) / num >= value5)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x04004910 RID: 18704
		private ByteQuadtree source;

		// Token: 0x04004911 RID: 18705
		private int x;

		// Token: 0x04004912 RID: 18706
		private int y;

		// Token: 0x04004913 RID: 18707
		private int level;
	}
}
