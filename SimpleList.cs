using System;

// Token: 0x02000953 RID: 2387
public class SimpleList<T>
{
	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x06003933 RID: 14643 RVA: 0x00153BA9 File Offset: 0x00151DA9
	public T[] Array
	{
		get
		{
			return this.array;
		}
	}

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x06003934 RID: 14644 RVA: 0x00153BB1 File Offset: 0x00151DB1
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06003935 RID: 14645 RVA: 0x00153BB9 File Offset: 0x00151DB9
	// (set) Token: 0x06003936 RID: 14646 RVA: 0x00153BC4 File Offset: 0x00151DC4
	public int Capacity
	{
		get
		{
			return this.array.Length;
		}
		set
		{
			if (value != this.array.Length)
			{
				if (value > 0)
				{
					T[] array = new T[value];
					if (this.count > 0)
					{
						System.Array.Copy(this.array, 0, array, 0, this.count);
					}
					this.array = array;
					return;
				}
				this.array = SimpleList<T>.emptyArray;
			}
		}
	}

	// Token: 0x17000489 RID: 1161
	public T this[int index]
	{
		get
		{
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x00153C34 File Offset: 0x00151E34
	public SimpleList()
	{
		this.array = SimpleList<T>.emptyArray;
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x00153C47 File Offset: 0x00151E47
	public SimpleList(int capacity)
	{
		this.array = ((capacity == 0) ? SimpleList<T>.emptyArray : new T[capacity]);
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x00153C68 File Offset: 0x00151E68
	public void Add(T item)
	{
		if (this.count == this.array.Length)
		{
			this.EnsureCapacity(this.count + 1);
		}
		T[] array = this.array;
		int num = this.count;
		this.count = num + 1;
		array[num] = item;
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x00153CB0 File Offset: 0x00151EB0
	public void Clear()
	{
		if (this.count > 0)
		{
			System.Array.Clear(this.array, 0, this.count);
			this.count = 0;
		}
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x00153CD4 File Offset: 0x00151ED4
	public bool Contains(T item)
	{
		for (int i = 0; i < this.count; i++)
		{
			if (this.array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x00153D16 File Offset: 0x00151F16
	public void CopyTo(T[] array)
	{
		System.Array.Copy(this.array, 0, array, 0, this.count);
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x00153D2C File Offset: 0x00151F2C
	public void EnsureCapacity(int min)
	{
		if (this.array.Length < min)
		{
			int num = ((this.array.Length == 0) ? 16 : (this.array.Length * 2));
			num = ((num < min) ? min : num);
			this.Capacity = num;
		}
	}

	// Token: 0x040033D9 RID: 13273
	private const int defaultCapacity = 16;

	// Token: 0x040033DA RID: 13274
	private static readonly T[] emptyArray = new T[0];

	// Token: 0x040033DB RID: 13275
	public T[] array;

	// Token: 0x040033DC RID: 13276
	public int count;
}
